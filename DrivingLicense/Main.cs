using DrivingLicense.Managers;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System.Linq;

namespace DrivingLicense
{
    public class Main : RocketPlugin<Config>
    {
        public static Main Instance;
        protected override void Load()
        {
            Instance = this;
            VehicleManager.onEnterVehicleRequested += VehicleManagerOnonEnterVehicleRequested;
            VehicleManager.onSwapSeatRequested += VehicleManagerOnonSwapSeatRequested;
        }

        private void VehicleManagerOnonSwapSeatRequested(Player player, InteractableVehicle vehicle, ref bool shouldallow, byte fromseatındex, ref byte toseatındex)
        {
            if (toseatındex != 0) return;
            var license =
                Configuration.Instance.Licenses.FirstOrDefault(e => e.AvaiableVehicles.Contains(vehicle.asset.id));
            if (license == null) return;

            if (license.Players.Contains(player.channel.owner.playerID.steamID.m_SteamID)) return;
            shouldallow = false;
            QuestsManager.SendMessage(player.channel.owner.playerID.steamID, $"<color=red> Bu araçta sürücü koltuğuna geçemezsin <color=yellow>{license.LicenseName}</color> lisansına ihtiyacın var. </color>", 4f);
        }

        private void VehicleManagerOnonEnterVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldallow)
        {
            var license =
                Configuration.Instance.Licenses.FirstOrDefault(e => e.AvaiableVehicles.Contains(vehicle.asset.id));
            if (license == null) return;

            if (license.Players.Contains(player.channel.owner.playerID.steamID.m_SteamID))
            {
                if (license.DriverToNeedLicenseItem)
                {
                    if (player.inventory.has(license.LicenseItem) != null)
                    {
                        shouldallow = true;
                        return;
                    }
                    shouldallow = false;
                    QuestsManager.SendMessage(player.channel.owner.playerID.steamID, $"<color=white> Bu aracı sürebilmek için <color=yellow>{(Assets.find(EAssetType.ITEM, license.LicenseItem) as ItemAsset).itemName}</color> eşyasına ihtiyacın var. </color>", 4f);
                    return;
                }
            }
            shouldallow = false;
            if (vehicle.tryAddPlayer(out var seat, player))
            {
                if (seat == 0)
                {
                    for (byte i = 1; i < vehicle.passengers.Length; i++)
                    {
                        if (vehicle.passengers[i].player == null)
                        {
                            VehicleManager.instance.channel.send("tellEnterVehicle", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                            {
                                vehicle.instanceID,
                                i,
                                player.channel.owner.playerID.steamID
                            });
                            QuestsManager.SendMessage(player.channel.owner.playerID.steamID, $"<color=white> Bu aracı sürebilmek için <color=yellow>{license.LicenseName}</color> lisansına ihtiyacın var.</color>", 4f);
                            return;
                        }
                    }
                    QuestsManager.SendMessage(player.channel.owner.playerID.steamID, $"<color=white> Bu aracı sürebilmek için <color=yellow>{license.LicenseName}</color> lisansına ihtiyacın var.</color>", 4f);
                    return;
                }
                else
                {
                    shouldallow = true;
                }
            }
        }

        protected override void Unload()
        {
            Instance = null;
            VehicleManager.onEnterVehicleRequested -= VehicleManagerOnonEnterVehicleRequested;
            VehicleManager.onSwapSeatRequested -= VehicleManagerOnonSwapSeatRequested;
            QuestsManager.Clear();
        }
    }
}
