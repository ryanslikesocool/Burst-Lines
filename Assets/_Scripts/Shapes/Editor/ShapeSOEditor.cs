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

        protected SerializedProperty color;
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

            color = shape.FindPropertyRelative("color");
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
            ShapeEditors.RendererEditor(color, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);
        }

        public abstract void DuringSceneGUI(SceneView sceneView);
    }
}