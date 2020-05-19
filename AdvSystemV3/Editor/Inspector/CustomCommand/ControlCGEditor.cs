using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(ControlCG))]
    public class ControlCGEditor : CommandEditorExtend
    {
        protected SerializedProperty atlasCGProp;
        protected SerializedProperty spriteCGProp;
        protected SerializedProperty sprtieColorProp;
        protected SerializedProperty displayProp;
        protected SerializedProperty durationProp;
        protected SerializedProperty FadeEaseTypeProp;
        protected SerializedProperty waitUntilFinishedProp;

        public override void OnEnable()
        {
            base.OnEnable();

            atlasCGProp = serializedObject.FindProperty("atlasCG");
            spriteCGProp = serializedObject.FindProperty("spriteCG");
            sprtieColorProp = serializedObject.FindProperty("sprtieColor");
            displayProp = serializedObject.FindProperty("display");
            durationProp = serializedObject.FindProperty("duration");
            FadeEaseTypeProp = serializedObject.FindProperty("FadeEaseType");
            waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");

            ControlCG t = target as ControlCG;
            if(t != null){
                if(t.SpriteCG != null){
                    if(t.AtlasCG == null && t.SpriteCG.AtlasAsset != null){
                        t.AtlasCG = t.SpriteCG.AtlasAsset;
                        //EditorUtility.SetDirty(t);
                    }
                }
            }
        }

        public override void DrawCommandGUI() 
        {
            serializedObject.Update();
            ControlCG t = target as ControlCG;

            EditorGUILayout.PropertyField(atlasCGProp);
            

            bool showDSChild = false;

            if (t.AtlasCG != null &&
                t.GetCGInAtlas() != null &&
                t.GetCGInAtlas().Count > 0)
            {
                showDSChild = true;
            }

            if(showDSChild){
                CommandEditor.ObjectField<SpriteDicing.DicedSprite>(spriteCGProp, 
                                                    new GUIContent("CG Child Name", "CG in the Atlas files name"), 
                                                    new GUIContent("<None>"),
                                                    t.GetCGInAtlas());
            }

            EditorGUILayout.PropertyField(spriteCGProp);

            EditorGUILayout.PropertyField(sprtieColorProp);
            EditorGUILayout.PropertyField(displayProp);
            EditorGUILayout.PropertyField(durationProp);
            EditorGUILayout.PropertyField(FadeEaseTypeProp);
            EditorGUILayout.PropertyField(waitUntilFinishedProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}