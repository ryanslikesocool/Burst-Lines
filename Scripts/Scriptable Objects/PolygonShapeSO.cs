using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BurstLines
{
    [CreateAssetMenu(menuName = "Burst Lines/Polygon Shape")]
    public class PolygonShapeSO : ShapeSO<PolygonShape>
    {
        public PolygonShape shape;

        public override Shape Shape => this.shape;
    }
}