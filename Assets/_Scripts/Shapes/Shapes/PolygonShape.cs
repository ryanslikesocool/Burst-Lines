using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace ifelse.Shapes
{
    [System.Serializable]
    public class PolygonShape : Shape
    {
        public bool closeShape = true;
        public Vector3[] points = null;

        private Vector3[] pointsToRender;

        public override void RenderPixelLine()
        {
            if ((closeShape && pointsToRender.Length < 3) || (!closeShape && pointsToRender.Length < 2)) { return; }

            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);

            for (int i = 0; i < pointsToRender.Length; i++)
            {
                GL.Vertex(pointsToRender[i]);
            }

            if (closeShape) { GL.Vertex(pointsToRender[0]); }

            GL.End();

            pointsToRender = null;
        }

        public override void RenderQuadLine()
        {
            GL.Begin(GL.QUADS);
            GL.Color(color);

            for (int i = 0; i < pointsToRender.Length; i++)
            {
                GL.Vertex(pointsToRender[i]);
            }

            GL.End();

            pointsToRender = null;
        }

        public override void CachePixelLine()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = $"Shape {this.GetHashCode()} Line";
            }
            mesh.Clear();

            int[] indices = new int[points.Length];
            Color32[] colors = new Color32[pointsToRender.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
                colors[i] = color;
            }

            mesh.SetVertices(points);
            mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
            mesh.SetColors(colors);
        }

        public override void CacheQuadLine()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = $"Shape {this.GetHashCode()} Quad";
            }
            mesh.Clear();

            int[] indices = new int[pointsToRender.Length];
            Color32[] colors = new Color32[pointsToRender.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
                colors[i] = color;
            }

            mesh.SetVertices(pointsToRender);
            mesh.SetIndices(indices, MeshTopology.Quads, 0);
            mesh.SetColors(colors);
        }

        public override JobHandle CalculateTransform(JobHandle inputDependencies)
        {
            NativeArray<Vector3> nativePoints = new NativeArray<Vector3>(points, Allocator.TempJob);
            NativeArray<float3> positions = nativePoints.Reinterpret<float3>();

            CalculateTransformJob calculateTransformJob = new CalculateTransformJob
            {
                Translation = position,
                Rotation = Rotation,
                Scale = scale,
                Positions = positions,
            };
            inputDependencies = calculateTransformJob.Schedule(points.Length, 64, inputDependencies);
            inputDependencies.Complete();

            pointsToRender = Extensions.ToArray(ref positions);

            nativePoints.Dispose();

            return inputDependencies;
        }

        [BurstCompile]
        private struct CalculateTransformJob : IJobParallelFor
        {
            [ReadOnly] public float3 Translation;
            [ReadOnly] public quaternion Rotation;
            [ReadOnly] public float3 Scale;

            public NativeArray<float3> Positions;

            public void Execute(int index)
            {
                Positions[index] = math.rotate(Rotation, Positions[index]);
                Positions[index] *= Scale;
                Positions[index] += Translation;
            }
        }

        public override JobHandle CalculateQuads(JobHandle inputDependencies)
        {
            if ((closeShape && pointsToRender.Length < 3) || (!closeShape && pointsToRender.Length < 2)) { return inputDependencies; }

            int pointCount = pointsToRender.Length - (Extensions.Approximately(pointsToRender[0], pointsToRender[pointsToRender.Length - 1]) ? 1 : 0);

            NativeArray<Vector3> nativePoints = new NativeArray<Vector3>(pointsToRender, Allocator.TempJob);
            NativeArray<float3> positionsIn = nativePoints.Reinterpret<float3>();

            NativeArray<float3> quadPositions = new NativeArray<float3>(positionsIn.Length * 4, Allocator.TempJob);
            CalculateQuadsJobv2 calculateQuadsJob = new CalculateQuadsJobv2
            {
                Epsilon = EPSILON,
                Right3 = new float3(1, 0, 0),
                QuaterTurn = quaternion.Euler(0, 0, math.PI * 0.5f),
                CloseShape = closeShape,
                Thickness = quadLineThickness,
                BillboardMethod = billboardMethod,
                LineAlignment = quadLineAlignment,
                Points = positionsIn,
                QuadPositions = quadPositions,
            };
            inputDependencies = calculateQuadsJob.Schedule(positionsIn.Length, 64, inputDependencies);
            inputDependencies.Complete();

            if (!closeShape)
            {
                Extensions.RemoveQuadAtIndex(ref quadPositions, pointsToRender.Length - 1);
            }

            pointsToRender = Extensions.ToArray(ref quadPositions);

            positionsIn.Dispose();
            quadPositions.Dispose();
            return inputDependencies;
        }

        [BurstCompile]
        private struct CalculateQuadsJobv2 : IJobParallelFor
        {
            [ReadOnly] public float Epsilon;
            [ReadOnly] public float3 Right3;
            [ReadOnly] public quaternion QuaterTurn;
            [ReadOnly] public bool CloseShape;
            [ReadOnly] public float Thickness;
            [ReadOnly] public BillboardMethod BillboardMethod;
            [ReadOnly] public QuadLineAlignment LineAlignment;
            [ReadOnly] public CapType CapA;
            [ReadOnly] public CapType CapB;

            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeArray<float3> Points;

            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<float3> QuadPositions;

            public void Execute(int index)
            {
                switch (BillboardMethod)
                {
                    default:
                        Debug.LogWarning("This billboard method isn't implemented yet!");
                        break;
                    case BillboardMethod.Undefined:
                        CalculateUndefined(index);
                        break;
                }
            }

            private void CalculateUndefined(int index)
            {
                float3 a = Points[(index - 1).Wrap(0, Points.Length, 1)];
                float3 b = Points[index];
                float3 c = Points[(index + 1).Wrap(0, Points.Length, 1)];
                float3 d = Points[(index + 2).Wrap(0, Points.Length, 1)];

                float3 abDist = math.normalize(b - a);
                float3 bcDist = math.normalize(c - b);
                float3 cdDist = math.normalize(d - c);

                float3 directionABC = math.normalize(bcDist - abDist);
                float3 directionBCD = math.normalize(cdDist - bcDist);

                float radiansA = Extensions.RadiansSigned(-abDist, bcDist, new float3(0, 0, 1)) * 0.5f;
                float directionMultiplierABC = radiansA != 0 ? Thickness / math.sin(radiansA) : 0;

                float radiansB = Extensions.RadiansSigned(-bcDist, cdDist, new float3(0, 0, 1)) * 0.5f;
                float directionMultiplierBCD = radiansB != 0 ? Thickness / math.sin(radiansB) : 0;

                directionABC *= directionMultiplierABC;
                directionBCD *= directionMultiplierBCD;

                float q03Mult = LineAlignment == QuadLineAlignment.Center ? 0.5f : 1;
                float q12Mult = LineAlignment == QuadLineAlignment.Center ? 0.5f : 0;

                if (!CloseShape)
                {
                    if (index == 0)
                    {
                        directionABC = math.rotate(QuaterTurn, math.normalize(b - c)) * Thickness;
                    }
                    else if (index == Points.Length - 2)
                    {
                        directionBCD = math.rotate(QuaterTurn, math.normalize(b - c)) * Thickness;
                    }
                }

                if (Extensions.Approximately(b, c))
                {
                    if (index == 0)
                    {
                        directionABC = math.rotate(QuaterTurn, math.normalize(b - c)) * Thickness;
                    }
                    else if (index == Points.Length - 1)
                    {
                        directionBCD = math.rotate(QuaterTurn, math.normalize(b - c)) * Thickness;
                    }
                }

                int quadStart = index * 4;

                QuadPositions[quadStart + 0] = b - directionABC * q03Mult;
                QuadPositions[quadStart + 1] = b + directionABC * q12Mult;
                QuadPositions[quadStart + 2] = c + directionBCD * q12Mult;
                QuadPositions[quadStart + 3] = c - directionBCD * q03Mult;
            }
        }

        public void CenterPointsTo(CenterMode mode)
        {
            Vector3 center = Vector3.zero;
            switch (mode)
            {
                case CenterMode.True:
                    Vector3 min = Vector3.one * Mathf.Infinity;
                    Vector3 max = Vector3.one * Mathf.NegativeInfinity;
                    foreach (Vector3 point in points)
                    {
                        min = Vector3.Min(min, point);
                        max = Vector3.Max(max, point);
                    }
                    center = min + (max - min) * 0.5f;
                    break;
                case CenterMode.Average:
                    center = Vector3.zero;
                    foreach (Vector3 point in points)
                    {
                        center += point;
                    }
                    center /= points.Length;
                    break;
            }

            for (int i = 0; i < points.Length; i++)
            {
                points[i] -= center;
            }
        }
    }
}