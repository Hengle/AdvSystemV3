using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;
using Sirenix.Utilities;


[CreateAssetMenu(menuName = "Adv Manager/Key Resource")]
public class AdvKeyContent : GlobalConfig<AdvKeyContent>
{
    public static AdvKeyContent GetCurrentInstance()
    {
        return Instance;
    }

    [Adv.HelpBox] public string tip = "Key 值就是圖檔名唷";

    [Header("放入所有的頭像，預設為自動抓取"), AssetList(Path = "/Textures/Avatar/", AutoPopulate = true)]
    public List<Sprite> GroupAvatar;

    [Header("放入所有的背景"), AssetList(Path = "Textures/Background/", AutoPopulate = true)]
    public List<Sprite> GroupBackground;

    [Header("放入所有的可切分 CG"), AssetList(Path = "Textures/EventCG/", AutoPopulate = true)]
    public List<SpriteDicing.DicedSpriteAtlas> GroupDicingCGAtlas;

    [Header("放入所有的立繪 (舊版不會動的)"), AssetList(Path = "Textures/Character/", AutoPopulate = true)]
    public List<Sprite> GroupBillBoard;

    [Header("放入所有的立繪 (新板 Dicing)"), AssetList(Path = "Textures/Character/", AutoPopulate = true)]
    public List<SpriteDicing.DicedSpriteAtlas> GroupDicingSpriteAtlas;

    [Header("放入所有的Prefab立繪"), AssetList(Path = "Prefabs/Character/", AutoPopulate = true)]
    public List<UIBillboardController> GroupBillboardPrefab;

    [Header("放入所有的怪物"), AssetList(Path = "Textures/Enemy/", AutoPopulate = true)]
    public List<Sprite> GroupEnemy;

    [Header("放入所有的Timeline"), AssetList(Path = "Animations/", AutoPopulate = true)]
    public List<TimelineAsset> GroupTimeline;

    [Header("放入所有的BGM"), AssetList(Path = "Sounds/Music/", AutoPopulate = true)]
    public List<AudioClip> GroupBGM;

    [Header("放入所有的語音(Voice)"), AssetList(Path = "Sounds/Voice/", AutoPopulate = true)]
    public List<AudioClip> GroupVoice;

    public Sprite GetAvatarByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupAvatar.Find(e => e.name == key);
    }

    public Sprite GetBackgroundByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupBackground.Find(e => e.name == key);
    }

    public Sprite GetBillboardByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupBillBoard.Find(e => e.name == key);
    }

    public SpriteDicing.DicedSprite GetDiceCGByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        foreach (var item in GroupDicingCGAtlas)
        {
            var _target = item.GetSprite(key);
            if (_target != null)
                return _target;
        }

        return null;
    }
    public SpriteDicing.DicedSpriteAtlas GetDiceCGAtlasByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupDicingCGAtlas.Find(e => e.name == key);
    }

    public SpriteDicing.DicedSpriteAtlas GetDiceAtlasByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupDicingSpriteAtlas.Find(e => e.name == key);
    }

    public UIBillboardController GetBillboardPrefabByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupBillboardPrefab.Find(e => e.name == key);
    }

    public SpriteDicing.DicedSprite GetDiceBillboardByKey(string key, SpriteDicing.DicedSpriteAtlas atlas)
    {
        if (key == "" || key == null)
            return null;

        return atlas.GetSprite(key);
    }

    public SpriteDicing.DicedSprite GetDiceBillboardByKeyContain(string key, SpriteDicing.DicedSpriteAtlas atlas)
    {
        if (key == "" || key == null)
            return null;

        return atlas.GetSpriteContainName(key);
    }

    public SpriteDicing.DicedSprite GetDiceBillboardByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        foreach (var item in GroupDicingSpriteAtlas)
        {
            var _target = item.GetSprite(key);
            if (_target != null)
                return _target;
        }

        return null;
    }

    public Sprite GetEnemyByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupEnemy.Find(e => e.name == key);
    }

    public TimelineAsset GetTimelineByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupTimeline.Find(e => e.name == key);
    }

    public AudioClip GetBGMByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupBGM.Find(e => e.name == key);
    }

    public AudioClip GetVoiceByKey(string key)
    {
        if (key == "" || key == null)
            return null;

        return GroupVoice.Find(e => e.name == key);
    }
}
