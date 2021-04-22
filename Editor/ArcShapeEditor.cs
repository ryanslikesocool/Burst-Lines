// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BurstLines.Editors
{
    public class ArcShapeEditor : ShapeEditor<ArcShape>
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

        public ArcShapeEditor(ShapeRenderer shapeRenderer, SerializedProperty property) : base(shapeRenderer, property) { }

        public override void OnEnable(Shape shapeObj)
        {
            base.OnEnable(shapeObj);

            arcShape = shapeObj as ArcShape;

            angleA = property.FindPropertyRelative("angleA");
            angleB = property.FindPropertyRelative("angleB");
            radius = property.FindPropertyRelative("radius");
            segments = property.FindPropertyRelative("segments");

            colorMode = property.FindPropertyRelative("colorMode");
            blendMode = property.FindPropertyRelative("blendMode");
            color = property.FindPropertyRelative("color");
            colors = property.FindPropertyRelative("colors");
            gradient = property.FindPropertyRelative("gradient");
            rendererType = property.FindPropertyRelative("rendererType");
            billboardMethod = property.FindPropertyRelative("billboardMethod");
            quadLineAlignment = property.FindPropertyRelative("quadLineAlignment");
            quadLineThickness = property.FindPropertyRelative("quadLineThickness");

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

            ShapeEditors.ArcShapeEditor(angleA, angleB, radius, segments);
            ShapeEditors.RendererEditor(colorMode, blendMode, color, colors, gradient, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);

            if (EditorGUI.EndChangeCheck())
            {
                MarkDirty();
            }
        }

        public override void DuringSceneGUI(SceneView sceneView)
        {
            shapeRenderer.DrawHandles(arcShape, Tools.current);
        }

        protected override void MarkDirty()
        {
            arcShape.MarkDirty();
        }
    }
}
#endif