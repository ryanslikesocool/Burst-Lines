using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ifelse.BurstLines
{
    [ExecuteInEditMode]
    public class ShapeRenderer : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool renderInEditMode = true;
#endif
        public RenderMode renderMode = RenderMode.Immediate;

        public Material immediateModeMaterial = null;
        public MeshFilter retainedModePrefab = null;

        public List<ShapeSO> shapes = null;

        private Dictionary<ShapeSO, MeshFilter> shapeRendererLink = new Dictionary<ShapeSO, MeshFilter>();

        private void OnEnable()
        {
            foreach (ShapeSO shapeSO in shapes)
            {
                if (shapeSO == null) { continue; }
                shapeSO.GetProps(out Shape shape);
                shape.MarkDirty();
            }
        }

        private void OnDisable()
        {
            ClearMeshRenderers();
            foreach (ShapeSO shapeSO in shapes)
            {
                shapeSO.GetProps(out Shape shape);
                shape.Clear();
            }
        }

        private void OnRenderObject()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && !renderInEditMode)
            {
                ClearMeshRenderers();
                return;
            }
#endif
            if (shapes == null || shapes.Count == 0
             || (renderMode == RenderMode.Immediate && immediateModeMaterial == null)
             || (renderMode == RenderMode.Retained && retainedModePrefab == null)) { return; }

            JobHandle inputDependencies = new JobHandle();
            foreach (ShapeSO shapeSO in shapes)
            {
                if (shapeSO == null) { continue; }

                shapeSO.GetProps(out Shape shape);

                if (!shape.IsDirty) { continue; }

                shape.Clear();

                inputDependencies = shape.PreTransformJobs(inputDependencies);
                inputDependencies = shape.CalculateTransform(inputDependencies);
                inputDependencies = shape.PostTransformJobs(inputDependencies);

                inputDependencies = shape.CalculateVertices(inputDependencies);

                shape.CalculateColors();

                shape.ClearDirty();
            }
            inputDependencies.Complete();

            switch (renderMode)
            {
                case RenderMode.Immediate:
                    ClearMeshRenderers();
                    RenderImmediate();
                    break;
                case RenderMode.Retained:
                    RenderRetained();
                    break;
            }
        }

        private void ClearMeshRenderers()
        {
            foreach (MeshFilter filter in shapeRendererLink.Values)
            {
                DestroyImmediate(filter.sharedMesh);
                DestroyImmediate(filter.gameObject);
            }
            shapeRendererLink.Clear();
        }

        private void RenderImmediate()
        {
            immediateModeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            foreach (ShapeSO shapeSO in shapes)
            {
                if (shapeSO == null) { continue; }

                shapeSO.GetProps(out Shape shape);

                shape.Render();
            }

            GL.PopMatrix();
        }

        private void RenderRetained()
        {
            if (retainedModePrefab == null) { return; }

            foreach (ShapeSO shapeSO in shapes)
            {
                if (shapeSO == null) { continue; }

                shapeSO.GetProps(out Shape shape);

                Mesh mesh = shape.Retain();

                if (!shapeRendererLink.ContainsKey(shapeSO))
                {
                    shapeRendererLink.Add(shapeSO, null);
                }

                if (shapeRendererLink[shapeSO] == null)
                {
                    shapeRendererLink[shapeSO] = Instantiate(retainedModePrefab);
                    shapeRendererLink[shapeSO].gameObject.hideFlags = HideFlags.DontSave;
                    shapeRendererLink[shapeSO].sharedMesh = mesh;
                }
            }
        }
    }
}