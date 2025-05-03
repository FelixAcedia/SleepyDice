using System;
using SleepyDice.Models;
using SleepyDice.Utilites;
using ProjectM;
using ProjectM.Network;
using SleepyDice.Service;
using Stunlock.Network;
using Unity.Collections;
using Unity.Entities;
using VampireCommandFramework;
using static SleepyDice.Service.DiceService;

namespace SleepyDice.Commands;

/// <summary>
/// VCF command to roll a die with preset dice formats and modes
/// </summary>
[CommandGroup("dice", "d")]
public class DiceCommands{
    [Command("roll", "r", null, "Rolls a dice")]
    public void Roll(ChatCommandContext ctx, string dies = "1d20", DiceModel model = DiceModel.Sum)
    {
        if (!IsValidRoll(dies, out byte amount, out byte die, out sbyte modify, out string error)) {
            ctx.Reply(error);
            return;
        }

        byte[] rolls = new byte[amount];
        if (amount == 1)
            rolls[0] = RollSingleDice(die);
        else
            rolls = RollMultipleDices(die, amount);
        short result = DiceResult.EvaluateWithMode(model, rolls);

        string color = "";
        if (result == 1 + modify) color = "#E90000";
        else if ((model != DiceModel.Sum && result == die + modify) || result == die*amount + modify) color = "#7FE030";
        else color = "#E67E22";
        
        char modifyChar = modify >= 0 ? '+' : '-';
        
        FixedString512Bytes fixedString = new($"{ctx.User.CharacterName} rolled with {amount}d{die}{modifyChar}{modify} and got [{String.Join(',', rolls)}] - resulting with [" +
                                              Format.Color(color, $"{result}") + 
                                              "] in [" +
                                              Format.Color(color, $"{result + modify}") +
                                              "]");
        
        switch (ctx.Event.Type) {
            case ChatMessageType.Whisper:
                ctx.Reply("Dice rolling doesn't work within Whispers");
                break;
            case ChatMessageType.Team:
                ctx.Reply("Dice rolling doesn't work within Clan Chat");
                break;
            case ChatMessageType.Local:
                var users = UserService.GetUsersInRange(PlayerModel.GetCharacterPosition(ctx.User.LocalCharacter._Entity));
                foreach (int userIndex in users) {
                    ServerChatUtils.SendSystemMessageToClient(ServerUtilities.EntityManager, 
                        UserService.GetUser(ServerUtilities.ServerBootstrapSystem._ApprovedUsersLookup[userIndex].UserEntity),
                        ref fixedString);
                }
                break;
            default:
                ServerChatUtils.SendSystemMessageToAllClients(ServerUtilities.EntityManager, ref fixedString);
                break;
            
        }
    }
}