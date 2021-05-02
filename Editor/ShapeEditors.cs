// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Unity.Mathematics;

namespace BurstLines.Editors
{
    public static class ShapeEditors
    {
        public static void TransformEditor(SerializedProperty translation, SerializedProperty rotation, SerializedProperty scale)
        {
            EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(translation);
            EditorGUILayout.PropertyField(rotation);
            EditorGUILayout.PropertyField(scale);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        }

        public static void RendererEditor(SerializedProperty colorMode, SerializedProperty blendMode, SerializedProperty color, SerializedProperty colors, SerializedProperty gradient, SerializedProperty rendererType, SerializedProperty billboardMethod, SerializedProperty quadLineAlignment, SerializedProperty quadLineThickness)
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
                case ColorMode.PerPoint:
                case ColorMode.PerVertex:
                    EditorGUILayout.PropertyField(blendMode);
                    EditorGUILayout.PropertyField(colors);
                    break;
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
        }

        public static void PolygonShapeEditor(SerializedProperty closeShape, SerializedProperty points, PolygonShape shape)
        {
            EditorGUILayout.LabelField("Polygon", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(closeShape);
            EditorGUILayout.PropertyField(points);

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

        public static void DrawHandles(this MonoBehaviour sender, Shape shape, Tool tool)
        {
            switch (tool)
            {
                case Tool.Move:
                    MoveHandle(sender, shape);
                    break;
                case Tool.Rotate:
                    RotateHandle(sender, shape);
                    break;
                case Tool.Scale:
                    ScaleHandle(sender, shape);
                    break;
                case Tool.Transform:
                    TransformHandle(sender, shape);
                    break;
            }
        }

        public static void MoveHandle(this MonoBehaviour sender, Shape shape)
        {
            Vector3 position = shape.translation;

            EditorGUI.BeginChangeCheck();

            position = Handles.PositionHandle(position, shape.Rotation);

            if (EditorGUI.EndChangeCheck())
            {
                shape.translation = position;
                Undo.RegisterCompleteObjectUndo(sender, "Moved shape");
            }
        }

        public static void RotateHandle(this MonoBehaviour sender, Shape shape)
        {
            Quaternion rotation = shape.Rotation;

            EditorGUI.BeginChangeCheck();

            rotation = Handles.RotationHandle(rotation, shape.translation);

            if (EditorGUI.EndChangeCheck())
            {
                shape.Rotation = rotation;
                Undo.RegisterCompleteObjectUndo(sender, "Rotated shape");
            }
        }

        public static void ScaleHandle(this MonoBehaviour sender, Shape shape)
        {
            Vector3 scale = shape.scale;
            float magnitude = math.lengthsq(shape.scale);

            EditorGUI.BeginChangeCheck();

            scale = Handles.ScaleHandle(scale, shape.translation, shape.Rotation, magnitude);

            if (EditorGUI.EndChangeCheck())
            {
                shape.scale = scale;
                Undo.RegisterCompleteObjectUndo(sender, "Scaled shape");
            }
        }

        public static void TransformHandle(this MonoBehaviour sender, Shape shape)
        {
            Vector3 position = shape.translation;
            Quaternion rotation = shape.Rotation;
            Vector3 scale = shape.scale;

            EditorGUI.BeginChangeCheck();

            Handles.TransformHandle(ref position, ref rotation, ref scale);

            if (EditorGUI.EndChangeCheck())
            {
                shape.translation = position;
                shape.Rotation = rotation;
                shape.scale = scale;
                Undo.RegisterCompleteObjectUndo(sender, "Transformed shape");
            }
        }
    }
}
#endif