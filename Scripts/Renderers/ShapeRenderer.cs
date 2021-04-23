// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BurstLines
{
    [ExecuteInEditMode]
    public abstract class ShapeRenderer<T> : MonoBehaviour where T : Shape, new()
    {
#if UNITY_EDITOR
        public bool renderInEditMode = true;
#endif
        public ShapeType shapeType = ShapeType.Polygon;
        public RenderMode renderMode = RenderMode.Immediate;

        public T shape = new T();

        private new MeshRenderer renderer = null;
        private MeshFilter filter = null;

        protected Mesh mesh;
        public Mesh Mesh
        {
            get => mesh;
            protected set
            {
                mesh = value;
                if (filter != null)
                {
                    filter.sharedMesh = mesh;
                }
            }
        }

        public Material immediateModeMaterial = null;

        private void OnEnable()
        {
            shape?.MarkDirty();
        }

        private void OnDisable()
        {
            DestroyMesh();
            shape?.Clear();
        }

        protected virtual void OnRenderObject()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && !renderInEditMode)
            {
                DestroyMesh();
                return;
            }
#endif
            if (shape == null
             || (renderMode == RenderMode.Immediate && immediateModeMaterial == null)) { return; }

            JobHandle inputDependencies = new JobHandle();

            if (!shape.IsDirty) { return; }
            shape.Clear();
            inputDependencies = shape.CalculateShape(inputDependencies);
            shape.ClearDirty();

            inputDependencies.Complete();

            switch (renderMode)
            {
                case RenderMode.Immediate:
                    DestroyMesh();
                    RenderImmediate();
                    break;
                case RenderMode.Retained:
                    RenderRetained();
                    break;
                case RenderMode.Code:
                    RenderCodeAccess();
                    break;
            }
        }

        protected virtual void DestroyMesh()
        {
            Extensions.DestroySafe(filter.sharedMesh);
        }

        protected virtual void RenderImmediate()
        {
            immediateModeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            shape?.Render();

            GL.PopMatrix();
        }

        protected virtual void RenderRetained()
        {
            if (shape == null) { return; }

            renderer = this.ValidateReference(renderer);
            filter = this.ValidateReference(filter);
            filter.hideFlags = HideFlags.DontSave;

            Mesh = shape.Retain();
        }

        protected virtual void RenderCodeAccess()
        {
            if (shape == null) { return; }

            Mesh = shape.Retain();
        }
    }
}