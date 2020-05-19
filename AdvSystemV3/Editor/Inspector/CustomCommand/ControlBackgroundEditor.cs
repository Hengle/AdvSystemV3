using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(ControlBackground))]
    public class ControlBackgroundEditor : CommandEditorExtend
    {
        public override void DrawCommandGUI() 
        {
            DrawDefaultInspector();
        }
    }
}