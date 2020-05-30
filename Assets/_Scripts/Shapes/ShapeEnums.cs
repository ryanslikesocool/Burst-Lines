using System;
using UnityEngine;

namespace ifelse.Shapes
{
    public enum RendererType
    {
        PixelLine,
        QuadLine
    }

    public enum RenderMode
    {
        Immediate,
        Retained
    }

    public enum BillboardMethod
    {
        Undefined,
        ZForward,
        FaceCameraPosition,
        FaceCameraPlane
    }

    public enum QuadLineAlignment
    {
        Edge,
        Center
    }

    public enum CapType
    {
        None,
        Point,
        Square,
        Rounded
    }

    public enum CenterMode
    {
        True,
        Average
    }
}