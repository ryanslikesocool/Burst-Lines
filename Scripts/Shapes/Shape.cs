// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

namespace BurstLines
{
    [Serializable]
    public abstract class Shape
    {
        public bool IsDirty { get; private set; }

        public float3 translation;
        public float3 rotation;
        public quaternion Rotation
        {
            get => quaternion.Euler(rotation);
            set
            {
                rotation = value.toEulerAngles();
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

        public virtual void Clear() { throw new System.NotImplementedException(); }

        public virtual void Render() { throw new System.NotImplementedException(); }
        public virtual Mesh Retain() { throw new System.NotImplementedException(); }

        public virtual JobHandle CalculateShape(JobHandle inputDependencies) { throw new System.NotImplementedException(); }

        public virtual JobHandle CalculateTransform(JobHandle inputDependencies, ref float3[] points) { throw new System.NotImplementedException(); }
        public virtual JobHandle CalculateVertices(JobHandle inputDependencies, float3[] points) { throw new System.NotImplementedException(); }
        public virtual void CalculateColors() { throw new System.NotImplementedException(); }

        public virtual JobHandle PreTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
        public virtual JobHandle PostRender(JobHandle inputDependencies) { return inputDependencies; }
    }
}