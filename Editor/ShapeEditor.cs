// Made with <3 by Ryan Boyer http://ryanjboyer.com

#if UNITY_EDITOR
using UnityEditor;

namespace BurstLines.Editors
{
    public abstract class ShapeEditor
    {
        protected ShapeRenderer shapeRenderer { get; }
        protected SerializedProperty property { get; }

        protected Shape shapeObject;

        protected SerializedProperty translation;
        protected SerializedProperty eulerRotation;
        protected SerializedProperty scale;

        public ShapeEditor(ShapeRenderer shapeRenderer, SerializedProperty property)
        {
            this.shapeRenderer = shapeRenderer;
            this.property = property;
        }

        public virtual void OnEnable(Shape shapeObj)
        {
            shapeObject = shapeObj;

            translation = property.FindPropertyRelative("translation");
            eulerRotation = property.FindPropertyRelative("rotation");
            scale = property.FindPropertyRelative("scale");

            Undo.undoRedoPerformed += MarkDirty;
        }

        public virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= MarkDirty;
        }

        public virtual void OnInspectorGUI()
        {
            ShapeEditors.TransformEditor(translation, eulerRotation, scale);
        }

        protected abstract void MarkDirty();

        public abstract void DuringSceneGUI(SceneView sceneView);
    }

    public abstract class ShapeEditor<T> : ShapeEditor where T : Shape
    {
        public ShapeEditor(ShapeRenderer shapeRenderer, SerializedProperty property) : base(shapeRenderer, property) { }
    }
}
#endif