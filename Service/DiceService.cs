using System;
using System.Linq;

namespace SleepyDice.Service;

internal class DiceService{
    
    public static Random _Random = new Random();

    public static byte[] ValidDie = [
        4, 
        6, 
        8, 
        10, 
        12, 
        20, 
        100
    ];
    
    public static bool IsValidThrow(byte amount) => amount <= 4;
    public static bool IsValidDie(byte die) => ValidDie.Contains(die);

    public static bool IsValidRoll(string rollstring, out byte amount, out byte die, out sbyte modify, out string error) {
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
        if (!byte.TryParse(args[0], out amount) && args[0] == "")
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
        return byte.TryParse(args[1], out die) && IsValidDie(die) ;
    }
    
    public static byte RollSingleDice(byte die) => (byte)_Random.Next(1, die - 1);

    public static byte[] RollMultipleDices(int die, int count) {
        byte[] rolls = new byte[count];
        for (int i = 0; i < count; i++) {
             rolls[i] = (byte)_Random.Next(1, die - 1);
        }
        return rolls;
    }
}