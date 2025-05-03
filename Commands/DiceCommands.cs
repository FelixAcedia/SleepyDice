using System;
using ProjectM;
using ProjectM.Network;
using SleepyDice.Models;
using SleepyDice.Service;
using SleepyDice.Utilites;
using Unity.Collections;
using VampireCommandFramework;
using static SleepyDice.Service.DiceService;

namespace SleepyDice.Commands;

/// <summary>
/// VCF command to roll a die with preset dice formats and modes
/// </summary>
[CommandGroup("dice", "d")]
public class DiceCommands{
    [Command("roll", "r", null, "Rolls a dice")]
    public void Roll(ChatCommandContext ctx, string dies = "1d20", DiceModel mode = DiceModel.Sum)
    {
        if (!IsValidRoll(dies, out int amount, out int die, out int modify, out string error)) {
            ctx.Reply(error);
            return;
        }
        
        FixedString512Bytes fixedString = new FixedString512Bytes();
        int result = 0;
        if (amount == 1) {
            result = RollSingleDie(die) + modify;
            fixedString = $"{ctx.User.CharacterName} rolled with {amount}d{die}{modify:+##;-##;''} - resulting in [" +
                          Format.Color($"{result}", GetResultFormat(result, mode, die, amount, modify)) + 
                          "]";;
        }
        else {
            result = RollMultipleDices(mode, die, amount, out int[] rolls);
            fixedString = $"{ctx.User.CharacterName} rolled with {amount}d{die}{modify:+##;-##;''}, rolling [{String.Join(',', rolls)}] - resulting in [" +
                          Format.Color($"{result}", GetResultFormat(result, mode, die, amount, modify)) + 
                          "]";;

        }
        
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