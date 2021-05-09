// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace BurstLines
{
    public static partial class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleSigned(float3 lhs, float3 rhs, float3 normal) => math.degrees(RadiansSigned(lhs, rhs, normal));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadiansSigned(float3 lhs, float3 rhs, float3 normal) => math.atan2(math.dot(normal, math.cross(lhs, rhs)), math.dot(lhs, rhs));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 AddDeadzone(this float3 input, float deadzone, float replacement = 0)
        {
            input.x = input.x.AddDeadzone(deadzone);
            input.y = input.y.AddDeadzone(deadzone);
            input.z = input.z.AddDeadzone(deadzone);
            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float3 a, float3 b, float epsilon = EPSILON) => math.distancesq(a, b) < epsilon * epsilon;
    }
}