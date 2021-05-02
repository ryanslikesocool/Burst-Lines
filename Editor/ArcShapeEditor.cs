// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BurstLines.Editors
{
    [CustomEditor(typeof(ArcShapeRenderer))]
    public class ArcShapeEditor : Editor
    {
        protected ArcShapeRenderer shapeRenderer;
        private ArcShape shapeObj;

        protected SerializedProperty renderMode;
        protected SerializedProperty shape;
        protected SerializedProperty immediateModeMaterial;

        protected SerializedProperty translation;
        protected SerializedProperty rotation;
        protected SerializedProperty scale;

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

        private void OnEnable()
        {
            shapeRenderer = (ArcShapeRenderer)target;
            shapeObj = shapeRenderer.shape;

            renderMode = serializedObject.FindProperty("renderMode");
            shape = serializedObject.FindProperty("shape");
            immediateModeMaterial = serializedObject.FindProperty("immediateModeMaterial");

            translation = shape.FindPropertyRelative("translation");
            rotation = shape.FindPropertyRelative("rotation");
            scale = shape.FindPropertyRelative("scale");

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

            SceneView.duringSceneGui += DuringSceneGUI;
            Undo.undoRedoPerformed += MarkDirty;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            Undo.undoRedoPerformed -= MarkDirty;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(renderMode);
            EditorGUI.indentLevel++;
            if (shapeRenderer.renderMode == RenderMode.Immediate)
            {
                EditorGUILayout.PropertyField(immediateModeMaterial);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            ShapeEditors.TransformEditor(translation, rotation, scale);

            ShapeEditors.ArcShapeEditor(angleA, angleB, radius, segments);
            ShapeEditors.RendererEditor(colorMode, blendMode, color, colors, gradient, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);

            if (EditorGUI.EndChangeCheck())
            {
                MarkDirty();
            }

            if (GUILayout.Button("Mark All Dirty"))
            {
                shapeRenderer.shape?.MarkDirty();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DuringSceneGUI(SceneView sceneView)
        {
            shapeRenderer.DrawHandles(shapeObj, Tools.current);
        }

        private void MarkDirty()
        {
            shapeObj.MarkDirty();
        }
    }
}
#endif