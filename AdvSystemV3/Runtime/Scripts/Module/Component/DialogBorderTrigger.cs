using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

public class DialogBorderTrigger : MonoBehaviour
{
    public RectTransform NameDefault;
    public RectTransform TextDefault;
    public RectTransform NameIcon;
    public RectTransform TextIcon;
    public RectTransform NameNoIcon;
    public RectTransform TextNoIcon;

    private void OnEnable() {
        if(NameDefault && NameIcon){
            Trans(NameIcon, NameDefault);
        }
        if(TextDefault && TextIcon){
            Trans(TextIcon, TextDefault);
        }
    }

    private void OnDisable() {
        if(NameDefault && NameNoIcon){
            Trans(NameNoIcon, NameDefault);
        }
        if(TextDefault && TextNoIcon){
            Trans(TextNoIcon, TextDefault);
        }
    }

    public void Trans(RectTransform _from, RectTransform _target)
    {
        _target.anchoredPosition = _from.anchoredPosition;
        _target.sizeDelta = _from.sizeDelta;
    }
}
