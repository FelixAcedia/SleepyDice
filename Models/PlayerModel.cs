using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using SleepyDice.Utilites;
using Stunlock.Core;
using Stunlock.Network;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace SleepyDice.Models;

public class PlayerModel{
    public static float3 GetCharacterPosition(Entity player) =>
        ServerUtilities.EntityManager.GetComponentData<LocalToWorld>(player).Position;
}