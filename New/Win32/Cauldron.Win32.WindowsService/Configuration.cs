using Newtonsoft.Json;
using System.ServiceProcess;

namespace Cauldron.WindowsService
{
    internal class Configuration
    {
        [JsonProperty("account")]
        public ServiceAccount Account { get; private set; }

        [JsonProperty("delayed-autostart")]
        public bool DelayedAutoStart { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("display-name")]
        public string DisplayName { get; private set; }

        [JsonProperty("eventlog-name")]
        public string EventLogName { get; set; } = "Application";

        [JsonProperty("first-failure")]
        public RecoveryAction FirstFailure { get; set; } = RecoveryAction.None;

        [JsonProperty("reset-fail-counter-after")]
        public int ResetFailCountAfter { get; set; }

        [JsonProperty("restart-service-after")]
        public int RestartServiceAfter { get; set; }

        [JsonProperty("run-program")]
        public string RunProgram { get; set; }

        [JsonProperty("run-program-args")]
        public string RunProgramArguments { get; set; }

        [JsonProperty("second-failure")]
        public RecoveryAction SecondFailure { get; set; } = RecoveryAction.None;

        [JsonProperty("service-name")]
        public string ServiceName { get; private set; }

        [JsonProperty("start-type")]
        public ServiceStartMode StartType { get; private set; }

        [JsonProperty("subsequent-failure")]
        public RecoveryAction SubsequentFailure { get; set; } = RecoveryAction.None;
    }
}