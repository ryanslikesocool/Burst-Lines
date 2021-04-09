// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BurstLines.Editors
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
        private SerializedProperty gradient;
        private SerializedProperty rendererType;
        private SerializedProperty billboardMethod;
        private SerializedProperty quadLineAlignment;
        private SerializedProperty quadLineThickness;

        private ReorderableList colorsList;

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
            gradient = shape.FindPropertyRelative("gradient");
            rendererType = shape.FindPropertyRelative("rendererType");
            billboardMethod = shape.FindPropertyRelative("billboardMethod");
            quadLineAlignment = shape.FindPropertyRelative("quadLineAlignment");
            quadLineThickness = shape.FindPropertyRelative("quadLineThickness");

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
                onReorderCallback = (ReorderableList list) => { arcShape.MarkDirty(); }
            };
            colorsList.list = arcShape.colors;

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
            ShapeEditors.RendererEditor(colorMode, blendMode, color, colorsList, gradient, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);

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
#endif