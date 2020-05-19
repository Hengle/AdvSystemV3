using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UIBillboardController : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> Emoji = null;
    [SerializeField] private List<CanvasGroup> Body = null;
    [SerializeField] private List<CanvasGroup> Equip = null;
    [SerializeField] private CanvasGroup DefaultEmoji = null;
    [SerializeField] private CanvasGroup DefaultBody = null;
    [SerializeField][ValueDropdown ("useNamePopup")] public string DataTerm;

    [Title("Runtime Setting")]
    [ValueDropdown("GetEmojiList"), OnValueChanged("ChangeEmoji"), SerializeField] private CanvasGroup useEmoji;
    [ValueDropdown("GetBodyList"), OnValueChanged("ChangeBody"), SerializeField] private CanvasGroup useBody;
    [ValueDropdown("GetEquipList"), OnValueChanged("ChangeEquip"), SerializeField] private List<CanvasGroup> useEquip = null;


    string prepareToUseEmoji;
    string prepareToUseBody;
    List<string> prepareToUseEquip;
    Dictionary<string, CanvasGroup> runtimeEmojiDic;
    Dictionary<string, CanvasGroup> runtimeBodyDic;
    Dictionary<string, CanvasGroup> runtimeEquipDic;
    public bool useCustom = false;

    List<CanvasGroup> GetEmojiList(){
        return Emoji;
    }
    List<CanvasGroup> GetBodyList(){
        return Body;
    }
    List<CanvasGroup> GetEquipList(){
        List<CanvasGroup> list = new List<CanvasGroup>();
        foreach (var item in Equip)
            if(!useEquip.Contains(item))
                list.Add(item);
        
        return list;
    }

    void ChangeEmoji(){
        foreach (var item in Emoji)
            item.alpha = 0;
        useEmoji.alpha = 1;
    }
    void ChangeBody(){
        foreach (var item in Body)
            item.alpha = 0;
        useBody.alpha = 1;
    }
    void ChangeEquip(){
        foreach (var item in Equip)
            item.alpha = 0;
        foreach (var item in useEquip)
            item.alpha = 1;
    }

    public List<string> GetEmojiListString(){
        List<string> list = new List<string>(){"<Select>"};
        foreach (var item in Emoji)
            list.Add(item.name);
        return list;
    }

    public List<string> GetBodyListString(){
        List<string> list = new List<string>(){"<Select>"};
        foreach (var item in Body)
            list.Add(item.name);
        return list;
    }

    public List<string> GetEquipListString(){
        List<string> list = new List<string>(){"<Select>"};
        foreach (var item in Equip)
            list.Add(item.name);
        return list;
    }

    void Awake() {
        InitRuntimeDictionary();
    }

    void Start()
    {
        DefaultBillboard();
        CustomBillboard();
    }

    void DefaultBillboard(){
        if(useCustom)
            return;

        useEmoji = DefaultEmoji;
        useBody = DefaultBody;
        useEquip.Clear();
        ChangeEmoji();
        ChangeBody();
        ChangeEquip();
    }

    void CustomBillboard(){
        if(!useCustom)
            return;

        //is it Effect ? 
        RuntimeSetEmoji(prepareToUseEmoji);
        RuntimeSetBody(prepareToUseBody);
        RuntimeSetEquip(prepareToUseEquip);
    }
    public void SetInitDisplay(string emojiName, string bodyName, List<string> equipName){
        useCustom = true;
        prepareToUseEmoji = emojiName;
        prepareToUseBody =  bodyName;
        prepareToUseEquip = equipName;
    }

    void InitRuntimeDictionary(){
        runtimeEmojiDic = new Dictionary<string, CanvasGroup>();
        runtimeBodyDic = new Dictionary<string, CanvasGroup>();
        runtimeEquipDic = new Dictionary<string, CanvasGroup>();

        foreach (var item in Emoji)
            runtimeEmojiDic.Add(item.name, item);
        foreach (var item in Body)
            runtimeBodyDic.Add(item.name, item);
        foreach (var item in Equip){
            runtimeEquipDic.Add(item.name, item);
            runtimeEquipDic.Add(item.name.Replace("Equip_",""), item);
        }
    }

    public void RuntimeSetEmoji(string emojiName){
        if(string.IsNullOrEmpty(emojiName))
            return;

        if(runtimeEmojiDic.ContainsKey(emojiName))
            useEmoji = runtimeEmojiDic[emojiName];

        ChangeEmoji();
    }

    public void RuntimeSetBody(string bodyName){
        if(string.IsNullOrEmpty(bodyName))
            return;

        if(runtimeBodyDic.ContainsKey(bodyName))
            useBody = runtimeBodyDic[bodyName];

        ChangeBody();
    }

    public void RuntimeSetEquip(List<string> equipsName){
        if(equipsName == null)
            return;

        foreach (var item in equipsName)
            if(!string.IsNullOrEmpty(item) && runtimeEquipDic.ContainsKey(item))
                useEquip.Add(runtimeEquipDic[item]);
        
        ChangeEquip();
    }

    public void RuntimeUnequip(){
        useEquip.Clear();
        ChangeEquip();
    }

    public List<string> useNamePopup(){
        return FungusExt.AdvLocalizeContent.Instance.GetActorNamesList();
    }
}