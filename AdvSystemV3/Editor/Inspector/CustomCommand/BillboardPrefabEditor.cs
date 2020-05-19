using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(BillboardPrefab))]
    public class BillboardPrefabEditor : CommandEditorExtend
    {
        protected SerializedProperty displayProp;
        protected SerializedProperty targetPrefabProp;
        protected SerializedProperty useEmojiProp;
        protected SerializedProperty useBodyProp;
        protected SerializedProperty useEquipsProp;
        protected SerializedProperty flipFaceProp;
        protected SerializedProperty moveProp;
        protected SerializedProperty fromPositionProp;
        protected SerializedProperty toPositionProp;
        protected SerializedProperty positionShiftProp;
        protected SerializedProperty positionShiftValueProp;
        protected SerializedProperty spriteDistanceProp;
        protected SerializedProperty hideWhichProp;
        protected SerializedProperty useDefaultSettingsProp;
        protected SerializedProperty shiftIntoPlaceProp;
        protected SerializedProperty shiftOffsetProp;
        protected SerializedProperty fadeDurationProp;
        protected SerializedProperty moveDurationProp;
        protected SerializedProperty waitUntilFinishedProp;

        public string[] listEmoji = new string[]{};
        public string[] listBody = new string[]{};
        public string[] listEquip = new string[]{};

        public int emojiId = 0;
        public int bodyId = 0;
        public int equipId = 0;

        bool shouldUpdatePrefab = false;

        void Awake(){
             BillboardPrefab t = target as BillboardPrefab;
             if (t == null)
                return;

            if(t._TargetPrefab != null)
                return;

            //快速預選 Character_Erica
            var results = AssetDatabase.FindAssets("Character_Erica");
            foreach (string guid in results)
            {
                UIBillboardController temp = (UIBillboardController)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(UIBillboardController));
                t._TargetPrefab = temp;
                //EditorUtility.SetDirty(t);
                break;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            displayProp = serializedObject.FindProperty("display");
            targetPrefabProp = serializedObject.FindProperty("targetPrefab");
            useEmojiProp = serializedObject.FindProperty("useEmoji");
            useBodyProp = serializedObject.FindProperty("useBody");
            useEquipsProp = serializedObject.FindProperty("useEquips");
            flipFaceProp = serializedObject.FindProperty("flipFace");
            moveProp = serializedObject.FindProperty("move");
            fromPositionProp = serializedObject.FindProperty("fromPosition");
            toPositionProp = serializedObject.FindProperty("toPosition");
            positionShiftProp = serializedObject.FindProperty("positionShift");
            positionShiftValueProp = serializedObject.FindProperty("positionShiftAmount");
            spriteDistanceProp = serializedObject.FindProperty("spriteDistance");
            hideWhichProp = serializedObject.FindProperty("hideWhich");
            useDefaultSettingsProp = serializedObject.FindProperty("useDefaultSettings");
            shiftIntoPlaceProp = serializedObject.FindProperty("shiftIntoPlace");
            shiftOffsetProp = serializedObject.FindProperty("shiftOffset");
            fadeDurationProp = serializedObject.FindProperty("fadeDuration");
            moveDurationProp = serializedObject.FindProperty("moveDuration");
            waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");

            UpdatePrefabInfo();
        }

        void UpdatePrefabInfo(){

            BillboardPrefab t = target as BillboardPrefab;

            if(t._TargetPrefab != null){
                listEmoji = t._TargetPrefab.GetEmojiListString().ToArray();
                listBody = t._TargetPrefab.GetBodyListString().ToArray();
                listEquip = t._TargetPrefab.GetEquipListString().ToArray();
            }
            else
                listEmoji = new string[]{};
        }

        // Update is called once per frame
        public override void DrawCommandGUI() 
        {
            serializedObject.Update();

            BillboardPrefab t = target as BillboardPrefab;

            if(shouldUpdatePrefab == true){
                shouldUpdatePrefab = false;

                if(t._TargetPrefab != null){
                    listEmoji = t._TargetPrefab.GetEmojiListString().ToArray();
                    listBody = t._TargetPrefab.GetBodyListString().ToArray();
                    listEquip = t._TargetPrefab.GetEquipListString().ToArray();
                }
                else
                    listEmoji = new string[]{};
            }

            EditorGUILayout.PropertyField(displayProp);
            if(t._Display != DisplayType.None){
                if(t._Display != DisplayType.MoveToFront){
                    if(t._Display != DisplayType.Hide){

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(targetPrefabProp, new GUIContent("角色立繪Prefab"));

                        if(AdvKeyContent.GetCurrentInstance().GroupBillboardPrefab != null)
                            CommandEditor.ObjectField<UIBillboardController>(targetPrefabProp, 
                                                    new GUIContent("In folder (Prefabs/ADV)", "Dynamic Emoji Prefab (UIBillboardController)"), 
                                                    new GUIContent("<None>"),
                                                    AdvKeyContent.GetCurrentInstance().GroupBillboardPrefab);
                        
                        if (EditorGUI.EndChangeCheck())
                            shouldUpdatePrefab = true;

                        EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(useEmojiProp, new GUIContent("使用表情"));
                            int emojiIndex = EditorGUILayout.Popup(emojiId, listEmoji, EditorStyles.popup);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(useBodyProp, new GUIContent("使用衣服"));
                            int bodyIndex = EditorGUILayout.Popup(bodyId, listBody, EditorStyles.popup);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(useEquipsProp, new GUIContent("使用裝備(我前面有箭頭)"), true);
                            int equipIndex = EditorGUILayout.Popup(equipId, listEquip, EditorStyles.popup);
                        EditorGUILayout.EndHorizontal();


                        if (emojiIndex != emojiId)
                            useEmojiProp.stringValue = listEmoji[emojiIndex];
                        if (bodyIndex != bodyId)
                            useBodyProp.stringValue = listBody[bodyIndex];
                        if (equipIndex != equipId){
                            t._UseEquips.Add(listEquip[equipIndex]);
                            EditorUtility.SetDirty(t);
                        }

                        EditorGUILayout.PropertyField(flipFaceProp, new GUIContent("水平翻轉 ?"));
                        EditorGUILayout.PropertyField(spriteDistanceProp, new GUIContent("立繪所在距離"));
                    }
                    if(t._Display == DisplayType.Hide){
                        EditorGUILayout.PropertyField(hideWhichProp, new GUIContent("隱藏哪個位置"));
                    }
                    
                    if(t._Display == DisplayType.Show || t._Display == DisplayType.Replace || (t._Move && t._Display == DisplayType.Hide)){
                         EditorGUILayout.PropertyField(toPositionProp, new GUIContent("目標位置"));
                         EditorGUILayout.PropertyField(positionShiftProp, new GUIContent("目標位置加上偏移"));
                         EditorGUILayout.PropertyField(positionShiftValueProp, new GUIContent("偏移倍數"));
                    }
                    EditorGUILayout.LabelField("-- 移動設定 --", EditorStyles.boldLabel); 
                    EditorGUILayout.PropertyField(moveProp, new GUIContent("移動動畫 ?"));

                    if(t._Move){
                        if(!t._ShiftIntoPlace){
                            EditorGUILayout.PropertyField(fromPositionProp, new GUIContent("從哪個相對位置滑入"));
                        }
                    }
                    
                    
                    EditorGUILayout.PropertyField(useDefaultSettingsProp, new GUIContent("使用預設數值 ?"));

                    if(!t._UseDefaultSettings){
                        
                        if(t._Move){
                            if(t._Display != DisplayType.Hide){
                                EditorGUILayout.PropertyField(shiftIntoPlaceProp, new GUIContent("從目標位置附近滑入?"));
                                if(t._ShiftIntoPlace){
                                    EditorGUILayout.PropertyField(shiftOffsetProp, new GUIContent("從哪邊滑入(偏移量)"));
                                }
                            }
                        }
                        EditorGUILayout.PropertyField(fadeDurationProp, new GUIContent("淡出 / 淡入時間"));
                        if(t._Move){
                            EditorGUILayout.PropertyField(moveDurationProp, new GUIContent("滑動時間"));
                        }
                    }
                    
                    EditorGUILayout.PropertyField(waitUntilFinishedProp);
                }
                else {
                    EditorGUILayout.PropertyField(toPositionProp, new GUIContent("哪個位置"));
                }
            }

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("myVariable"), new GUIContent("aDifferentLabel"));

            serializedObject.ApplyModifiedProperties();

        }
    }
}