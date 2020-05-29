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
        [SerializeField]
        private bool renderInEditMode = true;
#endif
        [SerializeField]
        private RenderMode renderMode = RenderMode.Immediate;

        [Space]
        [SerializeField]
        private Material vertexColorMaterial = null;
        [SerializeField]
        private ShapeSO[] shapes = null;

        [Space]
        [SerializeField]
        private MeshFilter cachedRenderPrefab = null;
        private Dictionary<ShapeSO, MeshFilter> shapeRendererLink = new Dictionary<ShapeSO, MeshFilter>();

        private void OnRenderObject()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && !renderInEditMode) { return; }
#endif
            if (shapes == null || shapes.Length == 0) { return; }

            JobHandle inputDependencies = new JobHandle();
            foreach (ShapeSO shapeSO in shapes)
            {
                if (shapeSO == null) { continue; }

                Shape shape;
                shapeSO.GetProps(out shape);

                inputDependencies = shape.PreTransformJobs(inputDependencies);
                inputDependencies = shape.CalculateTransform(inputDependencies);
                inputDependencies = shape.PostTransformJobs(inputDependencies);

                if (shape.rendererType == RendererType.QuadLine)
                {
                    inputDependencies = shape.CalculateQuads(inputDependencies);
                }
            }
            inputDependencies.Complete();

            switch (renderMode)
            {
                case RenderMode.Immediate:
                    RenderImmediate();
                    break;
                case RenderMode.Cached:
                    RenderCached();
                    break;
            }
        }

        private void RenderImmediate()
        {
            vertexColorMaterial.SetPass(0);
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

            if (shapeRendererLink.Count > 0)
            {
                foreach (MeshFilter filter in shapeRendererLink.Values)
                {
                    Destroy(filter.gameObject);
                }
                shapeRendererLink.Clear();
            }
        }

        private void RenderCached()
        {
            if (cachedRenderPrefab == null) { return; }

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
                    shapeRendererLink[shapeSO] = Instantiate(cachedRenderPrefab);
                    shapeRendererLink[shapeSO].sharedMesh = shape.mesh;
                }
            }
        }
    }
}