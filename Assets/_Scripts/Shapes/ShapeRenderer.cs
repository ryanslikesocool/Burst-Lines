using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ifelse.Shapes
{
    [ExecuteInEditMode]
    public class ShapeRenderer : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private bool renderInEditMode = true;
#endif
        [SerializeField] private RenderMode renderMode = RenderMode.Immediate;

        [Space] [SerializeField] private MeshFilter retainedModePrefab = null;
        [SerializeField] private Material immediateModeMaterial = null;

        [Space] [SerializeField] private ShapeSO[] shapes = null;

        private Dictionary<ShapeSO, MeshFilter> shapeRendererLink = new Dictionary<ShapeSO, MeshFilter>();

        private void OnRenderObject()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && !renderInEditMode)
            {
                ClearMeshRenderers();
                return;
            }
#endif
            if (shapes == null || shapes.Length == 0
             || (renderMode == RenderMode.Immediate && immediateModeMaterial == null)
             || (renderMode == RenderMode.Retained && retainedModePrefab == null)) { return; }

            JobHandle inputDependencies = new JobHandle();
            foreach (ShapeSO shapeSO in shapes)
            {
                if (shapeSO == null) { continue; }

                Shape shape;
                shapeSO.GetProps(out shape);

                if (!shape.IsDirty) { continue; }

                inputDependencies = shape.PreTransformJobs(inputDependencies);
                inputDependencies = shape.CalculateTransform(inputDependencies);
                inputDependencies = shape.PostTransformJobs(inputDependencies);

                if (shape.RendererType == RendererType.QuadLine)
                {
                    inputDependencies = shape.CalculateQuads(inputDependencies);
                }

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
                    RenderCached();
                    break;
            }
        }

        private void ClearMeshRenderers()
        {
            if (shapeRendererLink.Count > 0)
            {
                foreach (MeshFilter filter in shapeRendererLink.Values)
                {
                    DestroyImmediate(filter.gameObject);
                }
                shapeRendererLink.Clear();
            }
        }

        private void RenderImmediate()
        {
            immediateModeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            foreach (ShapeSO shapeSO in shapes)
            {
                if (shapeSO == null) { continue; }

                Shape shape;
                shapeSO.GetProps(out shape);

                shape.Render();
            }

            GL.PopMatrix();
        }

        private void RenderCached()
        {
            if (retainedModePrefab == null) { return; }

            foreach (ShapeSO shapeSO in shapes)
            {
                if (shapeSO == null) { continue; }

                Shape shape;
                shapeSO.GetProps(out shape);

                shape.Cache();

                if (!shapeRendererLink.ContainsKey(shapeSO))
                {
                    shapeRendererLink.Add(shapeSO, null);
                }

                if (shapeRendererLink[shapeSO] == null)
                {
                    shapeRendererLink[shapeSO] = Instantiate(retainedModePrefab);
                    shapeRendererLink[shapeSO].sharedMesh = shape.Mesh;
                }
            }
        }
    }
}