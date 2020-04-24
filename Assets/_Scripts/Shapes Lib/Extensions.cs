using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public static class Extensions
{
    public static void CopyToNativeArray(this Vector3[] input, ref NativeArray<float3> result)
    {
        for (int i = 0; i < result.Length; i++)
        {
            if (i >= input.Length) { return; }
            result[i] = input[i];
        }
    }

    public static Vector3[] ToArray(ref NativeArray<float3> input)
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
        if (quadArray.Length % 4 != 0) { return; }

        List<float3> quads = new List<float3>(quadArray);
        for (int i = quadArray.Length - 1; i > -1; i -= 4)
        {
            if (math.all(quadArray[i] == quadArray[i - 1])
             && math.all(quadArray[i] == quadArray[i - 2])
             && math.all(quadArray[i] == quadArray[i - 3]))
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
}