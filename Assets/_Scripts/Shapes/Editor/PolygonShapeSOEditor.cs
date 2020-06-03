using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ifelse.Shapes
{
    [CustomEditor(typeof(PolygonShapeSO))]
    public class PolygonShapeSOEditor : ShapeSOEditor
    {
        private PolygonShape polygonShape;

        private SerializedProperty closeShape;
        private SerializedProperty points;

        private SerializedProperty colorMode;
        private SerializedProperty blendMode;
        private SerializedProperty color;
        private SerializedProperty colors;
        private SerializedProperty rendererType;
        private SerializedProperty billboardMethod;
        private SerializedProperty quadLineAlignment;
        private SerializedProperty quadLineThickness;

        private SerializedProperty capA;
        private SerializedProperty capDetailA;
        private SerializedProperty capB;
        private SerializedProperty capDetailB;

        public override void OnEnable()
        {
            base.OnEnable();

            polygonShape = ((PolygonShapeSO)scriptableObject).shape;

            closeShape = shape.FindPropertyRelative("closeShape");
            points = shape.FindPropertyRelative("points");

            colorMode = shape.FindPropertyRelative("colorMode");
            blendMode = shape.FindPropertyRelative("blendMode");
            color = shape.FindPropertyRelative("color");
            colors = shape.FindPropertyRelative("colors");
            rendererType = shape.FindPropertyRelative("rendererType");
            billboardMethod = shape.FindPropertyRelative("billboardMethod");
            quadLineAlignment = shape.FindPropertyRelative("quadLineAlignment");
            quadLineThickness = shape.FindPropertyRelative("quadLineThickness");

            capA = shape.FindPropertyRelative("capA");
            capDetailA = shape.FindPropertyRelative("capDetailA");
            capB = shape.FindPropertyRelative("capB");
            capDetailB = shape.FindPropertyRelative("capDetailB");

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

            bool hideCap = (RendererType)rendererType.enumValueIndex == RendererType.PixelLine;
            ShapeEditors.CapEditor(capA, capDetailA, capB, capDetailB, hideCap);

            ShapeEditors.PolygonShapeEditor(closeShape, points, polygonShape);

            ShapeEditors.RendererEditor(colorMode, blendMode, color, colors, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);

            if (EditorGUI.EndChangeCheck())
            {
                polygonShape.MarkDirty();
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void DuringSceneGUI(SceneView sceneView)
        {
            ShapeEditors.DrawHandles(scriptableObject, polygonShape, Tools.current);
        }
    }
}