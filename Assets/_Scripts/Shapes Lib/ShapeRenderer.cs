using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ShapeRenderer : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private bool renderInEditMode = true;
#endif

    [Space]
    [SerializeField]
    private Material vertexColorMaterial = null;
    [SerializeField]
    private ShapeSO[] shapes = null;

    private void OnRenderObject()
    {
#if UNITY_EDITOR
        if(!EditorApplication.isPlaying && !renderInEditMode) { return; }
#endif
        if(shapes == null || shapes.Length == 0) { return; }

        JobHandle inputDependencies = new JobHandle();
        foreach (ShapeSO shapeSO in shapes)
        {
            if(shapeSO == null) { continue; }

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
    }
}