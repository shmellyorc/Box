using Box.Services.Types;

namespace Box.Utils;

/// <summary>
/// Provides fast random number generation.
/// </summary>
public sealed class Rand : GameService
{
    private const double REAL_UNIT_INT = 1.0 / (int.MaxValue + 1.0);
    private const double REAL_UNIT_UINT = 1.0 / (uint.MaxValue + 1.0);
    private const uint Y = 842502087;
    private const uint Z = 3579807591;
    private const uint W = 273326509;
    private uint _x;
    private uint _y;
    private uint _z;
    private uint _w;

    // Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
    // with bitBufferIdx.
    private uint _bitBuffer;
    private uint _bitMask = 1;
    private uint _byteBuffer;
    private uint _byteMove = 0;

    /// <summary>
    /// Gets the instance of the Rand class for accessing fast random number generation.
    /// </summary>
    public static Rand Instance { get; private set; }


    #region Constructors
    /// <summary>
    /// Initialises a new instance using time dependent seed.
    /// </summary>
    internal Rand()
    {
        Instance ??= this;

        // Initialise using the system tick count.
        Reinitialise(Environment.TickCount);
    }
    #endregion


    #region Public Methods [Reinitialisation]
    /// <summary>
    /// Reinitialises using an int value as a seed.
    /// </summary>
    /// <param name="seed"></param>
    public void Reinitialise(int seed)
    {
        // The only stipulation stated for the xorshift RNG is that at least one of
        // the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
        // resetting of the x seed
        _x = (uint)seed;
        _y = Y;
        _z = Z;
        _w = W;
    }
    #endregion


    #region Public Methods [System.Random functionally equivalent methods]

    /// <summary>
    /// Generates a random int over the range 0 to int.MaxValue-1.
    /// MaxValue is not generated in order to remain functionally equivalent to System.Random.Next().
    /// This does slightly eat into some of the performance gain over System.Random, but not much.
    /// For better performance see:
    /// 
    /// Call NextInt() for an int over the range 0 to int.MaxValue.
    /// 
    /// Call NextUInt() and cast the result to an int to generate an int over the full Int32 value range
    /// including negative values. 
    /// </summary>
    /// <returns></returns>
    public int Integer()
    {
        while (true)
        {
            uint t = _x ^ _x << 11;
            _x = _y;
            _y = _z;
            _z = _w;
            _w = _w ^ _w >> 19 ^ (t ^ t >> 8);

            // Handle the special case where the value int.MaxValue is generated. This is outside of 
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = _w & 0x7FFFFFFF;
            if (rtn != 0x7FFFFFFF) return (int)rtn;
        }
    }


