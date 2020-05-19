using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(PunchRotationExtend))]
    public class PunchRotationExtendEditor : CommandEditorExtend
    {
        public override void DrawCommandGUI() 
        {
            DrawDefaultInspector();
        }
    }
}