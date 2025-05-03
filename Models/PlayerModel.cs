using SleepyDice.Utilites;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SleepyDice.Models;

public class PlayerModel{
    public static float3 GetCharacterPosition(Entity player) =>
        ServerUtilities.EntityManager.GetComponentData<LocalToWorld>(player).Position;
}