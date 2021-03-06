using System.Collections.Generic;

namespace EasyCraft.Base.User
{
    internal class Permission
    {
        internal static readonly Dictionary<PermissionId, UserType> PermissionList = new()
        {
            { PermissionId.Nothing, UserType.Nobody },
            { PermissionId.ChangeCore, UserType.Registered },
            { PermissionId.ChangeStarter, UserType.Registered },
            { PermissionId.ChangeServerExpire, UserType.Admin },
            { PermissionId.ChangeServerName, UserType.Admin },
            { PermissionId.ChangeServerPort, UserType.Admin },
            { PermissionId.ChangeServerAutoStart, UserType.Admin },
            { PermissionId.ChangeServerRam, UserType.Admin },
            { PermissionId.ChangeServerMaxPlayer, UserType.Admin },
            { PermissionId.CreateServer, UserType.Admin }
        };

        public static void LoadPermissions()
        {
            var reader = Database.Database.CreateCommand("SELECT id, type FROM permissions").ExecuteReader();
            while (reader.Read())
            {
                PermissionList[(PermissionId)reader.GetInt32(0)] = (UserType)reader.GetInt32(1);
            }
        }

        public static bool CheckPermission(PermissionId pid, UserType type)
        {
            return PermissionList[pid] >= type;
        }
    }

    public enum PermissionId
    {
        Nothing,
        ChangeCore,
        ChangeStarter,
        ChangeServerAutoStart,
        ChangeServerRam,
        ChangeServerExpire,
        ChangeServerMaxPlayer,
        ChangeServerPort,
        ChangeServerName,
        CreateServer
    }
}