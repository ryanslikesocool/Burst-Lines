// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BurstLines.Editors
{
    public class PolygonShapeEditor : ShapeEditor<PolygonShape>
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

        public PolygonShapeEditor(ShapeRenderer shapeRenderer, SerializedProperty property) : base(shapeRenderer, property) { }

        public override void OnEnable(Shape shapeObj)
        {
            base.OnEnable(shapeObj);

            polygonShape = shapeObj as PolygonShape;

            closeShape = property.FindPropertyRelative("closeShape");
            points = property.FindPropertyRelative("points");

            colorMode = property.FindPropertyRelative("colorMode");
            blendMode = property.FindPropertyRelative("blendMode");
            color = property.FindPropertyRelative("color");
            colors = property.FindPropertyRelative("colors");
            gradient = property.FindPropertyRelative("gradient");
            rendererType = property.FindPropertyRelative("rendererType");
            billboardMethod = property.FindPropertyRelative("billboardMethod");
            quadLineAlignment = property.FindPropertyRelative("quadLineAlignment");
            quadLineThickness = property.FindPropertyRelative("quadLineThickness");

            capA = property.FindPropertyRelative("capA");
            capDetailA = property.FindPropertyRelative("capDetailA");
            capB = property.FindPropertyRelative("capB");
            capDetailB = property.FindPropertyRelative("capDetailB");

            SceneView.duringSceneGui += DuringSceneGUI;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            ShapeEditors.PolygonShapeEditor(closeShape, points, polygonShape);

            ShapeEditors.RendererEditor(colorMode, blendMode, color, colors, gradient, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);

            ShapeEditors.CapEditor(capA, capDetailA, capB, capDetailB, (RendererType)rendererType.enumValueIndex == RendererType.PixelLine);

            if (EditorGUI.EndChangeCheck())
            {
                MarkDirty();
            }
        }

        public override void DuringSceneGUI(SceneView sceneView)
        {
            shapeRenderer.DrawHandles(polygonShape, Tools.current);
        }

        protected override void MarkDirty()
        {
            polygonShape?.MarkDirty();
        }
    }
}
#endif