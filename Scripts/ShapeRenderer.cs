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
    public class ShapeRenderer : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool renderInEditMode = true;
#endif
        public ShapeType shapeType = ShapeType.Polygon;
        public RenderMode renderMode = RenderMode.Immediate;

        public Shape shape = new PolygonShape();

        private new MeshRenderer renderer = null;
        private MeshFilter filter = null;

        private Mesh mesh;
        public Mesh Mesh
        {
            get => mesh;
            set
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

        private void OnRenderObject()
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

        private void DestroyMesh()
        {
            if (filter != null)
            {
                Extensions.DestroySafe(filter.mesh);
                Extensions.DestroySafe(filter);
            }
            if (renderer != null)
            {
                Extensions.DestroySafe(renderer);
            }
        }

        private void RenderImmediate()
        {
            immediateModeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            shape?.Render();

            GL.PopMatrix();
        }

        private void RenderRetained()
        {
            if (shape == null) { return; }

            renderer = this.ValidateReference(renderer);
            filter = this.ValidateReference(filter);
            filter.hideFlags = HideFlags.DontSave;

            Mesh = shape.Retain();
        }

        private void RenderCodeAccess()
        {
            if (shape == null) { return; }

            Mesh = shape.Retain();
        }

        public void MarkDirty(ShapeType oldShapeType, bool force = false)
        {
            if (oldShapeType != shapeType || force)
            {
                switch (shapeType)
                {
                    case ShapeType.Polygon:
                        shape = new PolygonShape();
                        break;
                    case ShapeType.Arc:
                        shape = new ArcShape();
                        break;
                }
            }
        }
    }
}