    /// <summary>
    /// Generates a random int over the range 0 to upperBound-1, and not including upperBound.
    /// </summary>
    /// <param name="upperBound"></param>
    /// <returns></returns>
    public int Integer(int upperBound)
    {
        //no check 4 better performance
        //if ( upperBound < 0 )
        //	throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
        uint t = _x ^ _x << 11;
        _x = _y; _y = _z; _z = _w;
        // The explicit int cast before the first multiplication gives better performance.
        // See comments in NextDouble.
        return (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ _w >> 19 ^ (t ^ t >> 8))) * upperBound);
    }


    /// <summary>
    /// Generates a random int over the range lowerBound to upperBound-1, and not including upperBound.
    /// upperBound must be >= lowerBound. lowerBound may be negative.
    /// </summary>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <returns></returns>
    public int Integer(int lowerBound, int upperBound)
    {
        //no check 4 better performance
        //if ( lowerBound > upperBound )
        //	throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
        uint t = _x ^ _x << 11;
        _x = _y; _y = _z; _z = _w;
        // The explicit int cast before the first multiplication gives better performance.
        // See comments in NextDouble.
        int range = upperBound - lowerBound;
        if (range < 0)
        {   // If range is <0 then an overflow has occured and must resort to using long integer arithmetic instead (slower).
            // We also must use all 32 bits of precision, instead of the normal 31, which again is slower.	
            return lowerBound + (int)(REAL_UNIT_UINT * (_w = _w ^ _w >> 19 ^ (t ^ t >> 8)) * ((long)upperBound - lowerBound));
        }
        // 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
        // a little more performance.
        return lowerBound + (int)(REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = (_w ^ _w >> 19) ^ (t ^ t >> 8))) * range);
    }


    /// <summary>
    /// Generates a random double. Values returned are from 0.0 up to but not including 1.0.
    /// </summary>
    /// <returns></returns>
    public double Double()
    {
        uint t = _x ^ _x << 11;
        _x = _y; _y = _z; _z = _w;
        // Here we can gain a 2x speed improvement by generating a value that can be cast to 
        // an int instead of the more easily available uint. If we then explicitly cast to an 
        // int the compiler will then cast the int to a double to perform the multiplication, 
        // this final cast is a lot faster than casting from a uint to a double. The extra cast
        // to an int is very fast (the allocated bits remain the same) and so the overall effect 
        // of the extra cast is a significant performance improvement.
        //
        // Also note that the loss of one bit of precision is equivalent to what occurs within 
        // System.Random.
        return REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ _w >> 19 ^ (t ^ t >> 8)));
    }

    /// <summary>
    /// Generates a random float value between 0 (inclusive) and 1 (exclusive).
    /// </summary>
    /// <returns>A random float value.</returns>
    public float Float() => (float)Double();

    /// <summary>
    /// Generates a random float value within the specified range.
    /// </summary>
    /// <param name="min">The inclusive minimum value of the range.</param>
    /// <param name="max">The exclusive maximum value of the range.</param>
    /// <returns>A random float value within the specified range.</returns>
    public float Float(float min, float max)
    {
        return min + Float() * (max - min);
    }


    /// <summary>
    /// Fills the provided byte array with random bytes.
    /// </summary>
    /// <param name="buffer"></param>
    public unsafe void Bytes(byte[] buffer)
    {
        // Fill up the bulk of the buffer in chunks of 4 bytes at a time.
        uint x = _x, y = _y, z = _z, w = _w;
        int i = 0;
        uint t;
        //unsafe optimization by kasthack
        //2x speedup
        if (buffer.Length > 3)
        {
            fixed (byte* bptr = buffer)
            {
                uint* iptr = (uint*)bptr;
                uint* endptr = iptr + buffer.Length / 4;
                //#if WHILE
                do
                {
                    t = (x ^ (x << 11));
                    x = y; y = z; z = w;
                    w = w ^ w >> 19 ^ (t ^ t >> 8);
                    *iptr = w;
                }
                while (++iptr < endptr);
                i = buffer.Length - buffer.Length % 4;
            }
        }
        // Fill up any remaining bytes in the buffer.
        if (i < buffer.Length)
        {
            // Generate 4 bytes.
            t = (x ^ (x << 11));
            x = y; y = z; z = w;
            w = w ^ w >> 19 ^ (t ^ t >> 8);
            do
            {
                buffer[i] = (byte)(w >>= 8);
            } while (++i < buffer.Length);
        }
        _x = x; _y = y; _z = z; _w = w;
    }
    #endregion


    #region Public Methods [Methods not present on System.Random]
    /// <summary>
    /// Generates a uint. Values returned are over the full range of a uint, 
    /// uint.MinValue to uint.MaxValue, inclusive.
    /// 
    /// This is the fastest method for generating a single random number because the underlying
    /// random number generator algorithm generates 32 random bits that can be cast directly to 
    /// a uint.
    /// </summary>
    /// <returns></returns>
    public uint UInteger()
    {
        uint t = _x ^ _x << 11;
        _x = _y; _y = _z; _z = _w;
        return _w = _w ^ _w >> 19 ^ (t ^ t >> 8);
    }

    // /// <summary>
    // /// Generates a random int over the range 0 to int.MaxValue, inclusive. 
    // /// This method differs from Next() only in that the range is 0 to int.MaxValue
    // /// and not 0 to int.MaxValue-1.
    // /// 
    // /// The slight difference in range means this method is slightly faster than Next()
    // /// but is not functionally equivalent to System.Random.Next().
    // /// </summary>
    // /// <returns></returns>
    // internal int IntegerInclusive()
    // {
    //     uint t = _x ^ _x << 11;
    //     _x = _y; _y = _z; _z = _w;
    //     return (int)(0x7FFFFFFF & (_w = _w ^ _w >> 19 ^ (t ^ t >> 8)));
    // }

    /// <summary>
    /// Generates a single random bit.
    /// This method's performance is improved by generating 32 bits in one operation and storing them
    /// ready for future calls.
    /// </summary>
    /// <returns></returns>
    public bool Boolean()
    {
        if (_bitMask != 1) return (_bitBuffer & (_bitMask >>= 1)) == 0;
        // Generate 32 more bits.
        uint t = (_x ^ (_x << 11));
        _x = _y; _y = _z; _z = _w;
        _bitBuffer = _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));

        // Reset the bitMask that tells us which bit to read next.
        _bitMask = 0x80000000;
        return (_bitBuffer & _bitMask) == 0;
    }

    /// <summary>
    /// Generates a random byte value.
    /// </summary>
    /// <returns>A random byte value.</returns>
    public byte Byte()
    {
        if (_byteMove != 0)
        {
            --_byteMove;
            return (byte)(_byteBuffer >>= 8);
        }
        uint t = _x ^ _x << 11;
        _x = _y; _y = _z; _z = _w;
        _byteBuffer = _w = _w ^ _w >> 19 ^ (t ^ t >> 8);
        _byteMove = 3;
        return (byte)_byteBuffer;
    }

    /// <summary>
    /// Generates a random float value between <paramref name="min"/> and <paramref name="max"/>, inclusive.
    /// </summary>
    /// <param name="min">The inclusive minimum value of the range.</param>
    /// <param name="max">The inclusive maximum value of the range.</param>
    /// <returns>A random float value in the range [min, max], including both ends.</returns>
    public float Range(float min, float max)
        => Float() * ((max - min) + float.Epsilon) + min;

    /// <summary>
    /// Generates a random double value between <paramref name="min"/> and <paramref name="max"/>, inclusive.
    /// </summary>
    /// <param name="min">The inclusive minimum value of the range.</param>
    /// <param name="max">The inclusive maximum value of the range.</param>
    /// <returns>A random double value in the range [min, max], including both ends.</returns>
    public int Range(int min, int max) => Integer(min, max + 1);

    /// <summary>
    /// Generates a random double value within the specified range.
    /// </summary>
    /// <param name="min">The inclusive minimum value of the range.</param>
    /// <param name="max">The exclusive maximum value of the range.</param>
    /// <returns>A random double value within the specified range.</returns>
    public double Range(double min, double max)
        => Double() * ((max - min) + double.Epsilon) + min;

    #endregion
}
