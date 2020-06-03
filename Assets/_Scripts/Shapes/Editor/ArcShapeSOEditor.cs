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

        private SerializedProperty colorMode;
        private SerializedProperty blendMode;
        private SerializedProperty color;
        private SerializedProperty colors;
        private SerializedProperty rendererType;
        private SerializedProperty billboardMethod;
        private SerializedProperty quadLineAlignment;
        private SerializedProperty quadLineThickness;

        public override void OnEnable()
        {
            base.OnEnable();

            arcShape = ((ArcShapeSO)scriptableObject).shape;

            angleA = shape.FindPropertyRelative("angleA");
            angleB = shape.FindPropertyRelative("angleB");
            radius = shape.FindPropertyRelative("radius");
            segments = shape.FindPropertyRelative("segments");

            colorMode = shape.FindPropertyRelative("colorMode");
            blendMode = shape.FindPropertyRelative("blendMode");
            color = shape.FindPropertyRelative("color");
            colors = shape.FindPropertyRelative("colors");
            rendererType = shape.FindPropertyRelative("rendererType");
            billboardMethod = shape.FindPropertyRelative("billboardMethod");
            quadLineAlignment = shape.FindPropertyRelative("quadLineAlignment");
            quadLineThickness = shape.FindPropertyRelative("quadLineThickness");

            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            ShapeEditors.ArcShapeEditor(angleA, angleB, radius, segments);
            ShapeEditors.RendererEditor(colorMode, blendMode, color, colors, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);

            if (EditorGUI.EndChangeCheck())
            {
                arcShape.MarkDirty();
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void DuringSceneGUI(SceneView sceneView)
        {
            ShapeEditors.DrawHandles(scriptableObject, arcShape, Tools.current);
        }
    }
}