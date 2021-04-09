// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BurstLines.Editors
{
    [CustomEditor(typeof(ShapeRenderer))]
    public class ShapeRendererEditor : Editor
    {
        private ShapeRenderer shapeRenderer;

        protected SerializedProperty renderMode;
        protected SerializedProperty immediateModeMaterial;
        protected SerializedProperty retainedModePrefab;
        protected SerializedProperty shapes;

        protected ReorderableList shapeList;

        public virtual void OnEnable()
        {
            shapeRenderer = (ShapeRenderer)target;

            renderMode = serializedObject.FindProperty("renderMode");
            immediateModeMaterial = serializedObject.FindProperty("immediateModeMaterial");
            retainedModePrefab = serializedObject.FindProperty("retainedModePrefab");
            shapes = serializedObject.FindProperty("shapes");

            shapeList = new ReorderableList(serializedObject, shapes, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, $"Shapes ({shapes.arraySize})"); },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, shapes.GetArrayElementAtIndex(index));
                    rect.y += 2;
                }
            };
            shapeList.list = shapeRenderer.shapes;
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
            else
            {
                EditorGUILayout.PropertyField(retainedModePrefab);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);

            shapeList.DoLayoutList();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            if (GUILayout.Button("Mark All Dirty"))
            {
                foreach (ShapeSO shapeSO in shapeRenderer.shapes)
                {
                    if (shapeSO == null) { continue; }
                    shapeSO.GetProps(out Shape shape);
                    shape.MarkDirty();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (ShapeSO shapeSO in shapeRenderer.shapes)
                {
                    if (shapeSO == null) { continue; }
                    shapeSO.GetProps(out Shape shape);
                    shape.MarkDirty();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif