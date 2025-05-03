using ProjectM.Network;
using SleepyDice.Utilites;
using Unity.Entities;
using Unity.Mathematics;

namespace SleepyDice.Service;

public class UserService{
    public static User GetUser(Entity userEntity) => ServerUtilities.EntityManager.GetComponentData<User>(userEntity); 

    public static UserBitMask128.Enumerable GetUsersInRange(float3 position) {
        UserActivityGrid  userActivityGrid = ServerUtilities.UserActivityGridSystem.GetUserActivityGrid();
        UserBitMask128 userBitMask = userActivityGrid.GetUsersInRadius(position, 20f);
        return userBitMask.GetUsers();
    }
}