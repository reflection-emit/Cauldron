using System;

namespace NugetMonkey
{
    public sealed class NugetVersion
    {
        public NugetVersion(string version)
        {
            var result = version.SplitVersion();
            this.Major = result.Item1;
            this.Minor = result.Item2;
            this.Revision = result.Item3;
            this.IsBeta = result.Item4;
        }

        public NugetVersion(int major, int minor, int revision) : this(major, minor, revision, false)
        {
        }

        public NugetVersion()
        {
            this.Major = 1;
        }

        public NugetVersion(int major, int minor, int revision, bool isBeta)
        {
            this.Major = major;
            this.Minor = minor;
            this.Revision = revision;
            this.IsBeta = this.IsBeta;
        }

        public bool IsBeta { get; set; }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Revision { get; set; }

        public static implicit operator NugetVersion(string value) => new NugetVersion(value);

        public static implicit operator Version(NugetVersion value) => new Version(value.Major, 0, value.Minor, value.Revision);

        public NugetVersion Increment()
        {
            var defaultVersion =  NugetMonkeyJson.BasicVersion.SplitVersion();

            var major = this.Major > defaultVersion.Item1? this.Major : defaultVersion.Item1;
            var minor = this.Minor > defaultVersion.Item2? this.Minor : defaultVersion.Item2;
            var revision = defaultVersion.Item3 < 0 ? 0 : this.Revision + 1;
            var isBeta = defaultVersion.Item4;

            if (revision >= 100)
            {
                minor++;
                revision = 0;
            }

            return new NugetVersion(major, minor, revision, isBeta);
        }

        public override string ToString() =>
            this.IsBeta ?
            $"{this.Major}.{this.Minor}.{this.Revision}-beta" :
            $"{this.Major}.{this.Minor}.{this.Revision}";
    }
}