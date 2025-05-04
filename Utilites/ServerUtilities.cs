using ProjectM;
using ProjectM.Network;
using ProjectM.Scripting;
using Unity.Entities;

namespace SleepyDice.Utilites;

public class ServerUtilities{
    private static World? _serverWorld;
    public static EntityManager EntityManager => Server.EntityManager;
    public static UserActivityGridSystem UserActivityGridSystem =>  Server.GetExistingSystemManaged<UserActivityGridSystem>();
    public static ServerBootstrapSystem  ServerBootstrapSystem =>  Server.GetExistingSystemManaged<ServerBootstrapSystem>();
    
    public static World Server {
        get {
            if (_serverWorld != null) return _serverWorld;
            _serverWorld = GetWorld("Server") ?? throw new System.Exception("There is no server.");
            return _serverWorld;
        }
    }
    private static World GetWorld(string name) {
        foreach (var world in World.s_AllWorlds) {
            if (world.Name == name) {
                return world;
            }
        }
        return null;
    }
    
    
}