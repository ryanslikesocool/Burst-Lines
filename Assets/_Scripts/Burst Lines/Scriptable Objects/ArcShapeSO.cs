using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.BurstLines
{
    [CreateAssetMenu(menuName = "ifelse/Shapes/Arc Shape")]
    public class ArcShapeSO : ShapeSO
    {
        public ArcShape shape;

        public override void GetProps(out Shape shape)
        {
            shape = this.shape;
        }
    }
}