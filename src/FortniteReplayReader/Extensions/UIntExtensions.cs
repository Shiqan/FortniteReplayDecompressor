using System;

namespace FortniteReplayReader.Extensions;

public static class UIntExtensions
{
    public static string MillisecondsToTimeStamp(this uint milliseconds)
    {
        var t = TimeSpan.FromMilliseconds(milliseconds);
        return $"{t.Minutes:D2}:{t.Seconds:D2}";
    }

    public static int CentimetersToDistance(this uint traveled) => (int) Math.Ceiling((double) traveled / 100000);
}