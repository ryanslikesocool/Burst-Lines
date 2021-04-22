using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BurstLines
{
    public abstract class ShapeSO : ScriptableObject
    {
        public abstract Shape Shape { get; }
    }

    public abstract class ShapeSO<T> : ShapeSO where T : Shape
    {
        public Material material = null;
    }
}