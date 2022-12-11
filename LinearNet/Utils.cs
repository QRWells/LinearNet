using System.Numerics;

namespace LinearNet;

public static class Utils
{
    public static T Sqrt<T>(this T value) where T : struct, INumber<T>
    {
        return (T)Convert.ChangeType(Math.Sqrt(Convert.ToDouble(value)), typeof(T));
    }
}