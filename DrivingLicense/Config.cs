using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrivingLicense.Models;
using Rocket.API;

namespace DrivingLicense
{
    public class Config : IRocketPluginConfiguration
    {
        public ushort Effect, EffectMessage;
        public List<License> Licenses;
        public List<Zone> Zones;
        public void LoadDefaults()
        {
            Effect = 24020;
            EffectMessage = 24021;
            Licenses = new List<License>()
            {
                new License()
                {
                    LicenseName = "Jeep License",
                    LicensePrice = 500,
                    LicenseItem = 1328,
                    AvaiableVehicles = new List<ushort>(){2,3,4},
                    DriverToNeedLicenseItem = false,
                    Players = new List<ulong>()
                }
            };
            Zones = new List<Zone>();
        }
    }
}
