// Made with <3 by Ryan Boyer http://ryanjboyer.com

using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

namespace BurstLines
{
    [System.Serializable]
    public class PolygonShape : Shape
    {
        #region Properties

        public RendererType rendererType;
        public ColorMode colorMode = ColorMode.Solid;
        public BlendMode blendMode = BlendMode.Mix;
        [ColorUsage(true, true)] public Color color = new Color(0, 0, 0, 255);
        [ColorUsage(true, true)] public Color[] colors = new Color[0];
        [GradientUsage(true)] public Gradient gradient = new Gradient();
        protected Color[] vertexColors;
        public Mesh mesh;
        public bool closeShape = true;
        public float3[] points = null;
        public BillboardMethod billboardMethod;
        public float quadLineThickness;
        public QuadLineAlignment quadLineAlignment;
        public CapType capA;
        public int capDetailA;
        public CapType capB;
        public int capDetailB;

        public NativeArray<float3> pointsToRender;

        #endregion

        #region Execution

        public override void Render()
        {
            if (mesh != null)
            {
                Mesh.DestroyImmediate(mesh);
            }

            switch (rendererType)
            {
                case RendererType.PixelLine:
                    RenderPixelLine();
                    break;
                case RendererType.QuadLine:
                    RenderQuadLine();
                    break;
            }
        }

        public override Mesh Retain()
        {
            switch (rendererType)
            {
                case RendererType.PixelLine:
                    RetainPixelLine();
                    break;
                case RendererType.QuadLine:
                    RetainQuadLine();
                    break;
            }

            return mesh;
        }

        public virtual void RenderPixelLine()
        {
            if ((closeShape && points.Length < 3) || (!closeShape && points.Length < 2)) { return; }

            GL.Begin(GL.LINES);

            for (int i = 0; i < pointsToRender.Length; i++)
            {
                GL.Color(vertexColors[i]);
                GL.Vertex(pointsToRender[i]);
            }

            GL.End();
        }

        public virtual void RenderQuadLine()
        {
            GL.Begin(GL.QUADS);

            for (int i = 0; i < pointsToRender.Length; i++)
            {
                GL.Color(vertexColors[i]);
                GL.Vertex(pointsToRender[i]);
            }

            GL.End();
        }

        public virtual void RetainPixelLine()
        {
            if ((closeShape && points.Length < 3) || (!closeShape && points.Length < 2)) { return; }

            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = $"Shape {this.GetHashCode()} Line";
            }
            mesh.Clear();

            int[] indices = new int[pointsToRender.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            mesh.SetVertices(pointsToRender);
            mesh.SetIndices(indices, MeshTopology.Lines, 0);
            mesh.SetColors(vertexColors);
        }

        public virtual void RetainQuadLine()
        {
            if ((closeShape && points.Length < 3) || (!closeShape && points.Length < 2)) { return; }

            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = $"Shape {this.GetHashCode()} Quad";
            }
            mesh.Clear();

            int[] indices = new int[pointsToRender.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            mesh.SetVertices(pointsToRender);
            mesh.SetIndices(indices, MeshTopology.Quads, 0);
            mesh.SetColors(vertexColors);
        }

        public override void Clear()
        {
            if (pointsToRender.IsCreated) { pointsToRender.Dispose(); }
        }

        #endregion

        #region Calculation

        public override JobHandle CalculateShape(JobHandle inputDependencies)
        {
            Clear();

            float3[] points = this.points;

            inputDependencies = PreTransformJobs(inputDependencies);
            inputDependencies = CalculateTransform(inputDependencies, ref points);
            inputDependencies = PostTransformJobs(inputDependencies);

            inputDependencies = CalculateVertices(inputDependencies, points);

            CalculateColors();

            return inputDependencies;
        }

        public override JobHandle CalculateTransform(JobHandle inputDependencies, ref float3[] points)
        {
            float4x4 transformMatrix = float4x4.TRS(translation, Rotation, scale);
            NativeArray<float3> positionsIn = new NativeArray<float3>(points, Allocator.TempJob);
            CalculateTransformJob calculateTransformJob = new CalculateTransformJob
            {
                TransformMatrix = transformMatrix,
                Positions = positionsIn,
            };
            inputDependencies = calculateTransformJob.Schedule(points.Length, 64, inputDependencies);
            inputDependencies.Complete();

            points = positionsIn.ToArray();
            positionsIn.Dispose();

            return inputDependencies;
        }

        [BurstCompile]
        private struct CalculateTransformJob : IJobParallelFor
        {
            [ReadOnly] public float4x4 TransformMatrix;

