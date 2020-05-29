using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

namespace ifelse.Shapes
{
    [System.Serializable]
    public class ArcShape : PolygonShape
    {
        public float angleA = -180;
        public float angleB = 180;
        public float radius = 1;
        public int segments = 32;

        public override JobHandle PreTransformJobs(JobHandle inputDependencies)
        {
            closeShape = math.abs(angleA - angleB) % 360 == 0;

            NativeArray<float3> positions = new NativeArray<float3>(segments + 1, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            CalculateSegmentsJob calculateSegmentsJob = new CalculateSegmentsJob
            {
                Right3 = Vector3.right,
                AngleA = angleA,
                AngleB = angleB,
                Radius = radius,
                Segments = segments,
                Positions = positions
            };
            inputDependencies = calculateSegmentsJob.Schedule(positions.Length, 64, inputDependencies);
            inputDependencies.Complete();

            points = Extensions.ToArray(ref positions);

            positions.Dispose();

            return base.PreTransformJobs(inputDependencies);
        }

        [BurstCompile]
        private struct CalculateSegmentsJob : IJobParallelFor
        {
            [ReadOnly] public float3 Right3;
            [ReadOnly] public float AngleA;
            [ReadOnly] public float AngleB;
            [ReadOnly] public float Radius;
            [ReadOnly] public int Segments;

            public NativeArray<float3> Positions;

            public void Execute(int index)
            {
                float deltaAngle = AngleB - AngleA;
                float step = deltaAngle / Segments;
                float angle = math.radians(AngleA + step * index);
                quaternion rotation = quaternion.EulerXYZ(0, 0, angle);

                Positions[index] = math.rotate(rotation, Right3) * Radius;
            }
        }
    }
}