using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingLicense.Models
{
    public class License
    {
        public string LicenseName;
        public uint LicensePrice;
        public bool DriverToNeedLicenseItem;
        public ushort LicenseItem;

        public List<ushort> AvaiableVehicles;
        public List<ulong> Players;
    }
}
