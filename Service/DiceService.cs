using System;
using System.Linq;
using System.Text.RegularExpressions;
using SleepyDice.Models;

namespace SleepyDice.Service;

internal class DiceService{
    
    public static Random _Random = new Random();

    public static int[] ValidDie = [
        4, 
        6, 
        8, 
        10, 
        12, 
        20, 
        100
    ];
    
    public static bool IsValidThrow(int amount) => amount > 0 && amount <= 4;
    public static bool IsValidDie(int die) => ValidDie.Contains(die);

    public static bool IsValidRoll(string rollstring, out int amount, out int die, out int modify, out string error) {
        amount = 0; die = 0; modify = 0; error = "";

        string regPattern = @"^(\d+|)d(\d+)([+-]\d+)*$";
        var match = Regex.Match(rollstring, regPattern);

        if (!match.Success) {
            error = "Roll string does not match format";
            return false;
        }
        
        if (!int.TryParse(match.Groups[1].Value, out amount) && match.Groups[1].Value == "")
            amount = 1;
        else if (!IsValidThrow(amount)) {
            error = "Invalid amount of throws";
            return false;
        }

        CaptureCollection cc = match.Groups[3].Captures;
        
        if (cc.Count > 0) {
            for (int i = 0; i < cc.Count; i++) {
                if (!int.TryParse(cc[i].Value, out int temp)) {
                    error = "Invalid modifier";
                    return false;
                }
                modify += temp;
            }
        }
        return int.TryParse(match.Groups[2].Value, out die) && IsValidDie(die) ;
    }
    
    public static int RollDie(int die) => _Random.Next(die) + 1;

    public static int RollDices(DiceModes mode, int die, int amount, out int[] rolls) {
        rolls = Enumerable.Repeat(0, amount).Select(x => RollDie(die)).ToArray();
        return mode switch {
            DiceModes.Sum => rolls.Sum(x => x),
            DiceModes.Highest => rolls.Max(),
            DiceModes.Lowest => rolls.Min(),
            _ => throw new System.Exception("Invalid mode")
        };
    }

    public static string GetResultFormat(int result, DiceModes mode, int die, int amount, int modify) {
        if (result == 1 + modify) return "#E90000";
        else if ((mode != DiceModes.Sum && result == die + modify) || result == die*amount + modify) return "#7FE030";
        else return "#E67E22";
    }
}