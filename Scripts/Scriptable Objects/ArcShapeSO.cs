using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BurstLines
{
    [CreateAssetMenu(menuName = "Burst Lines/Arc Shape")]
    public class ArcShapeSO : ShapeSO
    {
        public ArcShape shape;

        public override void GetProps(out Shape shape)
        {
            shape = this.shape;
        }
    }
}