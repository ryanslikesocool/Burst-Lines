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

        public override void OnEnable()
        {
            base.OnEnable();

            polygonShape = ((PolygonShapeSO)scriptableObject).shape;

            closeShape = shape.FindPropertyRelative("closeShape");
            points = shape.FindPropertyRelative("points");

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

            ShapeEditors.PolygonShapeEditor(closeShape, points);
            ShapeEditors.CenterPolygonEditor(polygonShape);

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