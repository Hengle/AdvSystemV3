using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(PunchScaleExtend))]
    public class PunchScaleExtendEditor : CommandEditorExtend
    {
        public override void DrawCommandGUI() 
        {
            DrawDefaultInspector();
        }
    }
}