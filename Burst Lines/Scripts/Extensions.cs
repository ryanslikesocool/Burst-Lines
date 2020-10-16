using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using System.Runtime.CompilerServices;

namespace BurstLines
{
    public static class Extensions
    {
        public const float EPSILON = 0.000001f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleSigned(float3 lhs, float3 rhs, float3 normal)
        {
            return math.degrees(RadiansSigned(lhs, rhs, normal));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadiansSigned(float3 lhs, float3 rhs, float3 normal)
        {
            return math.atan2(
                math.dot(normal, math.cross(lhs, rhs)),
                math.dot(lhs, rhs)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Wrap(this int input, int min, int max, int iterations)
        {
            if (min >= max) { return input; }
            for (int i = 0; i < iterations; i++)
            {
                if (input < min)
                {
                    input += max - min;
                }
                else if (input >= max)
                {
                    input -= max - min;
                }
            }
            return input;
        }

        public static void RemoveQuadAtIndex(ref NativeArray<float3> vertexArray, int quadIndex)
        {
            if (vertexArray.Length % 4 != 0) { return; }

            List<float3> vertices = new List<float3>(vertexArray);
            for (int i = vertexArray.Length - 1; i > -1; i -= 4)
            {
                if ((i - 3) / 4 == quadIndex)
                {
                    vertices.RemoveAt(i - 0);
                    vertices.RemoveAt(i - 1);
                    vertices.RemoveAt(i - 2);
                    vertices.RemoveAt(i - 3);
                    break;
                }
            }

            vertexArray.Dispose();
            vertexArray = new NativeArray<float3>(vertices.ToArray(), Allocator.TempJob);
        }

        public static void RemoveLineAtIndex(ref NativeArray<float3> vertexArray, int lineIndex)
        {
            if (vertexArray.Length % 2 != 0) { return; }

            List<float3> vertices = new List<float3>(vertexArray);
            for (int i = vertexArray.Length - 1; i > -1; i -= 2)
            {
                if ((i - 1) / 2 == lineIndex)
                {
                    vertices.RemoveAt(i - 0);
                    vertices.RemoveAt(i - 1);
                    break;
                }
            }

            vertexArray.Dispose();
            vertexArray = new NativeArray<float3>(vertices.ToArray(), Allocator.TempJob);
        }

        public static float ClampOutOfRange(this float input, float bigNumber, float max)
        {
            if (math.abs(input) >= bigNumber)
            {
                return max;
            }
            return math.abs(input) < bigNumber ? input : 1;
        }

        public static float AddDeadzone(this float input, float deadzone, float replacement = 0)
        {
            deadzone = math.abs(deadzone);
            if (replacement == 0) { replacement = deadzone; }

            if (input > -deadzone && input < deadzone)
            {
                if (input < 0)
                {
                    return -replacement;
                }
                else
                {
                    return replacement;
                }
            }
            return input;
        }

        public static float3 AddDeadzone(this float3 input, float deadzone, float replacement = 0)
        {
            input.x = input.x.AddDeadzone(deadzone);
            input.y = input.y.AddDeadzone(deadzone);
            input.z = input.z.AddDeadzone(deadzone);
            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(float3 a, float3 b)
        {
            if (math.distancesq(a, b) < EPSILON) { return true; }
            return false;
        }
    }
}