using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

namespace ifelse.Shapes
{
    [System.Serializable]
    public abstract class Shape
    {
        protected const float EPSILON = 0.0001f;

        public Vector3 position;
        public Vector3 eulerRotation;
        public Quaternion Rotation
        {
            get { return Quaternion.Euler(eulerRotation); }
            set { eulerRotation = value.eulerAngles; }
        }
        public Vector3 scale = Vector3.one;

        public RendererType rendererType;
        public Color32 color;

        public Mesh mesh;

        public BillboardMethod billboardMethod;
        public float quadLineThickness;
        public QuadLineAlignment quadLineAlignment;

        public virtual void Render()
        {
            if (mesh != null)
            {
                UnityEngine.GameObject.Destroy(mesh);
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