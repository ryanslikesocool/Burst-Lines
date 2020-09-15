using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.BurstLines
{
    public abstract class ShapeSO : ScriptableObject
    {
        public abstract void GetProps(out Shape shape);
    }
}