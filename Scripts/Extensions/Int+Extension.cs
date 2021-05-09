// Made with <3 by Ryan Boyer http://ryanjboyer.com

using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace BurstLines
{
    public static partial class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Wrap(this int input, int min, int max)
        {
            if (min == max) { return input; }
            int mn = math.min(min, max);
            int mx = math.max(min, max);
            while (input < mn)
            {
                input += mx - mn;
            }
            while (input >= mx)
            {
                input -= mx - mn;
            }
            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Wrap(this int input, int max) => input.Wrap(0, max);
    }
}