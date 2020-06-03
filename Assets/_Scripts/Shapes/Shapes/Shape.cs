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

        public void MarkDirty()
        {
            IsDirty = true;
        }

        public void ClearDirty()
        {
            IsDirty = false;
        }

        public abstract void Render();
        public abstract Mesh Cache();

        public abstract JobHandle CalculateTransform(JobHandle inputDependencies);
        public abstract JobHandle CalculateVertices(JobHandle inputDependencies);
        public abstract void CalculateColors();

        public virtual JobHandle PreTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostRender(JobHandle inputDependencies) { return inputDependencies; }
    }
}