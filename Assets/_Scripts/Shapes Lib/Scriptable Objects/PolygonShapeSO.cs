using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ifelse/Shapes/Polygon Shape")]
public class PolygonShapeSO : ShapeSO
{
    public PolygonShape shape;

    public override void GetProps(out Shape shape)
    {
        shape = this.shape;
    }
}