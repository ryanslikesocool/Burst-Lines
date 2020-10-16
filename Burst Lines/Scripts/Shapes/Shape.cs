using UnityEngine;
using Unity.Jobs;

namespace BurstLines
{
    [System.Serializable]
    public abstract class Shape
    {
        protected const float EPSILON = 0.000001f;

        public bool IsDirty { get; private set; }

        public Vector3 position;
        public Vector3 eulerRotation;
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

        public abstract JobHandle CalculateTransform(JobHandle inputDependencies);
        public abstract JobHandle CalculateVertices(JobHandle inputDependencies);
        public abstract void CalculateColors();

        public virtual JobHandle PreTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostRender(JobHandle inputDependencies) { return inputDependencies; }
    }
}