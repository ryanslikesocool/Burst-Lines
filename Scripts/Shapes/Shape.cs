// Made with <3 by Ryan Boyer http://ryanjboyer.com

using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

namespace BurstLines
{
    [System.Serializable]
    public abstract class Shape
    {
        public bool IsDirty { get; private set; }

        public float3 translation;
        public float3 eulerRotation;
        public quaternion Rotation
        {
            get => Quaternion.Euler(eulerRotation);
            set
            {
                eulerRotation = value.EulerAngles();
                MarkDirty();
            }
        }
        public float3 scale = Vector3.one;

        public void MarkDirty()
        {
            IsDirty = true;
        }

        public void ClearDirty()
        {
            IsDirty = false;
        }

        public abstract void Clear();

        public abstract void Render();
        public abstract Mesh Retain();

        public abstract JobHandle CalculateShape(JobHandle inputDependencies);

        public abstract JobHandle CalculateTransform(JobHandle inputDependencies, ref float3[] points);
        public abstract JobHandle CalculateVertices(JobHandle inputDependencies, float3[] points);
        public abstract void CalculateColors();

        public virtual JobHandle PreTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostRender(JobHandle inputDependencies) { return inputDependencies; }
    }
}