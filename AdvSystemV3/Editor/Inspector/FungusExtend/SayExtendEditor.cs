// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(SayExtend))]
    public class SayExtendEditor : CommandEditorExtend
    {
        public static bool showTagHelp;
        public Texture2D blackTex;
        
        public static void DrawTagHelpLabel()
        {
            string tagsText = TextTagParser.GetTagHelp();

            if (CustomTag.activeCustomTags.Count > 0)
            {
                tagsText += "\n\n\t-------- CUSTOM TAGS --------";
                List<Transform> activeCustomTagGroup = new List<Transform>();
                foreach (CustomTag ct in CustomTag.activeCustomTags)
                {
                    if(ct.transform.parent != null)
                    {
                        if (!activeCustomTagGroup.Contains(ct.transform.parent.transform))
                        {
                            activeCustomTagGroup.Add(ct.transform.parent.transform);
                        }
                    }
                    else
                    {
                        activeCustomTagGroup.Add(ct.transform);
                    }
                }
                foreach(Transform parent in activeCustomTagGroup)
                {
                    string tagName = parent.name;
                    string tagStartSymbol = "";
                    string tagEndSymbol = "";
                    CustomTag parentTag = parent.GetComponent<CustomTag>();
                    if (parentTag != null)
                    {
                        tagName = parentTag.name;
                        tagStartSymbol = parentTag.TagStartSymbol;
                        tagEndSymbol = parentTag.TagEndSymbol;
                    }
                    tagsText += "\n\n\t" + tagStartSymbol + " " + tagName + " " + tagEndSymbol;
                    foreach(Transform child in parent)
                    {
                        tagName = child.name;
                        tagStartSymbol = "";
                        tagEndSymbol = "";
                        CustomTag childTag = child.GetComponent<CustomTag>();
                        if (childTag != null)
                        {
                            tagName = childTag.name;
                            tagStartSymbol = childTag.TagStartSymbol;
                            tagEndSymbol = childTag.TagEndSymbol;
                        }
                            tagsText += "\n\t      " + tagStartSymbol + " " + tagName + " " + tagEndSymbol;
                    }
                }
            }
            tagsText += "\n";
            float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagsText), EditorGUIUtility.currentViewWidth);
            EditorGUILayout.SelectableLabel(tagsText, GUI.skin.GetStyle("HelpBox"), GUILayout.MinHeight(pixelHeight));
        }
        
        protected SerializedProperty showIconProp;
        protected SerializedProperty overrideTermProp;
        protected SerializedProperty overridePortraitProp;
        protected SerializedProperty csvCommandKeyProp;
        protected SerializedProperty dataCharacterProp;
        protected SerializedProperty portraitEquipProp;
        //// NEW EXTEND

        protected SerializedProperty characterProp;
        protected SerializedProperty portraitProp;
        protected SerializedProperty storyTextProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty voiceOverClipProp;
        protected SerializedProperty showAlwaysProp;
        protected SerializedProperty showCountProp;
        protected SerializedProperty extendPreviousProp;
        protected SerializedProperty fadeWhenDoneProp;
        protected SerializedProperty waitForClickProp;
        protected SerializedProperty stopVoiceoverProp;
        protected SerializedProperty setSayDialogProp;
        protected SerializedProperty waitForVOProp;

        public override void OnEnable()
        {
            base.OnEnable();
            CatchLocalizeText();
            return;
        }

        public void InitializeSayExtendEditor()
        {
            //Debug.Log("Run Initialzed");
            showIconProp = serializedObject.FindProperty("showIcon");
            overrideTermProp = serializedObject.FindProperty("overrideTerm");
            overridePortraitProp = serializedObject.FindProperty("overridePortrait");
            csvCommandKeyProp = serializedObject.FindProperty("csvCommandKey");
            dataCharacterProp = serializedObject.FindProperty("dataCharacter");
            portraitEquipProp = serializedObject.FindProperty("portraitEquip");
            // NEW EXTEND

            characterProp = serializedObject.FindProperty("character");
            portraitProp = serializedObject.FindProperty("portrait");
            storyTextProp = serializedObject.FindProperty("storyText");
            descriptionProp = serializedObject.FindProperty("description");
            voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
            showAlwaysProp = serializedObject.FindProperty("showAlways");
            showCountProp = serializedObject.FindProperty("showCount");
            extendPreviousProp = serializedObject.FindProperty("extendPrevious");
            fadeWhenDoneProp = serializedObject.FindProperty("fadeWhenDone");
            waitForClickProp = serializedObject.FindProperty("waitForClick");
            stopVoiceoverProp = serializedObject.FindProperty("stopVoiceover");
            setSayDialogProp = serializedObject.FindProperty("setSayDialog");
            waitForVOProp = serializedObject.FindProperty("waitForVO");

            CatchLocalizeText();
        }

        protected virtual void CatchLocalizeText(){
            //Catch Localize Data when focus
            SayExtend t = target as SayExtend;
            FlowchartExtend flowchart = t.GetComponent<FlowchartExtend>();
            csvCommandKeyProp = serializedObject.FindProperty("csvCommandKey");
            storyTextProp = serializedObject.FindProperty("storyText");
            
            string textTerm = $"{flowchart.GoogleSheetID}.{flowchart.GooglePageID}.{csvCommandKeyProp.stringValue}";
            string localText = FungusExt.LocalizeManager.GetLocalizeText(textTerm);
            if(string.IsNullOrEmpty(localText)){
                string blockName = AdvUtility.FindParentBlock(flowchart, t).BlockName;
                Debug.LogError($"At : {t.gameObject.name} / {blockName} / {t.ItemId}");
            }
            
            storyTextProp.stringValue = string.IsNullOrEmpty(localText) ? $"(key not found : {textTerm})" : localText;
            serializedObject.ApplyModifiedProperties();
        }

        public override void DrawCommandGUI() 
        {
            if(showIconProp == null)
                InitializeSayExtendEditor();

            serializedObject.Update();

            bool showPortraits = false;
            bool overrideSpeaker = true;

            EditorGUILayout.PropertyField(showIconProp);

            if(overrideSpeaker) {
                CommandEditorExtend.StringField(overrideTermProp, 
                                                    new GUIContent("Override Name Term", "The localize key of speaker name"), 
                                                    new GUIContent("<None>"),
                                                    FungusExt.AdvLocalizeContent.Instance.GetActorNamesList());
                EditorGUILayout.PropertyField(overridePortraitProp);
                EditorGUILayout.PropertyField(csvCommandKeyProp);
            }

            EditorGUILayout.PropertyField(portraitEquipProp, true);

            Say t = target as Say;

            
            EditorGUILayout.PropertyField(storyTextProp);

            EditorGUILayout.PropertyField(descriptionProp);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(extendPreviousProp);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Tag Help", "View available tags"), new GUIStyle(EditorStyles.miniButton)))
            {
                showTagHelp = !showTagHelp;
            }
            EditorGUILayout.EndHorizontal();
            
            if (showTagHelp)
            {
                DrawTagHelpLabel();
            }
            
            EditorGUILayout.Separator();
            
            EditorGUILayout.PropertyField(voiceOverClipProp, 
                                          new GUIContent("Voice Over Clip", "Voice over audio to play when the text is displayed"));

            EditorGUILayout.PropertyField(showAlwaysProp);
            
            if (showAlwaysProp.boolValue == false)
            {
                EditorGUILayout.PropertyField(showCountProp);
            }

            GUIStyle centeredLabel = new GUIStyle(EditorStyles.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            GUIStyle leftButton = new GUIStyle(EditorStyles.miniButtonLeft);
            leftButton.fontSize = 10;
            leftButton.font = EditorStyles.toolbarButton.font;
            GUIStyle rightButton = new GUIStyle(EditorStyles.miniButtonRight);
            rightButton.fontSize = 10;
            rightButton.font = EditorStyles.toolbarButton.font;

            EditorGUILayout.PropertyField(fadeWhenDoneProp);
            EditorGUILayout.PropertyField(waitForClickProp);
            EditorGUILayout.PropertyField(stopVoiceoverProp);
            EditorGUILayout.PropertyField(setSayDialogProp);
            EditorGUILayout.PropertyField(waitForVOProp);
            
            if (showPortraits && t.Portrait != null)
            {
                Texture2D characterTexture = t.Portrait.texture;
                float aspect = (float)characterTexture.width / (float)characterTexture.height;
                Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
                if (characterTexture != null)
                {
                    GUI.DrawTexture(previewRect,characterTexture,ScaleMode.ScaleToFit,true,aspect);
                }
            }

            EditorGUILayout.PropertyField(dataCharacterProp);
            
            serializedObject.ApplyModifiedProperties();
        }
    }    
}