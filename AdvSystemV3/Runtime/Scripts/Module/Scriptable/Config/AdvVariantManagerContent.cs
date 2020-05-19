using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteDicing;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Adv Config/Variant Manager")]
public class AdvVariantManagerContent : ReleaseConfig<AdvVariantManagerContent>
{
    [Header("放入所有的可切分 CG"), AssetList(Path = "Textures/EventCG/", AutoPopulate = true)]
    public List<DicedSpriteAtlas> VariantsDiceSprite;
}

public class AdvVariantManager
{
    static AdvVariantManager instance;

    public static AdvVariantManager Instance {
        get {
            if(instance == null){
                instance = new AdvVariantManager();
            }
            return instance;
        }
    }

    public AdvVariantManager(){
        DicDiceSpriteAtlas = new Dictionary<string, DicedSpriteAtlas>();
        DicDiceSprite = new Dictionary<string, DicedSprite>();

        foreach (var item in AdvVariantManagerContent.Instance.VariantsDiceSprite)
        {
            DicDiceSpriteAtlas.Add(item.name, item);

            foreach (var dice in item.GetDicedSpriteList())
            {
                DicDiceSprite.Add($"{item.name}.{dice.name}", dice);
            }
        }
    }

    public Dictionary<string, DicedSpriteAtlas> DicDiceSpriteAtlas;
    public Dictionary<string, DicedSprite> DicDiceSprite;

    public DicedSpriteAtlas GetDiceSpriteAtlas(string name){
        if(string.IsNullOrEmpty(name))
            return null;

        DicedSpriteAtlas result;
        if(DicDiceSpriteAtlas.TryGetValue(name, out result))
            return result;

        return null;
    }

    public DicedSprite GetDiceSprite(string name){
        if(string.IsNullOrEmpty(name))
            return null;

        DicedSprite result;
        if(DicDiceSprite.TryGetValue(name, out result))
            return result;

        return null;
    }
}