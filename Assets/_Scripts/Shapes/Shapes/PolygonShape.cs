using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

namespace ifelse.Shapes
{
    [System.Serializable]
    public class PolygonShape : Shape
    {
        #region Properties

        public RendererType rendererType;
        public RendererType RendererType
        {
            get { return rendererType; }
            set
            {
                rendererType = value;
                MarkDirty();
            }
        }

        public ColorMode colorMode = ColorMode.Solid;
        public ColorMode ColorMode
        {
            get { return colorMode; }
            set
            {
                colorMode = value;
                MarkDirty();
            }
        }

        public BlendMode blendMode = BlendMode.Mix;
        public BlendMode BlendMode
        {
            get { return blendMode; }
            set
            {
                blendMode = value;
                MarkDirty();
            }
        }

        public Color32 color = new Color32(0, 0, 0, 255);
        public Color32 Color
        {
            get { return color; }
            set
            {
                color = value;
                MarkDirty();
            }
        }

        public Color32[] colors = new Color32[0];
        public Color32[] Colors
        {
            get { return colors; }
            set
            {
                colors = value;
                MarkDirty();
            }
        }

        public Gradient gradient = new Gradient();
        public Gradient Gradient
        {
            get { return gradient; }
            set
            {
                gradient = value;
                MarkDirty();
            }
        }

        protected Color32[] vertexColors;

        public Mesh mesh;
        public Mesh Mesh
        {
            get { return mesh; }
            set
            {
                mesh = value;
                MarkDirty();
            }
        }

        public bool closeShape = true;
        public bool CloseShape
        {
            get { return closeShape; }
            set
            {
                closeShape = value;
                MarkDirty();
            }
        }

        public Vector3[] points = null;
        public Vector3[] Points
        {
            get { return points; }
            set
            {
                points = value;
                MarkDirty();
            }
        }

        public Vector3[] pointsToRender;
        public Vector3[] PointsToRender
        {
            get { return pointsToRender; }
            set
            {
                pointsToRender = value;
                MarkDirty();
            }
        }


        public BillboardMethod billboardMethod;
        public BillboardMethod BillboardMethod
        {
            get { return billboardMethod; }
            set
            {
                billboardMethod = value;
                MarkDirty();
            }
        }

        public float quadLineThickness;
        public float QuadLineThickness
        {
            get { return quadLineThickness; }
            set
            {
                quadLineThickness = value;
                MarkDirty();
            }
        }

        public QuadLineAlignment quadLineAlignment;
        public QuadLineAlignment QuadLineAlignment
        {
            get { return quadLineAlignment; }
            set
            {
                quadLineAlignment = value;
                MarkDirty();
            }
        }

        public CapType capA;
        public CapType CapA
        {
            get { return capA; }
            set
            {
                capA = value;
                MarkDirty();
            }
        }

        public int capDetailA;
        public int CapDetailA
        {
            get { return capDetailA; }
            set
            {
                capDetailA = value;
                MarkDirty();
            }
        }

        public CapType capB;
        public CapType CapB
        {
            get { return capB; }
            set
            {
                capB = value;
                MarkDirty();
            }
        }

        public int capDetailB;
        public int CapDetailB
        {
            get { return capDetailB; }
            set
            {
                capDetailB = value;
                MarkDirty();
            }
        }

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

