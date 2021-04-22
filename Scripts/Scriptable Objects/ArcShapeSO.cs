using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BurstLines
{
    [CreateAssetMenu(menuName = "Burst Lines/Arc Shape")]
    public class ArcShapeSO : ShapeSO<ArcShape>
    {
        public ArcShape shape;

        public override Shape Shape => this.shape;
    }
}