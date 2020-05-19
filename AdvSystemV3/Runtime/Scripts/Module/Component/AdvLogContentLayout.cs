using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FungusExt;
using TMPro;

public class AdvLogContentLayout : RecyclingListViewItem
{
    public bool UseDynamicNameColor = false;
    public bool UseDynamicContentColor = false;

    [Header("UI in this layout")]
    public Image AvatarBorder;
    public Image AvatarIcon;
    public Button BTNVoice;
    public TextMeshProUGUI TextName;
    public TextMeshProUGUI TextSay;

    //Data for this Layout
    [HideInInspector] public Sprite AvatarSprite;
    [HideInInspector] public AudioClip AvatarVoice;
    [HideInInspector] public string NameTerm;
    [HideInInspector] public string TextTerm;
    [HideInInspector] public bool showIcon;

    public void UpdateUIContent(){
        SystemLanguage current = (SystemLanguage) AdvUserSettingManager.Instance.AdvLanguage;
        UpdateUIContent(current);
    }
    

    public void UpdateUIContent(SystemLanguage targetLanguage){
        
        if(AvatarIcon != null){
            AvatarIcon.sprite = AvatarSprite;
            if(AvatarIcon.sprite == null)
            {
                AvatarBorder.gameObject.SetActive(false);
            }
            else
            {
                if(AvatarSprite.bounds.size.x > AvatarSprite.bounds.size.y)
                     { AvatarIcon.rectTransform.sizeDelta = new Vector2(AvatarBorder.rectTransform.sizeDelta.x, AvatarBorder.rectTransform.sizeDelta.y * (AvatarSprite.bounds.size.y / AvatarSprite.bounds.size.x));}
                else { AvatarIcon.rectTransform.sizeDelta = new Vector2(AvatarBorder.rectTransform.sizeDelta.x * (AvatarSprite.bounds.size.x / AvatarSprite.bounds.size.y), AvatarBorder.rectTransform.sizeDelta.y);}
                AvatarBorder.gameObject.SetActive(true);
            }
        }
            
        if(AvatarVoice == null)
            BTNVoice.gameObject.SetActive(false);
        else
            BTNVoice.gameObject.SetActive(true);

        if(UseDynamicNameColor){
            Color nameColor;
            nameColor = AdvManager.Instance.advSayDialog.ColorName.TryGetValue(NameTerm, out nameColor) ? nameColor : AdvProjectConfig.Instance.SaySpeakerColor;
            TextName.color = nameColor;
        }

        if(UseDynamicContentColor){
            Color ContentColor;
            ContentColor = AdvManager.Instance.advSayDialog.ColorText.TryGetValue(NameTerm, out ContentColor) ? ContentColor : AdvProjectConfig.Instance.SayContentColor;
            TextSay.color = ContentColor;
        }

        if(string.IsNullOrEmpty(NameTerm))
            TextName.text = "";
        else {
            string _name = LocalizeManager.GetLocalizeName(NameTerm);
            TextName.text = string.IsNullOrEmpty(_name) ? $"no key:{NameTerm}" : _name;
        }

        //When initialize, the "TextTerm" may be null
        if(string.IsNullOrEmpty(TextTerm))
            TextSay.text = "";
        else {
            string _text = LocalizeManager.GetLocalizeText(TextTerm);
            TextSay.text = string.IsNullOrEmpty(_text) ? $"no key:{TextTerm}" : _text;
        }
    }
}
