// Made with <3 by Ryan Boyer http://ryanjboyer.com

namespace BurstLines
{
    public enum ShapeType
    {
        Polygon,
        Arc
    }

    public enum RendererType
    {
        PixelLine,
        QuadLine
    }

    public enum RenderMode
    {
        Immediate,
        Retained,
        Code
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
        Square,
        Rounded
    }

    public enum CenterMode
    {
        Bounds,
        Average
    }

    public enum ColorMode
    {
        Solid,
        PerPoint,
        PerVertex,
        Gradient
    }

    public enum BlendMode
    {
        Step,
        Mix
    }
}