using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrivingLicense.Managers;
using DrivingLicense.Models;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace DrivingLicense.Commands
{
    public class CommandLicense : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = caller as UnturnedPlayer;

            var zone = Main.Instance.Configuration.Instance.Zones.FirstOrDefault(e =>
                Vector3.Distance(player.Position, new Vector3(e.X, e.Y, e.Z)) < 10);
            if (zone == null)
            {
                QuestsManager.SendMessage(player.CSteamID, "Burada lisans alabileceğin bir yer yok.", 3f);
                return;
            }

            var license =
                Main.Instance.Configuration.Instance.Licenses.FirstOrDefault(l => l.LicenseName == zone.LicenseName);
            if (license == null) return;

            if (license.Players.Contains(player.CSteamID.m_SteamID))
            {
                QuestsManager.SendMessage(player.CSteamID, "Bu lisansa zaten sahipsin.", 3f);
                return;
            }

            if (player.Experience < license.LicensePrice)
            {
                QuestsManager.SendMessage(player.CSteamID, $"Bu lisans eğitimi için üzerinde yeterince para yok. Lisans eğitimi fiyatı <color=green>$</color>{license.LicensePrice}", 3f);
                return;
            }

            if(player.CurrentVehicle != null) player.CurrentVehicle.forceRemoveAllPlayers();

            player.Experience -= license.LicensePrice;

            var vehicle = VehicleManager.spawnVehicleV2(zone.TrailVehicle, new Vector3(zone.VX, zone.VY + 2, zone.VZ),
                Quaternion.Euler(0, zone.VRotation, 0));

            vehicle.tellLocked(player.CSteamID, CSteamID.Nil, true);
            VehicleManager.instance.channel.send("tellEnterVehicle", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
            {
                vehicle.instanceID,
                (byte)0,
                player.CSteamID
            });

            QuestsManager.Quests.Add(player.CSteamID, new Quest()
            {
                Index = 0,
                Vehicle = vehicle,
                Zone = zone
            });

            Main.Instance.StartCoroutine(QuestsManager.CheckPlayer(player.CSteamID));
            var nextZone = zone.Quests[0];
            EffectManager.sendEffectReliable(Main.Instance.Configuration.Instance.Effect, player.CSteamID, new Vector3(nextZone.X, nextZone.Y, nextZone.Z));
            QuestsManager.SendMessage(player.CSteamID, $"Eğitim başladı <color=red> Kırmızı İşaretlere</color> doğru git ve adım tamamlandı yazısını bekle.",5f);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "license";

        public string Help => "Bu bölgede lisans alabileceğiniz bir şey varsa onu kurar falan filan";

        public string Syntax => "license";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>(){"alosha.license"};
    }
}
