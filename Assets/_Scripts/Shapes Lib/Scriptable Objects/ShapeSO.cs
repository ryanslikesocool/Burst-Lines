using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeSO : ScriptableObject
{
    public abstract void GetProps(out Shape shape);
}