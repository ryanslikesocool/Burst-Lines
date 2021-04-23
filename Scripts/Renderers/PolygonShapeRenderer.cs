// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BurstLines
{
    [ExecuteInEditMode]
    public class PolygonShapeRenderer : ShapeRenderer<PolygonShape> { }
}