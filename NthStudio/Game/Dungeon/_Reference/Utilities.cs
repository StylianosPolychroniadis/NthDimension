using System;
using System.Collections.Generic;

namespace NthStudio.Game.Dungeon
{
    #region Random Generator
    // ORIGNAL COMMENTS
    /* 
       A C-program for MT19937, with initialization improved 2002/1/26.
       Coded by Takuji Nishimura and Makoto Matsumoto.

       Before using, initialize the state by using init_genrand(seed)  
       or init_by_array(init_key, key_length).

       Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
       All rights reserved.                          

       Redistribution and use in source and binary forms, with or without
       modification, are permitted provided that the following conditions
       are met:

         1. Redistributions of source code must retain the above copyright
            notice, this list of conditions and the following disclaimer.

         2. Redistributions in binary form must reproduce the above copyright
            notice, this list of conditions and the following disclaimer in the
            documentation and/or other materials provided with the distribution.

         3. The names of its contributors may not be used to endorse or promote 
            products derived from this software without specific prior written 
            permission.

       THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
       "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
       LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
       A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
       CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
       EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
       PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
       PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
       LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
       NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
       SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


       Any feedback is very welcome.
       http://www.math.keio.ac.jp/matumoto/emt.html
       email: matumoto@math.keio.ac.jp
    */

    /// <summary>
    /// MersenneTwister, Pseudo-random number generator which provides better quality random numbers than the built-in Random class
    /// </summary>
    public class MT19937
    {
        // Period parameters
        private const ulong N = 624;
        private const ulong M = 397;
        private const ulong MATRIX_A = 0x9908B0DFUL;        // constant vector a 
        private const ulong UPPER_MASK = 0x80000000UL;      // most significant w-r bits
        private const ulong LOWER_MASK = 0X7FFFFFFFUL;      // least significant r bits
        private const uint DEFAULT_SEED = 4357;

        private static ulong[] mt = new ulong[N + 1];   // the array for the state vector
        private static ulong mti = N + 1;           // mti==N+1 means mt[N] is not initialized

        public MT19937()
        {
            ulong[] init = new ulong[4];
            init[0] = 0x123;
            init[1] = 0x234;
            init[2] = 0x345;
            init[3] = 0x456;
            init_by_array(init);
        }

        // initializes mt[N] with a seed
        void init_genrand(ulong s)
        {
            mt[0] = s & 0xffffffffUL;
            for (mti = 1; mti < N; mti++)
            {
                mt[mti] = (1812433253UL * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti);
                /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
                /* In the previous versions, MSBs of the seed affect   */
                /* only MSBs of the array mt[].                        */
                /* 2002/01/09 modified by Makoto Matsumoto             */
                mt[mti] &= 0xffffffffUL;
                /* for >32 bit machines */
            }
        }


        // initialize by an array with array-length
        // init_key is the array for initializing keys
        // key_length is its length
        public void init_by_array(ulong[] init_key)
        {
            ulong key_length = (ulong)init_key.Length;
            ulong i, j, k;
            init_genrand(19650218UL);
            i = 1; j = 0;
            k = (N > key_length ? N : key_length);
            for (; k > 0; k--)
            {
                mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1664525UL))
                + init_key[j] + j;      // non linear 
                mt[i] &= 0xffffffffUL;  // for WORDSIZE > 32 machines
                i++; j++;
                if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
                if (j >= key_length) j = 0;
            }
            for (k = N - 1; k > 0; k--)
            {
                mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 30)) * 1566083941UL))
                - i;                    // non linear
                mt[i] &= 0xffffffffUL;  // for WORDSIZE > 32 machines
                i++;
                if (i >= N) { mt[0] = mt[N - 1]; i = 1; }
            }
            mt[0] = 0x80000000UL;       // MSB is 1; assuring non-zero initial array
        }

        // generates a random number on [0,0x7fffffff]-interval
        public long genrand_int31()
        {
            return (long)(genrand_int32() >> 1);
        }
        // generates a random number on [0,1]-real-interval
        public double genrand_real1()
        {
            return (double)genrand_int32() * (1.0 / 4294967295.0); // divided by 2^32-1 
        }
        // generates a random number on [0,1)-real-interval
        public double genrand_real2()
        {
            return (double)genrand_int32() * (1.0 / 4294967296.0); // divided by 2^32
        }
        // generates a random number on (0,1)-real-interval
        public double genrand_real3()
        {
            return (((double)genrand_int32()) + 0.5) * (1.0 / 4294967296.0); // divided by 2^32
        }
        // generates a random number on [0,1) with 53-bit resolution
        public double genrand_res53()
        {
            ulong a = genrand_int32() >> 5;
            ulong b = genrand_int32() >> 6;
            return (double)(a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
        }
        // These real versions are due to Isaku Wada, 2002/01/09 added 

        // generates a random number on [0,0xffffffff]-interval
        public ulong genrand_int32()
        {
            ulong y = 0;
            ulong[] mag01 = new ulong[2];
            mag01[0] = 0x0UL;
            mag01[1] = MATRIX_A;
            /* mag01[x] = x * MATRIX_A  for x=0,1 */

            if (mti >= N)
            {
                // generate N words at one time
                ulong kk;

                if (mti == N + 1)   /* if init_genrand() has not been called, */
                    init_genrand(5489UL); /* a default initial seed is used */

                for (kk = 0; kk < N - M; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1UL];
                }
                for (; kk < N - 1; kk++)
                {
                    y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                    //mt[kk] = mt[kk+(M-N)] ^ (y >> 1) ^ mag01[y & 0x1UL];
                    mt[kk] = mt[kk - 227] ^ (y >> 1) ^ mag01[y & 0x1UL];
                }
                y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1UL];

                mti = 0;
            }

            y = mt[mti++];

            /* Tempering */
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680UL;
            y ^= (y << 15) & 0xefc60000UL;
            y ^= (y >> 18);

            return y;
        }

        public int Next(int lo, int hi)
        {
            return ((int)genrand_int31() % (hi - lo + 1)) + lo;
        }

        public int Next(int maxCount)
        {
            return ((int)genrand_int31()) % maxCount;
        }
    }
    #endregion

    #region Random Normal
    public class RandomNormal
    {
        public static ushort[] RandNormalTable = new ushort[] {
            206,     613,     1022,    1430,    1838,    2245,     2652,     3058,
            3463,    3867,    4271,    4673,    5075,    5475,     5874,     6271,
            6667,    7061,    7454,    7845,    8234,    8621,     9006,     9389,
            9770,    10148,   10524,   10898,   11269,  11638,   12004,   12367,
            12727,   13085,   13440,   13792,   14140,  14486,   14828,   15168,
            15504,   15836,   16166,   16492,   16814,  17133,   17449,   17761,
            18069,   18374,   18675,   18972,   19266,  19556,   19842,   20124,
            20403,   20678,   20949,   21216,   21479,  21738,   21994,   22245,

            22493,   22737,   22977,   23213,   23446,  23674,   23899,   24120,
            24336,   24550,   24759,   24965,   25166,  25365,   25559,   25750,
            25937,   26120,   26300,   26476,   26649,  26818,   26983,   27146,
            27304,   27460,   27612,   27760,   27906,  28048,   28187,   28323,
            28455,   28585,   28711,   28835,   28955,  29073,   29188,   29299,
            29409,   29515,   29619,   29720,   29818,  29914,   30007,   30098,
            30186,   30272,   30356,   30437,   30516,  30593,   30668,   30740,
            30810,   30879,   30945,   31010,   31072,  31133,   31192,   31249,

            31304,   31358,   31410,   31460,   31509,  31556,   31601,   31646,
            31688,   31730,   31770,   31808,   31846,  31882,   31917,   31950,
            31983,   32014,   32044,   32074,   32102,  32129,   32155,   32180,
            32205,   32228,   32251,   32273,   32294,  32314,   32333,   32352,
            32370,   32387,   32404,   32420,   32435,  32450,   32464,   32477,
            32490,   32503,   32515,   32526,   32537,  32548,   32558,   32568,
            32577,   32586,   32595,   32603,   32611,  32618,   32625,   32632,
            32639,   32645,   32651,   32657,   32662,  32667,   32672,   32677,

            32682,   32686,   32690,   32694,   32698,  32702,   32705,   32708,
            32711,   32714,   32717,   32720,   32722,  32725,   32727,   32729,
            32731,   32733,   32735,   32737,   32739,  32740,   32742,   32743,
            32745,   32746,   32747,   32748,   32749,  32750,   32751,   32752,
            32753,   32754,   32755,   32756,   32757,  32757,   32758,   32758,
            32759,   32760,   32760,   32761,   32761,  32761,   32762,   32762,
            32763,   32763,   32763,   32764,   32764,  32764,   32764,   32765,
            32765,   32765,   32765,   32766,   32766,  32766,   32766,   32767,
        };
    }
    #endregion

    #region Perlin Noise
    class Perlin
    {
        int _octaves;
        int _seed;
        int[] _primes;
        double _persistence = 0.65;

        public Perlin()
        {
            _seed = 31337;
            _octaves = 4;
            Init();
        }

        public Perlin(int seed, int octaves)
        {
            _seed = seed;
            _octaves = octaves;
            Init();
        }

        public int Seed
        {
            get { return _seed; }
            set
            {
                if (_seed != value)
                {
                    _seed = value;
                    Init();
                }
            }
        }

        public int Octaves
        {
            get { return _octaves; }
            set
            {
                if (value < 1 || value > 8)
                    throw new ArgumentException("Invalid value for Octaves");
                if (_octaves != value)
                {
                    _octaves = value;
                    Init();
                }
            }
        }

        public double Persistence
        {
            get { return _persistence; }
            set { _persistence = value; }
        }

        /// <summary>
        /// Initialize array of prime numbers so that the noise function is
        /// unique for each octave
        /// </summary>
        void Init()
        {
            Random r = new Random(_seed);
            _primes = new int[3 * _octaves];
            for (int i = 0; i < _primes.Length; i++)
                _primes[i] = GeneratePrime(r);
        }

        double Noise(int x, int y, int octave)
        {
            int oi = octave * 3;
            int n = x + y * 57 + 111;
            n = (n << 13) ^ n;
            return (1.0 - ((n * (n * n * _primes[oi] + _primes[oi + 1]) + _primes[oi + 2]) & 0x7fffffff) / 1073741824.0);
        }

        double CosineInterpolate(double a, double b, double x)
        {
            double ft, f;
            ft = x * Math.PI;
            f = (1.0 - Math.Cos(ft)) * 0.5;
            return a * (1 - f) + b * f;
        }

        double SmoothNoise(double x, double y, int octave)
        {
            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(y);
            double fx = x - ix;
            double fy = y - iy;

            double v1 = Noise(ix, iy, octave);
            double v2 = Noise(ix + 1, iy, octave);
            double v3 = Noise(ix, iy + 1, octave);
            double v4 = Noise(ix + 1, iy + 1, octave);

            double top = CosineInterpolate(v1, v2, fx);
            double bottom = CosineInterpolate(v3, v4, fx);
            return CosineInterpolate(top, bottom, fy);
        }

        public double PerlinNoise(double x, double y)
        {
            double total = 0.0;
            double frequency = 1.0;
            double amp = 1.0;
            double divide = 0.0;
            for (int i = 0; i < _octaves; i++)
            {
                total += SmoothNoise(x * frequency, y * frequency, i) * amp;
                divide += amp;
                amp *= _persistence;
                frequency *= 2.0;
            }
            total /= divide;
            return total;
        }

        public static int GeneratePrime(Random r)
        {
            int candidate;
            do
            {
                candidate = (r.Next(100000) + 5000) * 2 + 1;
            } while (!IsPrime(candidate));
            return candidate;
        }

        static System.Collections.Generic.List<int> PrimeList = new System.Collections.Generic.List<int>();

        static bool IsPrime(int candidate)
        {
            if (PrimeList.Count == 0)
                PrimeList.AddRange(new int[] { 2, 3 });
            int maxTest = ISqrt(candidate);

            for (int i = 0; i < PrimeList.Count; i++)
            {
                int prime = PrimeList[i];
                if (prime > maxTest)
                    return true;
                if (0 == (candidate % prime))
                    return false;
            }

            int searching = PrimeList[PrimeList.Count - 1];
            while (searching <= maxTest)
            {
                searching += 2;
                if (IsPrime(searching))
                {
                    PrimeList.Add(searching);
                    if (0 == (candidate % searching))
                        return false;
                }
            }
            return true;
        }

        public static int ISqrt(int num)
        {
            if (num == 0)
                return 0;
            int n = (num / 2) + 1;
            int n1 = (n + (num / n)) / 2;
            while (n1 < n)
            {
                n = n1;
                n1 = (n + (num / n)) / 2;
            }
            return n;
        }
    }
    #endregion Perlin Noise

    #region PropertyStream
    internal class PropertyStream : IDisposable
    {
        /// <summary>
        /// string.Format for writing a property with a line prefix
        /// </summary>
        public const string PropFormat = "{0,-50}:= {1}";

        System.IO.TextReader _reader;
        bool _readingRecords = false;
        int _lineNumber = 0;

        public PropertyStream(System.IO.TextReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Reads the next property/value pair from the stream.  Returns true
        /// if the read was successful, or false if the end of the file has been reached.
        /// </summary>
        /// <param name="property">Property.  If null then a blank line was processed.</param>
        /// <param name="value">Value.  If null then a blank line was processed.</param>
        /// <returns>Returns false when the end of the file has been reached.</returns>
        public bool ReadNext(out string property, out string value)
        {
            bool gotNext = false;
            property = value = null;

            while (!gotNext)
            {
                // Get the next line from the text stream
                string line = _reader.ReadLine();

                // Are we at the end of the file?
                if (line == null)
                    return false;

                // Track line numbers for better error reporting
                _lineNumber++;

                // Trim the whitespace at the ends for easier parsing
                line = line.Trim();

                // Handle *BEGIN* and *END* stuff
                if (!_readingRecords)
                {
                    if (line.StartsWith("*BEGIN*"))
                        _readingRecords = true;
                    continue;
                }

                if (line.StartsWith("*END*"))
                {
                    _readingRecords = false;
                    continue;
                }

                // Skip comments
                if (line.StartsWith("//") || line.StartsWith("=="))
                    continue;

                // Is this line empty?
                if (line.Length == 0)
                    // Return blanks
                    gotNext = true;
                else
                {
                    // Look for the := character sequence, which is our universal property/value separator
                    int splitLocation = line.IndexOf(":=");
                    if (splitLocation == line.Length - 2)
                    {
                        // Extract the property name part of the line
                        property = line.Substring(0, splitLocation).Trim();

                        // Extract the value from the line
                        value = string.Empty;
                    }
                    else
                    {
                        if (splitLocation <= 0 || splitLocation > line.Length - 2)
                            throw new ApplicationException("Failed to parse settings stream.  Could not parse line " + _lineNumber.ToString());

                        // Extract the property name part of the line
                        property = line.Substring(0, splitLocation).Trim();

                        // Extract the value from the line
                        value = line.Substring(splitLocation + 2).Trim();
                    }

                    // Finish
                    gotNext = true;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the line number of the last ReadNext
        /// </summary>
        public int LineNumber { get { return _lineNumber; } }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader.Dispose();
                _reader = null;
            }
        }

        public void ParseInt(string fileDesc, string property, string value, out int var)
        {
            if (!int.TryParse(value, out var))
                throw new ApplicationException(string.Format("Failed to load {0}.  Invalid {1} on line {2}.", fileDesc, property, LineNumber));
        }

        //public void ParseDice(string fileDesc, string property, string value, out Dice var)
        //{
        //    throw new NotImplementedException("Dice");
        //    //if (!Dice.TryParse(value, out var))
        //    //    throw new ApplicationException(string.Format("Failed to load {0}.  Invalid {1} on line {2}.", fileDesc, property, LineNumber));
        //}
    }
    #endregion PropertyStream

    #region List
    /// <summary>
    /// Utility methods for manipulating lists
    /// </summary>
    static class Lists
    {
        public static void RandomizeOrder<T>(List<T> list)
        {
            // Get a local copy of the random number generator
            MT19937 rand = Globals.Rand;

            int listCount = list.Count;

            // Start randomizing at the beginning of the list
            for (int i = 0; i < listCount - 1; i++)
            {
                // Get the element which currently occupies this position
                T currentElement = list[i];

                // Pick a random element to put into this position
                int choose = rand.Next(listCount - i) + i;

                // Swap the random element into this position
                list[i] = list[choose];
                list[choose] = currentElement;
            }
        }

        /// <summary>
        /// Collects a new list of items matching the specified filter
        /// </summary>
        /// <typeparam name="T">Type of objects to collect</typeparam>
        /// <param name="candidates">Candidate items over which to enumerate</param>
        /// <param name="filter">Returns true if the object should be collected</param>
        public static List<T> Collect<T>(IEnumerable<T> candidates, Predicate<T> filter)
        {
            List<T> results = new List<T>();
            foreach (T candidate in candidates)
                if (filter(candidate))
                    results.Add(candidate);
            return results;
        }

        /// <summary>
        /// Collects all of the items matching the specified filter into the destination
        /// list.  
        /// </summary>
        /// <typeparam name="T">Type of objects to collect</typeparam>
        /// <param name="dest">Destination list to fill</param>
        /// <param name="filter">Returns true if the object should be collected</param>
        /// <returns>Returns the number of items that were collected</returns>
        public static int CollectInto<T>(IList<T> dest, IEnumerable<T> candidates, Predicate<T> filter)
        {
            int count = 0;
            foreach (T candidate in candidates)
                if (filter(candidate))
                {
                    dest[count++] = candidate;
                }
            return count;
        }

        public static int FindIndex<T>(IList<T> list, Predicate<T> match)
        {
            for (int i = 0; i < list.Count; i++)
                if (match(list[i]))
                    return i;
            return -1;
        }

        public static int FindLastIndex<T>(IList<T> list, Predicate<T> match)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (match(list[i]))
                    return i;
            return -1;
        }
    }
    #endregion List

    #region FloodFill
    public delegate bool DoesLocationMatchDelegate(int x, int y);
    public delegate void PaintLocationDelegate(int x, int y);

    internal class FloodFill
    {
        public static void Fill(int x, int y, DoesLocationMatchDelegate doesMatch, PaintLocationDelegate paint)
        {
            Queue<System.Drawing.Point> pointQueue = new Queue<System.Drawing.Point>();
            if (!doesMatch(x, y))
                return;
            pointQueue.Enqueue(new System.Drawing.Point(x, y));

            while (pointQueue.Count > 0)
            {
                System.Drawing.Point p = pointQueue.Dequeue();
                if (doesMatch(p.X, p.Y))
                {
                    int xLeft = p.X;
                    int xRight = p.X;
                    while (doesMatch(xLeft - 1, p.Y))
                        xLeft--;
                    while (doesMatch(xRight + 1, p.Y))
                        xRight++;

                    for (int xCurrent = xLeft; xCurrent <= xRight; xCurrent++)
                    {
                        paint(xCurrent, p.Y);
                        if (doesMatch(xCurrent, p.Y - 1))
                            pointQueue.Enqueue(new System.Drawing.Point(xCurrent, p.Y - 1));
                        if (doesMatch(xCurrent, p.Y + 1))
                            pointQueue.Enqueue(new System.Drawing.Point(xCurrent, p.Y + 1));
                    }
                }
            }
        }

        public static void FillDiag(int x, int y, DoesLocationMatchDelegate doesMatch, PaintLocationDelegate paint)
        {
            Queue<System.Drawing.Point> pointQueue = new Queue<System.Drawing.Point>();
            if (!doesMatch(x, y))
                return;
            pointQueue.Enqueue(new System.Drawing.Point(x, y));

            while (pointQueue.Count > 0)
            {
                System.Drawing.Point p = pointQueue.Dequeue();
                if (doesMatch(p.X, p.Y))
                {
                    int xLeft = p.X;
                    int xRight = p.X;
                    while (doesMatch(xLeft - 1, p.Y))
                        xLeft--;
                    while (doesMatch(xRight + 1, p.Y))
                        xRight++;

                    for (int xCurrent = xLeft; xCurrent <= xRight; xCurrent++)
                    {
                        paint(xCurrent, p.Y);
                        if (doesMatch(xCurrent, p.Y - 1))
                            pointQueue.Enqueue(new System.Drawing.Point(xCurrent, p.Y - 1));
                        if (doesMatch(xCurrent, p.Y + 1))
                            pointQueue.Enqueue(new System.Drawing.Point(xCurrent, p.Y + 1));
                    }
                    if (doesMatch(xLeft - 1, p.Y - 1))
                        pointQueue.Enqueue(new System.Drawing.Point(xLeft - 1, p.Y - 1));
                    if (doesMatch(xLeft - 1, p.Y + 1))
                        pointQueue.Enqueue(new System.Drawing.Point(xLeft - 1, p.Y + 1));
                    if (doesMatch(xRight + 1, p.Y - 1))
                        pointQueue.Enqueue(new System.Drawing.Point(xRight + 1, p.Y - 1));
                    if (doesMatch(xRight + 1, p.Y + 1))
                        pointQueue.Enqueue(new System.Drawing.Point(xRight + 1, p.Y + 1));
                }
            }
        }
    }
    #endregion FloodFill

    #region AStar
    class AStar
    {
        int _sizex, _sizey;
        bool[] _closedGrid;
        NodeInfo[] _openGrid;
        NodeInfo[] _heap;
        int _heapCount = 0;

        int[] _movex = new int[] { 1, 1, 0, -1, -1, -1, 0, 1 };
        int[] _movey = new int[] { 0, -1, -1, -1, 0, 1, 1, 1 };

        public AStar(int sizex, int sizey)
        {
            _sizex = sizex;
            _sizey = sizey;
            _closedGrid = new bool[sizex * sizey];
            _openGrid = new NodeInfo[sizex * sizey];
            _heap = new NodeInfo[sizex * sizey];
            _heapCount = 0;
        }

        /// <summary>
        /// Delegate to get the cost between two nodes.  Returns NaN if movement is impossible
        /// </summary>
        public delegate double GetCostDelegate(System.Drawing.Point p1, System.Drawing.Point p2, System.Drawing.Point prev);

        public double EstimateDistance(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }

        public NodePath<System.Drawing.Point> FindPath(
            System.Drawing.Point start,
            System.Drawing.Point end,
            GetCostDelegate MeasureLink)
        {
            // Clear the closed and open lists
            Array.Clear(_closedGrid, 0, _closedGrid.Length);
            Array.Clear(_openGrid, 0, _openGrid.Length);
            Array.Clear(_heap, 0, _heapCount);
            _heapCount = 0;

            // Get the heuristic value for our starting node and add it to the open list
            NodeInfo info = new NodeInfo(start);
            info.Parent = info;
            info.G = 0.0;
            info.H = EstimateDistance(start, end);
            _openGrid[start.Y * _sizex + start.X] = info;
            HeapAdd(info);

#if DEBUG
           // List<Point> debugList = new List<Point>();
#endif

            // Keep looping.  If the open list ever becomes empty then
            // we've failed to find a path.
            while (_heapCount > 0)
            {
                // Grab the best candidate from the open list
                info = HeapPop();

                // Add this candidate to the closed list
                System.Drawing.Point nodePoint = info.Node;
                _closedGrid[nodePoint.Y * _sizex + nodePoint.X] = true;
                System.Drawing.Point parentNodePoint = System.Drawing.Point.Empty;
                if (info.Parent != null)
                    parentNodePoint = info.Parent.Node;
#if DEBUG
        //        debugList.Add(nodePoint);
#endif

                // If we've reached the destination point then we can stop
                if (nodePoint == end)
                    return info.MakePath();

                // Test all directions from the current node
                for (int i = 0; i < 8; i++)
                {
                    System.Drawing.Point linkNodePoint = new System.Drawing.Point(
                        nodePoint.X + _movex[i],
                        nodePoint.Y + _movey[i]);

                    // Are we off the grid?
                    if (linkNodePoint.X < 0 || linkNodePoint.Y < 0 ||
                        linkNodePoint.X >= _sizex || linkNodePoint.Y >= _sizey)
                        continue;

                    // Measure the link's cost
                    double cost = MeasureLink(nodePoint, linkNodePoint, parentNodePoint);

                    if (double.IsNaN(cost))
                        continue;

                    // Only consider adjacent nodes which aren't already in the closed list
                    int linkOffset = linkNodePoint.Y * _sizex + linkNodePoint.X;
                    if (!_closedGrid[linkOffset])
                    {
                        // If not open yet, add to open list
                        if (_openGrid[linkOffset] == null)
                        {
                            NodeInfo linkInfo = new NodeInfo(linkNodePoint);
                            linkInfo.Parent = info;
                            linkInfo.G = info.G + cost;
                            linkInfo.H = EstimateDistance(nodePoint, linkNodePoint) * 2; // Ugly hack: *2

                            // Add to the open list
                            _openGrid[linkOffset] = linkInfo;
                            HeapAdd(linkInfo);
                        }
                        else
                        {
                            // Node is already in the open list, but we must
                            // compare the pathfinding values to see if
                            // we've found a better route
                            NodeInfo linkInfo = _openGrid[linkOffset];
                            if (info.G + cost < linkInfo.G)
                            {
                                linkInfo.Parent = info;
                                linkInfo.G = info.G + cost;
                                for (int j = 0; j < _heapCount; j++)
                                    if (_heap[j] == linkInfo)
                                    {
                                        HeapBubbleUp(j);
                                        break;
                                    }
                            }
                        }
                    }
                }
            }

            // no path was found
            return null;
        }

        void HeapAdd(NodeInfo info)
        {
            _heap[_heapCount++] = info;

            // Do bubble-up
            HeapBubbleUp(_heapCount - 1);
        }

        void HeapBubbleUp(int i)
        {
            while (i > 0)
            {
                // Get index of heap node's parent
                int j = ((i + 1) >> 1) - 1;

                // Do we need to reorder the heap?
                if (_heap[j].F <= _heap[i].F)
                    break;

                // Swap elements and keep checking
                NodeInfo temp = _heap[j];
                _heap[j] = _heap[i];
                _heap[i] = temp;
                i = j;
            }
        }

        NodeInfo HeapPop()
        {
            NodeInfo top = _heap[0];
            _heapCount--;
            if (_heapCount > 0)
            {
                _heap[0] = _heap[_heapCount];
                _heap[_heapCount] = null;

                int i = 0;
                while ((i << 1) + 1 < _heapCount)
                {
                    int j = (i << 1) + 1;
                    if (j + 1 < _heapCount && _heap[j + 1].F < _heap[j].F)
                        j++;
                    if (_heap[i].F <= _heap[j].F)
                        break;

                    NodeInfo temp = _heap[j];
                    _heap[j] = _heap[i];
                    _heap[i] = temp;
                    i = j;
                }
            }
            return top;
        }

        /// <summary>
        /// Wraps a user-defined node type in order to provide additional
        /// path finding values
        /// </summary>
        /// <typeparam name="TNode2">User-supplied node type</typeparam>
        class NodeInfo
        {
            System.Drawing.Point _node;
            NodeInfo _parent;
            double _g = 0.0, _h = 0.0;

            public NodeInfo(System.Drawing.Point node)
            {
                _node = node;
            }

            /// <summary>
            /// Gets the wrapped node
            /// </summary>
            public System.Drawing.Point Node { get { return _node; } }

            /// <summary>
            /// Gets the NodeInfo for the parent node.  The parent node is
            /// used to backtrack the path to the starting node
            /// </summary>
            public NodeInfo Parent { get { return _parent; } set { _parent = value; } }

            /// <summary>
            /// Gets the F value.  The F value is used by A* to find the
            /// best candidate in the open list, and is defined as the total
            /// cost to reach the current node plus the estimated remaining
            /// cost to reach the destination node
            /// </summary>
            public double F { get { return _g + _h; } }

            /// <summary>
            /// Gets or sets the G value.  The G value is defined by A* as
            /// the total cost of reaching the current node from the start
            /// node.
            /// </summary>
            public double G { get { return _g; } set { _g = value; } }

            /// <summary>
            /// Gets or sets the H value.  The H value (heuristic value)
            /// is defined by A* as the estimated cost of reaching the
            /// destination node from the current node.
            /// </summary>
            public double H { get { return _h; } set { _h = value; } }

            /// <summary>
            /// Makes a path from the start node to the current node
            /// </summary>
            /// <returns>Returns a new NodePath</returns>
            public NodePath<System.Drawing.Point> MakePath()
            {
                // Add the current node
                List<System.Drawing.Point> path = new List<System.Drawing.Point>();
                NodeInfo current = this;
                path.Add(current.Node);

                // We assume that the start node will have itself as
                // its parent, indicating we should stop
                while (current.Parent != current)
                {
                    current = current.Parent;
                    path.Add(current.Node);
                }
                path.Reverse();
                return new NodePath<System.Drawing.Point>(path, this.G);
            }
        }
    }

    /// <summary>
    /// Stores a path from one node to another
    /// </summary>
    /// <typeparam name="TNode">User-supplied node type</typeparam>
    public class NodePath<TNode>
    {
        List<TNode> _path;
        double _totalCost;
        public NodePath(List<TNode> path, double totalCost)
        {
            _path = path;
            _totalCost = totalCost;
        }
        /// <summary>
        /// Gets the list of nodes, from the start node to the end
        /// </summary>
        public List<TNode> Path { get { return _path; } }

        /// <summary>
        /// Gets the total cost of traveling from the start
        /// to the end node
        /// </summary>
        public double TotalCost { get { return _totalCost; } }
    }
    #endregion Astar

}
