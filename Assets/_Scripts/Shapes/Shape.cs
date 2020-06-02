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

        public Color32 color;
        public Color32 Color
        {
            get { return color; }
            set
            {
                color = value;
                MarkDirty();
            }
        }

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