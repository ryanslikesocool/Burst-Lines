using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace ifelse.Shapes
{
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

        public static void RendererEditor(SerializedProperty colorMode, SerializedProperty blendMode, SerializedProperty color, ReorderableList colors, SerializedProperty gradient, SerializedProperty rendererType, SerializedProperty billboardMethod, SerializedProperty quadLineAlignment, SerializedProperty quadLineThickness)
        {
            EditorGUILayout.LabelField("Renderer", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(rendererType);

            switch ((RendererType)rendererType.enumValueIndex)
            {
                case RendererType.QuadLine:
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(billboardMethod);
                    EditorGUILayout.PropertyField(quadLineAlignment);
                    EditorGUILayout.PropertyField(quadLineThickness);
                    EditorGUI.indentLevel--;
                    break;
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);

            EditorGUILayout.PropertyField(colorMode);
            EditorGUI.indentLevel++;
            switch ((ColorMode)colorMode.enumValueIndex)
            {
                case ColorMode.Solid:
                    EditorGUILayout.PropertyField(color);
                    break;
                case ColorMode.Gradient:
                    EditorGUILayout.PropertyField(blendMode);
                    EditorGUILayout.PropertyField(gradient);
                    break;
                default:
                    EditorGUILayout.PropertyField(blendMode);
                    colors.DoLayoutList();
                    break;
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        }

        public static void PolygonShapeEditor(SerializedProperty closeShape, ReorderableList points, PolygonShape shape)
        {
            EditorGUILayout.LabelField("Polygon", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(closeShape);
            points.DoLayoutList();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Center Bounds"))
            {
                shape.CenterPointsTo(CenterMode.Bounds);
            }
            if (GUILayout.Button("Center Average"))
            {
                shape.CenterPointsTo(CenterMode.Average);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        }

        public static void ArcShapeEditor(SerializedProperty angleA, SerializedProperty angleB, SerializedProperty radius, SerializedProperty segments)
        {
            EditorGUILayout.LabelField("Arc", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(angleA);
            EditorGUILayout.PropertyField(angleB);
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.IntSlider(segments, 3, 128);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        }

        public static void CapEditor(SerializedProperty capA, SerializedProperty capDetailA, SerializedProperty capB, SerializedProperty capDetailB, bool hide)
        {
            if (hide) { return; }

            EditorGUILayout.LabelField("Caps", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(capA);
            if ((CapType)capA.enumValueIndex == CapType.Rounded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.IntSlider(capDetailA, 1, 16);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(capB);
            if ((CapType)capB.enumValueIndex == CapType.Rounded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.IntSlider(capDetailB, 1, 16);
                EditorGUI.indentLevel--;
            }

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
            Vector3 position = shape.Position;

            EditorGUI.BeginChangeCheck();

            position = Handles.PositionHandle(position, shape.Rotation);

            if (EditorGUI.EndChangeCheck())
            {
                shape.Position = position;
                Undo.RegisterCompleteObjectUndo(scriptableObject, "Moved shape");
            }
        }

        public static void RotateHandle(ShapeSO scriptableObject, Shape shape)
        {
            Quaternion rotation = shape.Rotation;

            EditorGUI.BeginChangeCheck();

            rotation = Handles.RotationHandle(rotation, shape.Position);

            if (EditorGUI.EndChangeCheck())
            {
                shape.Rotation = rotation;
                Undo.RegisterCompleteObjectUndo(scriptableObject, "Rotated shape");
            }
        }

        public static void ScaleHandle(ShapeSO scriptableObject, Shape shape)
        {
            Vector3 scale = shape.Scale;
            float magnitude = shape.Scale.sqrMagnitude;

            EditorGUI.BeginChangeCheck();

            scale = Handles.ScaleHandle(scale, shape.Position, shape.Rotation, magnitude);

            if (EditorGUI.EndChangeCheck())
            {
                shape.Scale = scale;
                Undo.RegisterCompleteObjectUndo(scriptableObject, "Scaled shape");
            }
        }

        public static void TransformHandle(ShapeSO scriptableObject, Shape shape)
        {
            Vector3 position = shape.Position;
            Quaternion rotation = shape.Rotation;
            Vector3 scale = shape.Scale;

            EditorGUI.BeginChangeCheck();

            Handles.TransformHandle(ref position, ref rotation, ref scale);

            if (EditorGUI.EndChangeCheck())
            {
                shape.Position = position;
                shape.Rotation = rotation;
                shape.Scale = scale;
                Undo.RegisterCompleteObjectUndo(scriptableObject, "Transformed shape");
            }
        }
    }
}