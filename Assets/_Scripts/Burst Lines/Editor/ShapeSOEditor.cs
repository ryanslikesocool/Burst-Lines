using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ifelse.BurstLines
{
    public abstract class ShapeSOEditor : Editor
    {
        protected ShapeSO scriptableObject;
        protected Shape shapeObject;

        protected SerializedProperty shape;

        protected SerializedProperty position;
        protected SerializedProperty eulerRotation;
        protected SerializedProperty scale;

        public virtual void OnEnable()
        {
            scriptableObject = (ShapeSO)target;

            shape = serializedObject.FindProperty("shape");

            position = shape.FindPropertyRelative("position");
            eulerRotation = shape.FindPropertyRelative("eulerRotation");
            scale = shape.FindPropertyRelative("scale");
        }

        public override void OnInspectorGUI()
        {
            ShapeEditors.TransformEditor(position, eulerRotation, scale);
        }

        public abstract void DuringSceneGUI(SceneView sceneView);
    }
}