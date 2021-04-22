// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BurstLines.Editors
{
    //[CustomEditor(typeof(ShapeRenderer))]
    public class ShapeRendererEditor : Editor
    {
        protected ShapeRenderer shapeRenderer;

        protected SerializedProperty shapeType;
        protected SerializedProperty renderMode;
        protected SerializedProperty shape;
        protected SerializedProperty immediateModeMaterial;

        private ShapeEditor shapeEditor = null;

        private void OnEnable()
        {
            shapeRenderer = (ShapeRenderer)target;

            if (shapeRenderer.shape == null)
            {
                shapeRenderer.MarkDirty(shapeRenderer.shapeType, true);
            }

            shapeType = serializedObject.FindProperty("shapeType");
            renderMode = serializedObject.FindProperty("renderMode");
            shape = serializedObject.FindProperty("shape");
            immediateModeMaterial = serializedObject.FindProperty("immediateModeMaterial");

            ChangeShapeEditor();
            shapeEditor.OnEnable(shapeRenderer.shape);
        }

        private void OnDisable()
        {
            shapeEditor.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            ShapeType oldShapeType = shapeRenderer.shapeType;

            EditorGUILayout.PropertyField(shapeType);
            EditorGUILayout.PropertyField(renderMode);
            EditorGUI.indentLevel++;
            if (shapeRenderer.renderMode == RenderMode.Immediate)
            {
                EditorGUILayout.PropertyField(immediateModeMaterial);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            shapeEditor.OnInspectorGUI();

            if (GUILayout.Button("Mark All Dirty"))
            {
                shapeRenderer.shape?.MarkDirty();
            }

            if (EditorGUI.EndChangeCheck())
            {
                shapeRenderer.shape?.MarkDirty();
            }
            serializedObject.ApplyModifiedProperties();

            if (oldShapeType != shapeRenderer.shapeType)
            {
                ChangeShapeEditor();
                shapeRenderer.MarkDirty(oldShapeType);
            }
        }

        private void ChangeShapeEditor()
        {
            switch (shapeRenderer.shapeType)
            {
                case ShapeType.Polygon:
                    shapeEditor = new PolygonShapeEditor(shapeRenderer, shape);
                    break;
                case ShapeType.Arc:
                    shapeEditor = new ArcShapeEditor(shapeRenderer, shape);
                    break;
            }
        }
    }
}
#endif