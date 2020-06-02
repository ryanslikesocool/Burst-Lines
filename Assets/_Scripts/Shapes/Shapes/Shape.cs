using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

namespace ifelse.Shapes
{
    [System.Serializable]
    public abstract class Shape
    {
        protected const float EPSILON = 0.000001f;

        public bool IsDirty { get; private set; }

        public Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                MarkDirty();
            }
        }

        public Vector3 eulerRotation;
        public Vector3 EulerRotation
        {
            get { return eulerRotation; }
            set
            {
                eulerRotation = value;
                MarkDirty();
            }
        }
        public Quaternion Rotation
        {
            get { return Quaternion.Euler(eulerRotation); }
            set
            {
                eulerRotation = value.eulerAngles;
                MarkDirty();
            }
        }

        public Vector3 scale = Vector3.one;
        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                MarkDirty();
            }
        }

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

        public BlendMode blendMode = BlendMode.Gradient;
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

        public void MarkDirty()
        {
            IsDirty = true;
        }

        public void ClearDirty()
        {
            IsDirty = false;
        }

        public virtual void Render()
        {
            if (mesh != null)
            {
                UnityEngine.GameObject.DestroyImmediate(mesh);
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

        public virtual void Cache()
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
        }

        protected virtual void MatchColorLength(int pointLength, int vertexCount)
        {
            if (colors == null || colors.Length == 0) { colors = new Color32[] { Color }; }

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
                    if (colors.Length != pointLength)
                    {
                        Color32[] pointColors = new Color32[pointLength];
                        if (colors.Length < pointLength)
                        {
                            for (int i = 0; i < colors.Length; i++)
                            {
                                pointColors[i] = colors[i];
                            }
                            for (int i = colors.Length; i < pointLength; i++)
                            {
                                pointColors[i] = colors[colors.Length - 1];
                            }
                        }
                        else
                        {
                            for (int i = 0; i < pointLength; i++)
                            {
                                pointColors[i] = colors[i];
                            }
                        }
                        colors = pointColors;
                    }

                    vertexColors = new Color32[vertexCount];
                    for (int i = 0; i < colors.Length; i++)
                    {
                        int vIndex = i * 4;
                        vertexColors[vIndex + 0] = colors[i];
                        vertexColors[vIndex + 1] = colors[i];
                        vertexColors[vIndex + 2] = colors[i];
                        vertexColors[vIndex + 3] = colors[i];
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
            }

            if ((colorMode == ColorMode.PerVertex || colorMode == ColorMode.PerPoint)
             && blendMode == BlendMode.Gradient)
            {
                int offset = rendererType == RendererType.PixelLine ? 1 : 2;
                Color32[] originalColors = new Color32[vertexColors.Length];
                for (int i = 0; i < vertexColors.Length; i++)
                {
                    originalColors[i] = vertexColors[(i + offset).Wrap(0, vertexColors.Length, 1)];
                }
                vertexColors = originalColors;
            }
        }

        public abstract void RenderPixelLine();
        public abstract void RenderQuadLine();

        public abstract void CachePixelLine();
        public abstract void CacheQuadLine();

        public abstract JobHandle CalculateTransform(JobHandle inputDependencies);
        public abstract JobHandle CalculateQuads(JobHandle inputDependencies);

        public virtual JobHandle PreTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostRender(JobHandle inputDependencies) { return inputDependencies; }
    }
}