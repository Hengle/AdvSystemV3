using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Adv Config/Editor Config")]
public class AdvEditorConfig : EditorConfig<AdvEditorConfig>
{
    [Adv.HelpBox] public string tip = "此專案編輯器Editor的Adv樣式設定";
    public string AdvPrefabFolderPath = "Assets/Scriptableobjects/AdvContents/";
    public string I2CharacterPrefix = "Character/{0}";
    public string I2MonsterPrefix = "Monster/{0}";
    public Sprite DefaultSprite;
    [HideInInspector] public string DefaultImportBlockName = "Main";
    [HideInInspector] public string DefaultEntranceBlockName = "Main";
    [Tooltip("編輯器 Say 預設顏色數值"), HideInInspector] public Color EditorSaySpeakerNameColor = new Color32(66, 66, 66, 255);
}
