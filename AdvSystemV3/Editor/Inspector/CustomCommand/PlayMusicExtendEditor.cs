using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(PlayMusicExtend))]
    public class PlayMusicExtendEditor : CommandEditorExtend
    {
        public override void DrawCommandGUI() 
        {
            DrawDefaultInspector();
        }
    }
}