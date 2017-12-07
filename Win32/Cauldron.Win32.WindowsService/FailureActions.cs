using System;

namespace Cauldron.WindowsService
{
    internal sealed class FailureActions
    {
        public RecoveryAction FirstFailure { get; set; } = RecoveryAction.None;
        public TimeSpan ResetFailCountAfter { get; set; } = TimeSpan.FromDays(1);
        public TimeSpan RestartServiceAfter { get; set; } = TimeSpan.FromMinutes(1);
        public string RunProgram { get; set; }
        public string RunProgramArguments { get; set; }
        public RecoveryAction SecondFailure { get; set; } = RecoveryAction.None;
        public RecoveryAction SubsequentFailure { get; set; } = RecoveryAction.None;
    }
}