using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    }

    public override void OnInspectorGUI()
    {
        ShapeEditors.TransformEditor(position, eulerRotation, scale);
        ShapeEditors.RendererEditor(color, rendererType, billboardMethod, quadLineAlignment, quadLineThickness);
    }

    public abstract void DuringSceneGUI(SceneView sceneView);
}