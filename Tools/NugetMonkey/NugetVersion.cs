using Cauldron;
using System;

namespace NugetMonkey
{
    public sealed class NugetVersion
    {
        public NugetVersion(string version)
        {
            var splitted = version.Split('.');

            if (splitted.Length == 3)
            {
                this.Major = splitted[0].ToInteger();
                this.Minor = splitted[1].ToInteger();

                if (splitted[2].IndexOf('-') >= 0)
                {
                    var revisionBeta = splitted[2].Split('-');
                    this.Revision = revisionBeta[0].ToInteger();
                    this.IsBeta = revisionBeta[1].ToBool();
                }
                else
                    this.Revision = splitted[2].ToInteger();
            }
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
            this.IsBeta = IsBeta;
        }

        public bool IsBeta { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }

        public static implicit operator NugetVersion(string value) => new NugetVersion(value);

        public static implicit operator Version(NugetVersion value) => new Version(value.Major, 0, value.Minor, value.Revision);

        public NugetVersion Increment()
        {
            var minor = this.Minor;
            var revision = this.Revision + 1;
            if (revision >= 100)
            {
                minor++;
                revision = 0;
            }

            return new NugetVersion(this.Major, minor, revision, this.IsBeta);
        }

        public override string ToString() =>
            this.IsBeta ?
            $"{this.Major}.{this.Minor}.{this.Revision}-beta" :
            $"{this.Major}.{this.Minor}.{this.Revision}";
    }
}