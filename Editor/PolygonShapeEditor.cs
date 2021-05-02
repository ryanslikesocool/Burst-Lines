// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BurstLines.Editors
{
    [CustomEditor(typeof(PolygonShapeRenderer))]
    public class PolygonShapeEditor : Editor
    {
        protected PolygonShapeRenderer shapeRenderer;
        private PolygonShape shapeObj;

        protected SerializedProperty renderMode;
        protected SerializedProperty shape;
        protected SerializedProperty immediateModeMaterial;

        protected SerializedProperty translation;
        protected SerializedProperty rotation;
        protected SerializedProperty scale;

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

        private void OnEnable()
        {
            shapeRenderer = (PolygonShapeRenderer)target;
            shapeObj = shapeRenderer.shape;

            renderMode = serializedObject.FindProperty("renderMode");
            shape = serializedObject.FindProperty("shape");
            immediateModeMaterial = serializedObject.FindProperty("immediateModeMaterial");

            translation = shape.FindPropertyRelative("translation");
            rotation = shape.FindPropertyRelative("rotation");
            scale = shape.FindPropertyRelative("scale");

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

            ShapeEditors.PolygonShapeEditor(closeShape, points, shapeObj);
            ShapeEditors.RendererEditor(colorMode, blendMode, color, colors, gradient, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);
            ShapeEditors.CapEditor(capA, capDetailA, capB, capDetailB, (RendererType)rendererType.enumValueIndex == RendererType.PixelLine);

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