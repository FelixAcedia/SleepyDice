using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using SleepyDice.Utilites;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SleepyDice.Service;

public static class UserService{
    public static User GetUser(Entity userEntity) => ServerUtilities.EntityManager.GetComponentData<User>(userEntity); 

    public static UserBitMask128.Enumerable GetUsersInRange(float3 position) {
        UserActivityGrid  userActivityGrid = ServerUtilities.UserActivityGridSystem.GetUserActivityGrid();
        UserBitMask128 userBitMask = userActivityGrid.GetUsersInRadius(position, 40f);
        return userBitMask.GetUsers();
    }

    public static IEnumerable<Entity> GetClanAllies(Entity userEntity) {
        Team unitTeam = ServerUtilities.EntityManager.GetComponentData<Team>(userEntity);
        var teamEntities = ServerUtilities.EntityManager.CreateEntityQuery(new EntityQueryDesc() { All = new ComponentType[] {
                ComponentType.ReadOnly<Team>()
            },
                Options = EntityQueryOptions.IncludeDisabled
        }).ToEntityArray(Allocator.Temp);
        foreach (Entity members in teamEntities) {
            if (ServerUtilities.EntityManager.GetComponentData<Team>(members).Clan == unitTeam.Clan) {
                yield return members;
            }
        }
        teamEntities.Dispose();
    }
}