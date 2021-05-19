using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrivingLicense.Managers;
using DrivingLicense.Models;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace DrivingLicense.Commands
{
    public class CommandLicenseZone : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = caller as UnturnedPlayer;

            // /licensezone create 
            var name = command[0];

            if (name == "create")
            {
                var licenseName = command[1];
                if (!Main.Instance.Configuration.Instance.Licenses.Any(e => e.LicenseName == licenseName))
                {
                    UnturnedChat.Say(player, "Geçerli bir lisans ismi gir.");
                    return;
                }
                var zoneName = command[2];
                if (!ushort.TryParse(command[3], out var result))
                {
                    UnturnedChat.Say(player, "Geçerli bir araç id gir.");
                    return;
                }
                var zone = new Zone()
                {
                    LicenseName = licenseName,
                    Quests = new List<ZoneQuest>(),
                    Y = player.Position.y,
                    X = player.Position.x,
                    Z = player.Position.z,
                    VRotation = 0,
                    VY = 0,
                    VX = 0,
                    VZ = 0,
                    Name = zoneName,
                    TrailVehicle = result
                };

                Main.Instance.Configuration.Instance.Zones.Add(zone);
                Main.Instance.Configuration.Save();
                UnturnedChat.Say(player, "Bölge oluşturuldu.");
                return;
            }

            if (name == "quest")
            {
                var zoneName = command[1];
                if (!Main.Instance.Configuration.Instance.Zones.Any(e => e.Name == zoneName))
                {
                    UnturnedChat.Say(player, "Geçerli bir bölge ismi gir.");
                    return;
                }

                var zone = Main.Instance.Configuration.Instance.Zones.FirstOrDefault(e => e.Name == zoneName);
                var name2 = command[2];

                if (name2 == "add")
                {
                    if (!int.TryParse(command[3], out var result))
                    {
                        UnturnedChat.Say(player, "Geçerli bir süre limiti gir.");
                        return;
                    }
                    zone.Quests.Add(new ZoneQuest()
                    {
                        Time = result,
                        X = player.Position.x,
                        Z = player.Position.z,
                        Y = player.Position.y
                    });
                    Main.Instance.Configuration.Save();
                    UnturnedChat.Say(player, "Alana yeni bir quest eklendi.");
                    return;
                }
                if (name2 == "remove")
                {
                    if (!int.TryParse(command[3], out var index))
                    {
                        UnturnedChat.Say(player, "Geçerli bir index gir.");
                        return;
                    }

                    if (index > zone.Quests.Count - 1)
                    {
                        UnturnedChat.Say(player, "Girdiğin sayı dizi sınırlarının dışındaydı.");
                        return;
                    }

                    zone.Quests.RemoveAt(index);
                    Main.Instance.Configuration.Save();
                    UnturnedChat.Say(player, "Alandan bir quest çıkarıldı.");
                    return;
                }

                return;
            }

            if (name == "modify")
            {
                var zoneName = command[1];
                if (!Main.Instance.Configuration.Instance.Zones.Any(e => e.Name == zoneName))
                {
                    UnturnedChat.Say(player, "Geçerli bir bölge ismi gir.");
                    return;
                }

                var zone = Main.Instance.Configuration.Instance.Zones.FirstOrDefault(e => e.Name == zoneName);
                var name2 = command[2];

                if (name2 == "vehiclespawn")
                {
                    zone.VX = player.Position.x;
                    zone.VY = player.Position.y;
                    zone.VZ = player.Position.z;
                    zone.VRotation = player.Rotation;
                    Main.Instance.Configuration.Save();
                    UnturnedChat.Say(player, "Bölge araç spawnı değiştirildi.");
                    return;
                }
            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "licensezone";

        public string Help => "Bu bölgede lisans alabileceğiniz bir şey varsa onu kurar falan filan";

        public string Syntax => "licensezone";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>(){ "alosha.licensezone" };
    }
}
