using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ShapeEditors
{
    public static void TransformEditor(SerializedProperty position, SerializedProperty rotation, SerializedProperty scale)
    {
        EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(position);
        EditorGUILayout.PropertyField(rotation);
        EditorGUILayout.PropertyField(scale);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
    }

    public static void RendererEditor(SerializedProperty color, SerializedProperty rendererType, SerializedProperty billboardMethod, SerializedProperty quadLineAlignment, SerializedProperty quadLineThickness)
    {
        EditorGUILayout.LabelField("Renderer", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(color);
        EditorGUILayout.PropertyField(rendererType);

        switch ((RendererType)rendererType.enumValueIndex)
        {
            case RendererType.QuadLine:
                EditorGUILayout.PropertyField(billboardMethod);
                EditorGUILayout.PropertyField(quadLineAlignment);
                EditorGUILayout.PropertyField(quadLineThickness);
                break;
        }

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
    }

    public static void PolygonShapeEditor(SerializedProperty closeShape, SerializedProperty points)
    {
        EditorGUILayout.LabelField("Polygon", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(closeShape);
        EditorGUILayout.PropertyField(points);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
    }

    public static void ArcShapeEditor(SerializedProperty angleA, SerializedProperty angleB, SerializedProperty radius, SerializedProperty segments)
    {
        EditorGUILayout.LabelField("Arc", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(angleA);
        EditorGUILayout.PropertyField(angleB);
        EditorGUILayout.PropertyField(radius);
        EditorGUILayout.PropertyField(segments);

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
    }

    public static void CenterPolygonEditor(PolygonShape shape)
    {
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Center True"))
        {
            shape.CenterPointsTo(CenterMode.True);
        }
        if (GUILayout.Button("Center Average"))
        {
            shape.CenterPointsTo(CenterMode.Average);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
    }

    public static void DrawHandles(ShapeSO scriptableObject, Shape shape, Tool tool)
    {
        switch (tool)
        {
            case Tool.Move:
                MoveHandle(scriptableObject, shape);
                break;
            case Tool.Rotate:
                RotateHandle(scriptableObject, shape);
                break;
            case Tool.Scale:
                ScaleHandle(scriptableObject, shape);
                break;
            case Tool.Transform:
                TransformHandle(scriptableObject, shape);
                break;
        }
    }

    public static void MoveHandle(ShapeSO scriptableObject, Shape shape)
    {
        Vector3 position = shape.position;

        EditorGUI.BeginChangeCheck();

        position = Handles.PositionHandle(position, shape.Rotation);

        if (EditorGUI.EndChangeCheck())
        {
            shape.position = position;
            Undo.RegisterCompleteObjectUndo(scriptableObject, "Moved shape");
        }
    }

    public static void RotateHandle(ShapeSO scriptableObject, Shape shape)
    {
        Quaternion rotation = shape.Rotation;

        EditorGUI.BeginChangeCheck();

        rotation = Handles.RotationHandle(rotation, shape.position);

        if (EditorGUI.EndChangeCheck())
        {
            shape.Rotation = rotation;
            Undo.RegisterCompleteObjectUndo(scriptableObject, "Rotated shape");
        }
    }

    public static void ScaleHandle(ShapeSO scriptableObject, Shape shape)
    {
        Vector3 scale = shape.scale;
        float magnitude = shape.scale.sqrMagnitude;

        EditorGUI.BeginChangeCheck();

        scale = Handles.ScaleHandle(scale, shape.position, shape.Rotation, magnitude);

        if (EditorGUI.EndChangeCheck())
        {
            shape.scale = scale;
            Undo.RegisterCompleteObjectUndo(scriptableObject, "Scaled shape");
        }
    }

    public static void TransformHandle(ShapeSO scriptableObject, Shape shape)
    {
        Vector3 position = shape.position;
        Quaternion rotation = shape.Rotation;
        Vector3 scale = shape.scale;

        EditorGUI.BeginChangeCheck();

        Handles.TransformHandle(ref position, ref rotation, ref scale);

        if (EditorGUI.EndChangeCheck())
        {
            shape.position = position;
            shape.Rotation = rotation;
            shape.scale = scale;
            Undo.RegisterCompleteObjectUndo(scriptableObject, "Transformed shape");
        }
    }
}