using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(BillBoard))]
    public class BillboardEditor : CommandEditorExtend
    {
        protected SerializedProperty displayProp;
        protected SerializedProperty spriteBillboardProp;
        protected SerializedProperty spriteAtlasProp;
        protected SerializedProperty spriteDiceBillboardProp;
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

        public override void OnEnable()
        {
            base.OnEnable();

            displayProp = serializedObject.FindProperty("display");
            spriteBillboardProp = serializedObject.FindProperty("spriteBillboard");
            spriteAtlasProp = serializedObject.FindProperty("spriteAtlas");
            spriteDiceBillboardProp = serializedObject.FindProperty("spriteDynamicBillboard");
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
        }

        // Update is called once per frame
        public override void DrawCommandGUI() 
        {
            serializedObject.Update();

            BillBoard t = target as BillBoard;

            EditorGUILayout.PropertyField(displayProp);
            if(t._Display != DisplayType.None){
                if(t._Display != DisplayType.MoveToFront){
                    if(t._Display != DisplayType.Hide){
                        EditorGUILayout.PropertyField(spriteBillboardProp, new GUIContent("優先使用怪物立繪"));
                        EditorGUILayout.PropertyField(spriteAtlasProp, new GUIContent("角色立繪"));
                        EditorGUILayout.PropertyField(spriteDiceBillboardProp, new GUIContent("指定表情(選用)"));
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