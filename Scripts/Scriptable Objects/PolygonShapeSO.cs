using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BurstLines
{
    [CreateAssetMenu(menuName = "Burst Lines/Polygon Shape")]
    public class PolygonShapeSO : ShapeSO
    {
        public PolygonShape shape;

        public override void GetProps(out Shape shape)
        {
            shape = this.shape;
        }
    }
}