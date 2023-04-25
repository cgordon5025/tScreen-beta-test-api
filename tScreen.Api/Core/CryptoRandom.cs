using System;
using System.Security.Cryptography;

namespace Core;

// Create cryptographic random numbers
// https://web.archive.org/web/20090304194122/http://msdn.microsoft.com:80/en-us/magazine/cc163367.aspx
public class CryptoRandom : Random, IRandom
{
#pragma warning disable CS0618
    private readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
#pragma warning restore CS0618
    private readonly byte[] _uint32Buffer = new byte[4];

    public CryptoRandom() { }
    public CryptoRandom(int ignoredSeed) { }

    public override int Next()
    {
        _rng.GetBytes(_uint32Buffer);
        return BitConverter.ToInt32(_uint32Buffer, 0) & 0x7FFFFFFF;
    }

    public override int Next(int maxValue)
    {
        if (maxValue < 0)
            throw new ArgumentOutOfRangeException(nameof(maxValue));

        return Next(0, maxValue);
    }

    public override int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue)
            throw new ArgumentOutOfRangeException(nameof(minValue));

        if (minValue == maxValue) return minValue;

        long diff = maxValue - minValue;

        const long max = (1 + (long) uint.MaxValue);
        
        while (true)
        {
            _rng.GetBytes(_uint32Buffer);
            var rand = BitConverter.ToUInt32(_uint32Buffer, 0);
            var remainder = max % diff;
            if (rand < max - remainder)
            {
                return (int)(minValue + (rand % diff));
            }
        }
    }

    public override double NextDouble()
    {
        _rng.GetBytes(_uint32Buffer);
        var rand = BitConverter.ToUInt32(_uint32Buffer, 0);
        return rand / (1.0 + uint.MaxValue);
    }

    public override void NextBytes(byte[] buffer)
    {
        if (buffer == null) 
            throw new ArgumentNullException(nameof(buffer));

        _rng.GetBytes(buffer);
    }
}