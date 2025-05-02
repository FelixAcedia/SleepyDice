using System;
using SleepyDice.Models;
using SleepyDice.Utilites;
using ProjectM;
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
                                              ChatUtilities.Color(color, $"{result}") + 
                                              "] in [" +
                                              ChatUtilities.Color(color, $"{result + modify}") +
                                              "]");
        ServerChatUtils.SendSystemMessageToAllClients(ServerUtilities.EntityManager, ref fixedString);
    }
}