            public NativeArray<float3> Positions;

            public void Execute(int index)
            {
                Positions[index] = math.transform(TransformMatrix, Positions[index]);
            }
        }

        public override JobHandle CalculateVertices(JobHandle inputDependencies, float3[] points)
        {
            if ((closeShape && points.Length < 3) || (!closeShape && points.Length < 2)) { return inputDependencies; }

            NativeArray<float3> positionsIn = new NativeArray<float3>(points, Allocator.TempJob);
            int vertexOffset = closeShape ? 0 : 1;

            if (rendererType == RendererType.QuadLine)
            {
                pointsToRender = new NativeArray<float3>((positionsIn.Length - vertexOffset) * 4, Allocator.Persistent);
                CalculateVerticesQuadJob calculateVerticesQuadJob = new CalculateVerticesQuadJob
                {
                    Epsilon = EPSILON,
                    Right3 = new float3(1, 0, 0),
                    QuaterTurn = quaternion.Euler(0, 0, math.PI * 0.5f),
                    CloseShape = closeShape,
                    Thickness = quadLineThickness,
                    BillboardMethod = billboardMethod,
                    LineAlignment = quadLineAlignment,
                    Points = positionsIn,
                    VertexPositions = pointsToRender,
                    CapA = capA,
                    CapB = capB
                };
                inputDependencies = calculateVerticesQuadJob.ScheduleParallel(positionsIn.Length - vertexOffset, 32, inputDependencies);
            }
            else
            {
                pointsToRender = new NativeArray<float3>((points.Length - vertexOffset) * 2, Allocator.Persistent);
                CalculateVerticesLineJob calculateVerticesLineJob = new CalculateVerticesLineJob
                {
                    Points = positionsIn,
                    VertexPositions = pointsToRender
                };
                inputDependencies = calculateVerticesLineJob.ScheduleParallel(positionsIn.Length - vertexOffset, 64, inputDependencies);
            }

            return inputDependencies;
        }

