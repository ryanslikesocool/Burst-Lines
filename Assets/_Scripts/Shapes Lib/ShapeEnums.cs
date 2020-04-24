using System;
using UnityEngine;

public enum RendererType
{
    PixelLine,
    QuadLine
}

public enum BillboardMethod
{
    Average,
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