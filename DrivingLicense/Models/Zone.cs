using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingLicense.Models
{
    public class Zone
    {
        public string Name, LicenseName;
        public ushort TrailVehicle;
        public int RightToError;
        public float X, Y, Z, VRotation, VX, VY, VZ;

        public List<ZoneQuest> Quests;
    }
}
