using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ifelse.Shapes
{
    [CustomEditor(typeof(ArcShapeSO))]
    public class ArcShapeSOEditor : ShapeSOEditor
    {
        private ArcShape arcShape;

        private SerializedProperty angleA;
        private SerializedProperty angleB;
        private SerializedProperty radius;
        private SerializedProperty segments;

        public override void OnEnable()
        {
            base.OnEnable();

            arcShape = ((ArcShapeSO)scriptableObject).shape;

            angleA = shape.FindPropertyRelative("angleA");
            angleB = shape.FindPropertyRelative("angleB");
            radius = shape.FindPropertyRelative("radius");
            segments = shape.FindPropertyRelative("segments");

            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            ShapeEditors.ArcShapeEditor(angleA, angleB, radius, segments);

            serializedObject.ApplyModifiedProperties();
        }

        public override void DuringSceneGUI(SceneView sceneView)
        {
            ShapeEditors.DrawHandles(scriptableObject, arcShape, Tools.current);
        }
    }
}