using System;

#if !ODIN_INSPECTOR

namespace Sirenix.Utilities
{
    public static class RectExtensions { }

    public class GlobalConfigAttribute : Attribute {
        public GlobalConfigAttribute(string assetPath){}
    }

    public class GlobalConfig<T> : EditorConfig<T> where T : UnityEngine.ScriptableObject
    {

    }
}
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
    public class OdinEditorWindow : UnityEditor.EditorWindow
    {
        protected virtual void OnGUI() { }
    }

    public class OdinEditor : UnityEditor.Editor
    {
        
    }

}
#endif

namespace Sirenix.OdinInspector
{
    public class Title : Attribute
    {
        public Title(string src) { }
    }
    public class ValueDropdown : Attribute
    {
        public ValueDropdown(string src) { }
    }
    public class OnValueChanged : Attribute
    {
        public OnValueChanged(string src) { }
    }
    public class VerticalGroup : Attribute
    {
        public VerticalGroup() { }
        public VerticalGroup(string src) { }
    }

    public class LabelWidth : Attribute
    {
        public LabelWidth() { }
        public LabelWidth(int src) { }
    }

    public class LabelText : Attribute
    {
        public LabelText() { }
        public LabelText(string src) { }
    }

    public class AssetList : Attribute
    {
        public string Path;
        public bool AutoPopulate;
    }

    public class HorizontalGroup : Attribute
    {
        public float LabelWidth;
        public HorizontalGroup(string src) { }
        public HorizontalGroup(string src, float a) { }
    }

    public class FoldoutGroup : Attribute
    {
        public FoldoutGroup(string src) { }
    }

    public class ToggleLeft : Attribute
    { }

    public class ReadOnly : Attribute
    { }

    public class PropertySpace : Attribute
    {
        public int SpaceBefore;
        public int SpaceAfter;
    }

    public class Button : Attribute
    {
        public Button(ButtonSizes size) { }
    }

    public enum ButtonSizes
    {
        Small,
        Medium,
        Large,
    }

    public class InfoBox : Attribute
    {
        public InfoBox(string str) { }
    }

    public class TableList : Attribute
    {
        public bool DrawScrollView;
        public bool HideToolbar;
    }

    public class AssetsOnly : Attribute { }

    public class ShowIfAttribute : Attribute {
        public ShowIfAttribute(string ifstr){}
    }
}

#endif