        public void CenterPointsTo(CenterMode mode)
        {
            float3 center = Vector3.zero;
            switch (mode)
            {
                case CenterMode.Bounds:
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
                    foreach (float3 point in points)
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

        public override void CalculateColors()
        {
            if (colors == null || colors.Length == 0) { colors = new Color[] { color }; }

            int pointCount = points.Length;
            int vertexCount = pointsToRender.Length;

            switch (colorMode)
            {
                case ColorMode.Solid:
                    colors = new Color[vertexCount];
                    for (int i = 0; i < vertexCount; i++)
                    {
                        colors[i] = color;
                    }
                    vertexColors = colors;
                    break;
                case ColorMode.PerPoint:
                    if (colors.Length != pointCount)
                    {
                        Color[] pointColors = new Color[pointCount];
                        if (colors.Length < pointCount)
                        {
                            for (int i = 0; i < colors.Length; i++)
                            {
                                pointColors[i] = colors[i];
                            }
                            for (int i = colors.Length; i < pointCount; i++)
                            {
                                pointColors[i] = colors[colors.Length - 1];
                            }
                        }
                        else
                        {
                            for (int i = 0; i < pointCount; i++)
                            {
                                pointColors[i] = colors[i];
                            }
                        }
                        colors = pointColors;
                    }

                    vertexColors = new Color[vertexCount];
                    if (rendererType == RendererType.QuadLine)
                    {
                        for (int i = 0; i < colors.Length; i++)
                        {
                            int vIndex = i * 4;
                            if (vIndex >= vertexColors.Length) { break; }
                            vertexColors[vIndex + 0] = colors[i];
                            vertexColors[vIndex + 1] = colors[i];
                            vertexColors[vIndex + 2] = colors[i];
                            vertexColors[vIndex + 3] = colors[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < colors.Length; i++)
                        {
                            int vIndex = i * 2;
                            if (vIndex >= vertexColors.Length) { break; }
                            vertexColors[vIndex + 0] = colors[i];
                            vertexColors[vIndex + 1] = colors[i];
                        }
                    }
                    break;
                case ColorMode.PerVertex:
                    if (colors.Length != vertexColors.Length)
                    {
                        Color[] localVertexColors = new Color[vertexCount];
                        if (colors.Length < vertexCount)
                        {
                            for (int i = 0; i < colors.Length; i++)
                            {
                                localVertexColors[i] = colors[i];
                            }
                            for (int i = colors.Length; i < vertexCount; i++)
                            {
                                localVertexColors[i] = colors[colors.Length - 1];
                            }
                        }
                        else
                        {
                            for (int i = 0; i < vertexCount; i++)
                            {
                                localVertexColors[i] = colors[i];
                            }
                        }
                        colors = localVertexColors;
                    }

                    vertexColors = colors;
                    break;
                case ColorMode.Gradient:
                    vertexColors = new Color[vertexCount];
                    float step = 1f / points.Length;
                    if (rendererType == RendererType.QuadLine)
                    {
                        for (int i = 0; i < pointCount; i++)
                        {
                            int vIndex = i * 4;
                            Color eval = gradient.Evaluate(i * step);
                            if (vIndex >= vertexColors.Length) { break; }
                            vertexColors[vIndex + 0] = eval;
                            vertexColors[vIndex + 1] = eval;
                            vertexColors[vIndex + 2] = eval;
                            vertexColors[vIndex + 3] = eval;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < colors.Length; i++)
                        {
                            int vIndex = i * 2;
                            Color eval = gradient.Evaluate(i * step);
                            if (vIndex >= vertexColors.Length) { break; }
                            vertexColors[vIndex + 0] = eval;
                            vertexColors[vIndex + 1] = eval;
                        }
                    }
                    break;
            }

            if ((colorMode != ColorMode.Solid)
             && blendMode == BlendMode.Mix)
            {
                int offset = rendererType == RendererType.PixelLine ? 1 : 2;
                Color[] remappedColors = new Color[vertexColors.Length];
                for (int i = 0; i < vertexColors.Length; i++)
                {
                    remappedColors[i] = vertexColors[(i + offset).Wrap(vertexColors.Length)];
                }

                if (!closeShape)
                {
                    if (colorMode == ColorMode.PerPoint)
                    {
                        if (rendererType == RendererType.PixelLine)
                        {
                            remappedColors[remappedColors.Length - 1] = colors[colors.Length - 1];
                        }
                        else
                        {
                            remappedColors[remappedColors.Length - 1] = colors[colors.Length - 1];
                            remappedColors[remappedColors.Length - 2] = colors[colors.Length - 1];
                        }
                    }
                    else if (colorMode == ColorMode.Gradient)
                    {
                        if (rendererType == RendererType.PixelLine)
                        {
                            remappedColors[remappedColors.Length - 1] = gradient.Evaluate(1);
                        }
                        else
                        {
                            Color eval = gradient.Evaluate(1);
                            remappedColors[remappedColors.Length - 1] = eval;
                            remappedColors[remappedColors.Length - 2] = eval;
                        }
                    }
                }

                vertexColors = remappedColors;
            }
        }

        [BurstCompile]
        private struct CalculateVerticesQuadJob : IJobFor
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

            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<float3> Points;
            [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float3> VertexPositions;

            public void Execute(int index)
            {
                switch (BillboardMethod)
                {
                    default:
                        Debug.LogWarning($"This billboard method ({BillboardMethod}) isn't implemented yet.");
                        break;
                    case BillboardMethod.Undefined:
                        CalculateUndefined(index);
                        break;
                }
            }

            private void CalculateUndefined(int index)
            {
                float3 a = Points[(index - 1).Wrap(Points.Length)];
                float3 b = Points[index];
                float3 c = Points[(index + 1).Wrap(Points.Length)];
                float3 d = Points[(index + 2).Wrap(Points.Length)];

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

                //These are badly named
                //They're actually meant to be read as 0, 3 and 1, 2 for the corresponding indices
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

                int quadStart = index * 4;

                VertexPositions[quadStart + 0] = b - directionABC * q03Mult;
                VertexPositions[quadStart + 1] = b + directionABC * q12Mult;
                VertexPositions[quadStart + 2] = c + directionBCD * q12Mult;
                VertexPositions[quadStart + 3] = c - directionBCD * q03Mult;
            }

            private void CalculatePixelLine(int index)
            {
                float3 a = Points[index];
                float3 b = Points[(index + 1).Wrap(Points.Length)];

                int lineStart = index * 2;

                VertexPositions[lineStart] = a;
                VertexPositions[lineStart + 1] = b;
            }
        }

        [BurstCompile]
        private struct CalculateVerticesLineJob : IJobFor
        {
            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<float3> Points;
            [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float3> VertexPositions;

            public void Execute(int index)
            {
                float3 a = Points[index];
                float3 b = Points[(index + 1).Wrap(Points.Length)];

                int lineStart = index * 2;

                VertexPositions[lineStart] = a;
                VertexPositions[lineStart + 1] = b;
            }
        }

        #endregion
    }
}