using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace ifelse.Shapes
{
    public static class Extensions
    {
        public const float EPSILON = 0.0001f;

        public static Vector3[] ToArray(ref NativeArray<float3> input)
        {
            Vector3[] result = new Vector3[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = input[i];
            }
            return result;
        }

        public static Vector3[] ToArray(ref NativeList<float3> input)
        {
            Vector3[] result = new Vector3[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = input[i];
            }
            return result;
        }

        public static float AngleSigned(float3 lhs, float3 rhs, float3 normal)
        {
            return math.degrees(RadiansSigned(lhs, rhs, normal));
        }

        /// <summary>
        /// Determine the signed radians between two vectors, with normal 'n'
        /// as the rotation axis.
        /// </summary>
        public static float RadiansSigned(float3 lhs, float3 rhs, float3 normal)
        {
            return math.atan2(
                math.dot(normal, math.cross(lhs, rhs)),
                math.dot(lhs, rhs)
            );
        }

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

        public static void ClearEmptyQuads(ref NativeArray<float3> quadArray)
        {
            if (quadArray.Length % 4 != 0)
            {
                Debug.LogWarning("Quad array is not divisible by 4!  Aborting");
                return;
            }

            NativeList<float3> quads = new NativeList<float3>(quadArray.Length, Allocator.Temp);
            for (int i = quadArray.Length - 1; i > -1; i -= 4)
            {
                if (!Approximately(quadArray[i], quadArray[i - 1])
                 && !Approximately(quadArray[i], quadArray[i - 2])
                 && !Approximately(quadArray[i], quadArray[i - 3])
                 && !Approximately(quadArray[i - 1], quadArray[i - 2])
                 && !Approximately(quadArray[i - 1], quadArray[i - 3])
                 && !Approximately(quadArray[i - 2], quadArray[i - 3]))
                {
                    quads.Add(quadArray[i]);
                    quads.Add(quadArray[i - 1]);
                    quads.Add(quadArray[i - 2]);
                    quads.Add(quadArray[i - 3]);
                }
            }

            quadArray.Dispose();
            quadArray = new NativeArray<float3>(quads, Allocator.TempJob);
            quads.Dispose();
        }

        public static void RemoveQuadAtIndex(ref NativeArray<float3> quadArray, int quadIndex)
        {
            if (quadArray.Length % 4 != 0) { return; }

            List<float3> quads = new List<float3>(quadArray);
            for (int i = quadArray.Length - 1; i > -1; i -= 4)
            {
                if ((i - 3) / 4 == quadIndex)
                {
                    quads.RemoveAt(i);
                    quads.RemoveAt(i - 1);
                    quads.RemoveAt(i - 2);
                    quads.RemoveAt(i - 3);
                }
            }

            quadArray.Dispose();
            quadArray = new NativeArray<float3>(quads.ToArray(), Allocator.TempJob);
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

        public static bool Approximately(float3 a, float3 b)
        {
            if (math.distancesq(a, b) < EPSILON) { return true; }
            return false;
        }
    }
}