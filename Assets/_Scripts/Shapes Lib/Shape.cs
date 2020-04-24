using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

[System.Serializable]
public abstract class Shape
{
    public Vector3 position;
    public Vector3 eulerRotation;
    public Quaternion Rotation
    {
        get { return Quaternion.Euler(eulerRotation); }
        set { eulerRotation = value.eulerAngles; }
    }
    public Vector3 scale = Vector3.one;

    public RendererType rendererType;
    public Color32 color;

    public BillboardMethod billboardMethod;
    public float quadLineThickness;
    public QuadLineAlignment quadLineAlignment;

    public virtual void Render()
    {
        switch (rendererType)
        {
            case RendererType.PixelLine:
                RenderPixelLine();
                break;
            case RendererType.QuadLine:
                RenderQuadLine();
                break;
        }
    }

    public abstract void RenderPixelLine();
    public abstract void RenderQuadLine();

    public abstract JobHandle CalculateTransform(JobHandle inputDependencies);
    public abstract JobHandle CalculateQuads(JobHandle inputDependencies);

    public virtual JobHandle PreTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
    public virtual JobHandle PostTransformJobs(JobHandle inputDependencies) { return inputDependencies; }
    public virtual JobHandle PostRender(JobHandle inputDependencies) { return inputDependencies; }
}