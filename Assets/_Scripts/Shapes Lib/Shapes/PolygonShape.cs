using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

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

    public override JobHandle CalculateTransform(JobHandle inputDependencies)
    {
        NativeArray<float3> positions = new NativeArray<float3>(points.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        points.CopyToNativeArray(ref positions);
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

        positions.Dispose();

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
        NativeArray<float3> positionsIn = new NativeArray<float3>(pointCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        pointsToRender.CopyToNativeArray(ref positionsIn);
        NativeArray<float3> quadPositions = new NativeArray<float3>(pointCount * 4, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        CalculateQuadsJob calculateQuadsJob = new CalculateQuadsJob
        {
            Epsilon = EPSILON,
            Right3 = new float3(1, 0, 0),
            CloseShape = closeShape,
            Thickness = quadLineThickness,
            BillboardMethod = billboardMethod,
            LineAlignment = quadLineAlignment,
            PointsToRender = positionsIn,
            QuadPositions = quadPositions,
        };
        inputDependencies = calculateQuadsJob.Schedule(inputDependencies);
        inputDependencies.Complete();

        Extensions.ClearEmptyQuads(ref quadPositions);
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
    private struct CalculateQuadsJob : IJob
    {
        //[ReadOnly] public float3 CameraPosition;
        //[ReadOnly] public quaternion CameraRotation;
        [ReadOnly] public float Epsilon;
        [ReadOnly] public float3 Right3;
        [ReadOnly] public bool CloseShape;
        [ReadOnly] public float Thickness;
        [ReadOnly] public BillboardMethod BillboardMethod;
        [ReadOnly] public QuadLineAlignment LineAlignment;
        [ReadOnly] public NativeArray<float3> PointsToRender;

        public NativeArray<float3> QuadPositions;

        public void Execute()
        {
            switch (BillboardMethod)
            {
                case BillboardMethod.Average:
                    Average();
                    break;
                case BillboardMethod.ZForward:
                    ZForward();
                    break;
                case BillboardMethod.FaceCameraPosition:
                    FaceCamera(false);
                    break;
                case BillboardMethod.FaceCameraPlane:
                    FaceCamera(true);
                    break;
            }
        }

        private void Average()
        {
            for (int i = 0; i < PointsToRender.Length; i++)
            {
                float3 a = PointsToRender[(i - 1).Wrap(0, PointsToRender.Length, 1)];
                float3 b = PointsToRender[i];
                float3 c = PointsToRender[(i + 1).Wrap(0, PointsToRender.Length, 1)];
                float3 d = PointsToRender[(i + 2).Wrap(0, PointsToRender.Length, 1)];

                float3 abDist = math.normalize(b - a);
                float3 bcDist = math.normalize(c - b);
                float3 cdDist = math.normalize(d - c);

                if (!CloseShape)
                {
                    if (i == 0)
                    {
                        abDist = -bcDist;
                    }
                    else if (i == PointsToRender.Length - 1)
                    {
                        cdDist = -bcDist;
                    }
                }

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

                int quadStart = i * 4;

                QuadPositions[quadStart + 0] = b - directionABC * q03Mult;
                QuadPositions[quadStart + 1] = b + directionABC * q12Mult;
                QuadPositions[quadStart + 2] = c + directionBCD * q12Mult;
                QuadPositions[quadStart + 3] = c - directionBCD * q03Mult;
            }

        }

        private void AverageOpen()
        {
            for (int i = 0; i < PointsToRender.Length; i++)
            {

            }
        }

        private void ZForward()
        {
            throw new System.NotImplementedException();
        }

        private void FaceCamera(bool facePlane)
        {
            throw new System.NotImplementedException();
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