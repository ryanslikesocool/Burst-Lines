// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace BurstLines
{
    public static class Extensions
    {
        public const float EPSILON = 0.00001f;

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
        public static int Wrap(this int input, int min, int max)
        {
            if (min >= max) { return input; }
            while (input < min)
            {
                input += max - min;
            }
            while (input >= max)
            {
                input -= max - min;
            }
            return input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Wrap(this int input, int max)
        {
            return input.Wrap(0, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampOutOfRange(this float input, float bigNumber, float max)
        {
            if (math.abs(input) >= bigNumber)
            {
                return max;
            }
            return math.abs(input) < bigNumber ? input : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 toEulerAngles(this quaternion q)
        {
            float3 angles;

            float sinr_cosp = 2 * (q.value.w * q.value.x + q.value.y * q.value.z);
            float cosr_cosp = 1 - 2 * (q.value.x * q.value.x + q.value.y * q.value.y);
            angles.x = math.atan2(sinr_cosp, cosr_cosp);

            float sinp = 2 * (q.value.w * q.value.y - q.value.z * q.value.x);
            if (math.abs(sinp) >= 1)
            {
                angles.y = math.abs(math.PI * 0.5f) * math.sign(sinp);
            }
            else
            {
                angles.y = math.asin(sinp);
            }

            float siny_cosp = 2 * (q.value.w * q.value.z + q.value.x * q.value.y);
            float cosy_cosp = 1 - 2 * (q.value.y * q.value.y + q.value.z * q.value.z);
            angles.z = math.atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

        public static T ValidateReference<T>(this MonoBehaviour sender, T component) where T : Component
        {
            if (component == null)
            {
                component = sender.GetComponentInChildren<T>();
                if (component == null)
                {
                    component = sender.gameObject.AddComponent<T>();
                }
            }
            return component;
        }

        public static void DestroySafe(UnityEngine.Object @object)
        {
            if (@object != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(@object);
                }
                else
                {
                    GameObject.DestroyImmediate(@object);
                }
            }
        }
    }
}