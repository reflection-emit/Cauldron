#if WINDOWS_UWP

using System;
using Windows.Storage.Streams;
using Windows.System.Profile;

#elif ANDROID

using Android.OS;

#elif NETCORE

using System.Net.NetworkInformation;
using System.Text;

#else

using System.Management;

#endif

using Cauldron.Core.Extensions;

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

                return Convert.ToBase64String(bytes).GetHash(HashAlgorithms.Sha256);
#elif ANDROID
                return Build.Serial.GetHash(HashAlgorithms.Sha256);
#elif NETCORE
                // TODO - This is not enough to uniquely identify a hardware...
                // We have to find out later on how we could do this better in the future
                var sb = new StringBuilder();

                foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
                    sb.Append(adapter.GetPhysicalAddress().ToString());

                return sb.ToString().GetHash(HashAlgorithms.Sha256);
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