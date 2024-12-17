using System;

namespace MeridianPrimePrime
{
    internal static class Compatibility
    {
        private const string GeodeShatterFixedVersion = "1.3.6";

        private static readonly Version ApplicationVersion;
        internal static readonly bool GeodeShatterFixed;

        static Compatibility()
        {
            ApplicationVersion = new Version(UnityEngine.Application.version);

            GeodeShatterFixed = ApplicationVersion >= new Version(GeodeShatterFixedVersion);
        }
    }
}
