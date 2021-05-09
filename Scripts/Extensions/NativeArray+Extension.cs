// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace BurstLines
{
    public static partial class Extensions
    {
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
    }
}