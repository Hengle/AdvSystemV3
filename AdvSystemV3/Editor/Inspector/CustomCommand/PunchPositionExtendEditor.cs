using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(PunchPositionExtend))]
    public class PunchPositionExtendEditor : CommandEditorExtend
    {
        public override void DrawCommandGUI() 
        {
            DrawDefaultInspector();
        }
    }
}