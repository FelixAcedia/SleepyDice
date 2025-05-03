using System;
using System.Linq;
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
        amount = 0;
        die = 0;
        modify = 0;
        error = "";
        if (string.IsNullOrEmpty(rollstring) || !rollstring.Contains('d') || rollstring.Count(c => c == 'd') != 1) {
            error = "The dice format is incorrect";
            return false;
        }
        if (rollstring.Contains('+') && (rollstring.IndexOf('d') > rollstring.IndexOf('+')) || 
            rollstring.Contains('-') && (rollstring.IndexOf('d') > rollstring.IndexOf('-'))) {
            error = "The dice format is incorrect";
            return false;
        }
        string[] args = rollstring.Split(new[] { 'd', '+', '-'});
        if (!int.TryParse(args[0], out amount) && args[0] == "")
            amount = 1;
        else if (!IsValidThrow(amount)) {
            error = "Invalid amount of throws";
            return false;
        }
        if (args.Length > 2) {
            int pos = args[0].Length + args[1].Length;
            for (int i = 2; i < args.Length; i++) {
                if (!sbyte.TryParse(args[i], out sbyte temp)) {
                    error = "Invalid modifier";
                    return false;
                }
                if (rollstring[pos + 1] == '-') {
                    temp *= -1;
                }
                pos += args[i].Length + 1;
                modify += temp;
            }
            
        }
        return int.TryParse(args[1], out die) && IsValidDie(die) ;
    }
    
    public static int RollSingleDie(int die) => _Random.Next(1, die + 1);

    public static int RollMultipleDices(DiceModel mode, int die, int amount, out int[] rolls) {
        rolls = Enumerable.Repeat(0, amount).Select(x => RollSingleDie(die)).ToArray();
        return mode switch {
            DiceModel.Sum => rolls.Sum(x => x),
            DiceModel.Highest => rolls.Max(),
            DiceModel.Lowest => rolls.Min(),
            _ => throw new System.Exception("Invalid mode")
        };
    }

    public static string GetResultFormat(int result, DiceModel mode, int die, int amount, int modify) {
        if (result == 1 + modify) return "#E90000";
        else if ((mode != DiceModel.Sum && result == die + modify) || result == die*amount + modify) return "#7FE030";
        else return "#E67E22";
    }
}