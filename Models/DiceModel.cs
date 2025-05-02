using System.Linq;

namespace SleepyDice.Models;

public enum DiceModel : byte {
    Sum,
    Highest,
    Lowest
}
internal class DiceResult{
    public static byte EvaluateWithMode(DiceModel model, byte[] rolls) {
        return model switch {
            DiceModel.Sum => (byte)rolls.Sum(x => x),
            DiceModel.Highest => rolls.Max(),
            DiceModel.Lowest => rolls.Min(),
            _ => throw new System.Exception("Invalid mode")
        };
    }
}