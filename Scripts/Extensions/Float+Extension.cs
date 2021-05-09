// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace BurstLines
{
    public static partial class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AddDeadzone(this float input, float deadzone, float replacement = 0)
        {
            deadzone = math.abs(deadzone);
            if (replacement == 0) { replacement = deadzone; }

            if (input > -deadzone && input < deadzone)
            {
                return math.select(replacement, -replacement, input < 0);
            }
            return input;
        }

    }
}