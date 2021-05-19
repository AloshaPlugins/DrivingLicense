using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrivingLicense.Models;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace DrivingLicense.Managers
{
    public static class QuestsManager
    {
        public static Dictionary<CSteamID, Quest> Quests = new Dictionary<CSteamID, Quest>();

        public static void SendMessage(CSteamID id, string content, float second) =>
            Main.Instance.StartCoroutine(MessageTimer(id, content, second));
        private static IEnumerator MessageTimer(CSteamID id, string content, float second)
        {
            EffectManager.sendUIEffect(Main.Instance.Configuration.Instance.EffectMessage, 315, id,true, content);
            yield return new WaitForSeconds(second);
            EffectManager.askEffectClearByID(Main.Instance.Configuration.Instance.EffectMessage, id);
        }

        public static IEnumerator CheckPlayer(CSteamID id)
        {
            int time = 0;
            while (true)
            {
                yield return new WaitForSeconds(1f);
                var data = Quests.FirstOrDefault(quest => quest.Key == id);
                if (data.Key == null) yield break;

                var player = UnturnedPlayer.FromCSteamID(id);
                if (player == null)
                {
                    Quests.Remove(id);
                    yield break;
                }


                if (data.Value.Vehicle == null || (data.Value.Vehicle != null && data.Value.Vehicle.isExploded) || (data.Value.Vehicle != null && data.Value.Vehicle.isUnderwater))
                {
                    Quests.Remove(id);
                    SendMessage(id, "<color=red> BAŞARAMADIN! </color>", 5f);
                    yield break;
                }
                var index = data.Value.Index;
                var zone = data.Value.Zone;

                var nextZone = zone.Quests[index];

                if (player.CurrentVehicle == null || (player.CurrentVehicle != data.Value.Vehicle))
                {
                    time += 1;
                    if (time > zone.RightToError)
                    {
                        var vehicle = data.Value.Vehicle;
                        vehicle.forceRemoveAllPlayers();
                        VehicleManager.askVehicleDestroy(vehicle);
                        Quests.Remove(id);
                        SendMessage(id, "<color=red> BAŞARAMADIN! </color>", 5f);
                        EffectManager.askEffectClearByID(Main.Instance.Configuration.Instance.Effect, player.CSteamID);
                        yield break;
                    }
                    continue;
                }

                var vector = new Vector3(nextZone.X, nextZone.Y, nextZone.Z);
                if (Vector3.Distance(vector, player.Position) <= 10)
                {
                    data.Value.Index += 1;

                    if (data.Value.Index > zone.Quests.Count - 1)
                    {
                        var vehicle = data.Value.Vehicle;
                        vehicle.forceRemoveAllPlayers();
                        VehicleManager.askVehicleDestroy(vehicle);

                        var license = Main.Instance.Configuration.Instance.Licenses.FirstOrDefault(l =>
                            l.LicenseName == data.Value.Zone.LicenseName);
                        if (license == null) yield break;

                        license.Players.Add(id.m_SteamID);
                        Quests.Remove(id);
                        Main.Instance.Configuration.Save();
                        EffectManager.askEffectClearByID(Main.Instance.Configuration.Instance.Effect, player.CSteamID);
                        SendMessage(id,
                            $"<size=30><color=cyan>TÜM ADIMLARI TAMAMLADIN!</color></size>\n\n<size=20>Artık <color=yellow>{ license.LicenseName}</color> lisansına sahipsin.</size>", 10f);
                        yield break;
                    }
                    EffectManager.askEffectClearByID(Main.Instance.Configuration.Instance.Effect, player.CSteamID);
                    nextZone = zone.Quests[data.Value.Index];
                    var vector2 = new Vector3(nextZone.X, nextZone.Y, nextZone.Z);
                    EffectManager.sendEffectReliable(Main.Instance.Configuration.Instance.Effect, player.CSteamID, vector2);
                    player.Player.quests.tellSetMarker(player.CSteamID, true, vector2, $"{data.Value.Index}/{zone.Quests.Count}");

                    SendMessage(id,
                        $"<color=yellow>{data.Value.Index}/{zone.Quests.Count}</color>", 5f);
                }
            }
        }

        public static void Clear() => Quests.Clear();
    }
}
