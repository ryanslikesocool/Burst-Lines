using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

namespace ifelse.BurstLines
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
            float deltaAngle = angleB - angleA;
            closeShape = math.abs(deltaAngle) % 360 == 0;

            NativeArray<float3> positions = new NativeArray<float3>(segments + (closeShape ? 0 : 1), Allocator.TempJob);
            CalculateSegmentsJob calculateSegmentsJob = new CalculateSegmentsJob
            {
                Right3 = Vector3.right,
                AngleA = angleA,
                DeltaAngle = deltaAngle,
                Step = deltaAngle / segments,
                Radius = radius,
                Segments = segments,
                Positions = positions
            };
            inputDependencies = calculateSegmentsJob.Schedule(positions.Length, 64, inputDependencies);
            inputDependencies.Complete();

            if (positions.Length != points.Length)
            {
                points = new float3[positions.Length];
            }

            points = positions.ToArray();
            positions.Dispose();

            return base.PreTransformJobs(inputDependencies);
        }

        [BurstCompile]
        private struct CalculateSegmentsJob : IJobParallelFor
        {
            [ReadOnly] public float3 Right3;
            [ReadOnly] public float AngleA;
            [ReadOnly] public float DeltaAngle;
            [ReadOnly] public float Step;
            [ReadOnly] public float Radius;
            [ReadOnly] public int Segments;

            [WriteOnly] public NativeArray<float3> Positions;

            public void Execute(int index)
            {
                float angle = math.radians(AngleA + Step * index);
                quaternion rotation = quaternion.EulerXYZ(0, 0, angle);

                Positions[index] = math.rotate(rotation, Right3) * Radius;
            }
        }
    }
}