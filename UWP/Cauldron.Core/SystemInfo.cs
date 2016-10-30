#if WINDOWS_UWP

using System;

using Windows.Storage.Streams;
using Windows.System.Profile;

#else

using Cauldron.Core.Extensions;
using System.Management;

#endif

namespace Cauldron.Core
{
    /// <summary>
    /// Provides methods for retrieving system information
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// Returns a string that can be used to identify the hardware
        /// </summary>
        public static string HardwareIdentifier
        {
            get
            {
#if WINDOWS_UWP
                var token = HardwareIdentification.GetPackageSpecificToken(null);
                var id = token.Id;
                var bytes = new byte[id.Length];

                using (var reader = DataReader.FromBuffer(id))
                    reader.ReadBytes(bytes);

                return Convert.ToBase64String(bytes);
#else
                string result = string.Empty;

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
                foreach (ManagementObject getserial in searcher.Get())
                    result += getserial["SerialNumber"].ToString();

                searcher = new ManagementObjectSearcher("Select * From Win32_processor");
                foreach (ManagementObject getPID in searcher.Get())
                    result += getPID["ProcessorID"].ToString();

                return result.GetHash(HashAlgorithms.Sha256);
#endif
            }
        }
    }
}