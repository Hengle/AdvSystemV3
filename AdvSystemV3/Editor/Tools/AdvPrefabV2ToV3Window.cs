#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fungus;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System.IO;

namespace FungusExt
{
    public class AdvPrefabV2ToV3Window : OdinEditorWindow
    {
        [MenuItem("Tools/Fungus/Adv Prefab V2 To V3")]
        public static AdvPrefabV2ToV3Window OpenWindow()
        {
            var window = GetWindow<AdvPrefabV2ToV3Window>(false, "Prefab V2 To V3 (Require Odin plugin)");

            const int width = 700;
            const int height = 700;
            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);

            return window;
        }

        protected override void OnGUI()
        {
            EditorGUILayout.HelpBox("用途: 將V2的Prefab內容轉為V3內容的土炮工具 (Require Odin plugin)", MessageType.Info);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            base.OnGUI();
        }

        public List<Flowchart> srcFlowcharts;
        public GameObject otherObject;
        public UpdateListV2ToV3 updateList;

        [Button(ButtonSizes.Large)]
        public void StartConvert()
        {
            if (srcFlowcharts == null)
                return;

            Dictionary<string, string> guidMap = new Dictionary<string, string>();
            foreach (var item in updateList.placeGuid)
            {
                string s_guid;
                long s_file;

                string r_guid;
                long r_file;

                if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item.search, out s_guid, out s_file) && UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item.replace, out r_guid, out r_file))
                {
                    guidMap.Add(s_guid, r_guid);
                }
            }

            foreach (var srcFlowchart in srcFlowcharts)
            {
                //直接對YAML 進行編輯
                string fullPath = Application.dataPath.Replace("/Assets", "/") + UnityEditor.AssetDatabase.GetAssetPath(srcFlowchart);
                string text = File.ReadAllText(fullPath);

                foreach (var item in guidMap)
                {
                    text = text.Replace(item.Key, item.Value);
                }
                text = text.Replace("CSVLine: ", "csvLine: ");
                text = text.Replace("CSVCommandKey: ", "csvCommandKey: ");
                text = text.Replace("speakerName: ", "overrideTerm: ");
                text = text.Replace("portrait: ", "overridePortrait: ");
                File.WriteAllText(fullPath, text);

                UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(srcFlowchart));
            }
        }
        [Button(ButtonSizes.Large)]
        public void ConvertOtherObject()
        {
            if (otherObject == null)
                return;

            Dictionary<string, string> guidMap = new Dictionary<string, string>();
            foreach (var item in updateList.placeGuid)
            {
                string s_guid;
                long s_file;

                string r_guid;
                long r_file;

                if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item.search, out s_guid, out s_file) && UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item.replace, out r_guid, out r_file))
                {
                    guidMap.Add(s_guid, r_guid);
                }
            }

            //直接對YAML 進行編輯
            string fullPath = Application.dataPath.Replace("/Assets", "/") + UnityEditor.AssetDatabase.GetAssetPath(otherObject);
            string text = File.ReadAllText(fullPath);

            foreach (var item in guidMap)
            {
                text = text.Replace(item.Key, item.Value);
            }
            File.WriteAllText(fullPath, text);

            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(otherObject));
        }
    }
}

#endif