        public override Mesh Cache()
        {
            switch (rendererType)
            {
                case RendererType.PixelLine:
                    CachePixelLine();
                    break;
                case RendererType.QuadLine:
                    CacheQuadLine();
                    break;
            }

            return Mesh;
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

        public virtual void CachePixelLine()
        {
            if ((closeShape && points.Length < 3) || (!closeShape && points.Length < 2)) { return; }

            if (Mesh == null)
            {
                Mesh = new Mesh();
                Mesh.name = $"Shape {this.GetHashCode()} Line";
            }
            Mesh.Clear();

            int[] indices = new int[pointsToRender.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            Mesh.SetVertices(pointsToRender);
            Mesh.SetIndices(indices, MeshTopology.Lines, 0);
            Mesh.SetColors(vertexColors);
        }

        public virtual void CacheQuadLine()
        {
            if (Mesh == null)
            {
                Mesh = new Mesh();
                Mesh.name = $"Shape {this.GetHashCode()} Quad";
            }
            Mesh.Clear();

            int[] indices = new int[pointsToRender.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            Mesh.SetVertices(pointsToRender);
            Mesh.SetIndices(indices, MeshTopology.Quads, 0);
            Mesh.SetColors(vertexColors);
        }

        #endregion

        #region Calculation

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
                Positions[index] *= Scale;
                Positions[index] = math.rotate(Rotation, Positions[index]);
                Positions[index] += Translation;
            }
        }

        public override JobHandle CalculateVertices(JobHandle inputDependencies)
        {
            if ((closeShape && points.Length < 3) || (!closeShape && points.Length < 2)) { return inputDependencies; }

            int pointCount = points.Length - (Extensions.Approximately(points[0], pointsToRender[points.Length - 1]) ? 1 : 0);

            NativeArray<Vector3> nativePoints = new NativeArray<Vector3>(points, Allocator.TempJob);
            NativeArray<float3> positionsIn = nativePoints.Reinterpret<float3>();

            int vertexCount = positionsIn.Length * (rendererType == RendererType.PixelLine ? 2 : 4);
            NativeArray<float3> vertexPositions = new NativeArray<float3>(vertexCount, Allocator.TempJob);
            CalculateVerticesJob calculateVerticesJob = new CalculateVerticesJob
            {
                RendererType = rendererType,
                Epsilon = EPSILON,
                Right3 = new float3(1, 0, 0),
                QuaterTurn = quaternion.Euler(0, 0, math.PI * 0.5f),
                CloseShape = closeShape,
                Thickness = QuadLineThickness,
                BillboardMethod = BillboardMethod,
                LineAlignment = QuadLineAlignment,
                Points = positionsIn,
                VertexPositions = vertexPositions,
                CapA = CapA,
                CapB = CapB
            };
            inputDependencies = calculateVerticesJob.Schedule(positionsIn.Length, 64, inputDependencies);
            inputDependencies.Complete();

            if (!closeShape)
            {
                if (rendererType == RendererType.QuadLine)
                {
                    Extensions.RemoveQuadAtIndex(ref vertexPositions, pointsToRender.Length - 1);
                }
                else
                {
                    Extensions.RemoveLineAtIndex(ref vertexPositions, pointsToRender.Length - 1);
                }
            }

            pointsToRender = Extensions.ToArray(ref vertexPositions);

            positionsIn.Dispose();
            vertexPositions.Dispose();
            return inputDependencies;
        }

        public void CenterPointsTo(CenterMode mode)
        {
            Vector3 center = Vector3.zero;
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

        public override void CalculateColors()
        {
            if (colors == null || colors.Length == 0) { colors = new Color32[] { Color }; }

            int pointCount = points.Length;
            int vertexCount = pointsToRender.Length;

            switch (colorMode)
            {
                case ColorMode.Solid:
                    colors = new Color32[vertexCount];
                    for (int i = 0; i < vertexCount; i++)
                    {
                        colors[i] = color;
                    }
                    vertexColors = colors;
                    break;
                case ColorMode.PerPoint:
                    if (colors.Length != pointCount)
                    {
                        Color32[] pointColors = new Color32[pointCount];
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

                    vertexColors = new Color32[vertexCount];
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
                        Color32[] localVertexColors = new Color32[vertexCount];
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
                    vertexColors = new Color32[vertexCount];
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
                Color32[] remappedColors = new Color32[vertexColors.Length];
                for (int i = 0; i < vertexColors.Length; i++)
                {
                    remappedColors[i] = vertexColors[(i + offset).Wrap(0, vertexColors.Length, 1)];
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
        private struct CalculateVerticesJob : IJobParallelFor
        {
            [ReadOnly] public RendererType RendererType;
            [ReadOnly] public float Epsilon;
            [ReadOnly] public float3 Right3;
            [ReadOnly] public quaternion QuaterTurn;
            [ReadOnly] public bool CloseShape;
            [ReadOnly] public float Thickness;
            [ReadOnly] public BillboardMethod BillboardMethod;
            [ReadOnly] public QuadLineAlignment LineAlignment;
            [ReadOnly] public CapType CapA;
            [ReadOnly] public CapType CapB;

            [ReadOnly] public NativeArray<float3> Points;

            [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<float3> VertexPositions;

            public void Execute(int index)
            {
                if (RendererType == RendererType.QuadLine)
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
                else
                {
                    CalculatePixelLine(index);
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

                VertexPositions[quadStart + 0] = b - directionABC * q03Mult;
                VertexPositions[quadStart + 1] = b + directionABC * q12Mult;
                VertexPositions[quadStart + 2] = c + directionBCD * q12Mult;
                VertexPositions[quadStart + 3] = c - directionBCD * q03Mult;
            }

            private void CalculatePixelLine(int index)
            {
                float3 a = Points[index];
                float3 b = Points[(index + 1).Wrap(0, Points.Length, 1)];

                int lineStart = index * 2;

                VertexPositions[lineStart] = a;
                VertexPositions[lineStart + 1] = b;
            }
        }

        #endregion
    }
}