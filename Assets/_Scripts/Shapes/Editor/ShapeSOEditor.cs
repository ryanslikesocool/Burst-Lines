using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ifelse.Shapes
{
    public abstract class ShapeSOEditor : Editor
    {
        protected ShapeSO scriptableObject;
        protected Shape shapeObject;

        protected SerializedProperty shape;

        protected SerializedProperty position;
        protected SerializedProperty eulerRotation;
        protected SerializedProperty scale;

        protected SerializedProperty colorMode;
        protected SerializedProperty blendMode;
        protected SerializedProperty color;
        protected SerializedProperty colors;
        protected SerializedProperty rendererType;
        protected SerializedProperty billboardMethod;
        protected SerializedProperty quadLineAlignment;
        protected SerializedProperty quadLineThickness;

        protected SerializedProperty capA;
        protected SerializedProperty capDetailA;
        protected SerializedProperty capB;
        protected SerializedProperty capDetailB;

        public virtual void OnEnable()
        {
            scriptableObject = (ShapeSO)target;

            shape = serializedObject.FindProperty("shape");

            position = shape.FindPropertyRelative("position");
            eulerRotation = shape.FindPropertyRelative("eulerRotation");
            scale = shape.FindPropertyRelative("scale");

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
        }

        public override void OnInspectorGUI()
        {
            ShapeEditors.TransformEditor(position, eulerRotation, scale);
            ShapeEditors.RendererEditor(colorMode, blendMode, color, colors, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);
        }

        public abstract void DuringSceneGUI(SceneView sceneView);
    }
}