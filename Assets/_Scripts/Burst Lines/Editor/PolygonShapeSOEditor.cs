using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace ifelse.BurstLines
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
        private SerializedProperty gradient;
        private SerializedProperty rendererType;
        private SerializedProperty billboardMethod;
        private SerializedProperty quadLineAlignment;
        private SerializedProperty quadLineThickness;

        private SerializedProperty capA;
        private SerializedProperty capDetailA;
        private SerializedProperty capB;
        private SerializedProperty capDetailB;

        private ReorderableList pointsList;
        private ReorderableList colorsList;

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
            gradient = shape.FindPropertyRelative("gradient");
            rendererType = shape.FindPropertyRelative("rendererType");
            billboardMethod = shape.FindPropertyRelative("billboardMethod");
            quadLineAlignment = shape.FindPropertyRelative("quadLineAlignment");
            quadLineThickness = shape.FindPropertyRelative("quadLineThickness");

            capA = shape.FindPropertyRelative("capA");
            capDetailA = shape.FindPropertyRelative("capDetailA");
            capB = shape.FindPropertyRelative("capB");
            capDetailB = shape.FindPropertyRelative("capDetailB");

            pointsList = new ReorderableList(serializedObject, points, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, $"Points ({points.arraySize})"); },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, points.GetArrayElementAtIndex(index));
                    rect.y += 2;
                },
                onReorderCallback = (ReorderableList list) => { polygonShape.MarkDirty(); }
            };
            pointsList.list = polygonShape.points;

            colorsList = new ReorderableList(serializedObject, colors, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, $"Colors ({colors.arraySize})"); },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, colors.GetArrayElementAtIndex(index));
                    rect.y += 2;
                },
                onReorderCallback = (ReorderableList list) => { polygonShape.MarkDirty(); }
            };
            colorsList.list = polygonShape.colors;

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

            ShapeEditors.PolygonShapeEditor(closeShape, pointsList, polygonShape);

            ShapeEditors.RendererEditor(colorMode, blendMode, color, colorsList, gradient, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);

            ShapeEditors.CapEditor(capA, capDetailA, capB, capDetailB, (RendererType)rendererType.enumValueIndex == RendererType.PixelLine);

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