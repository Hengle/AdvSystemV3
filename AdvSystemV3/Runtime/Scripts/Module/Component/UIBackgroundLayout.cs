using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteDicing;
using DG.Tweening;

public class UIBackgroundLayout : MonoBehaviour
{
    public Image BackgroundColor;
    public Image BackgroundTexFront;
    public Image BackgroundTexBehide;
    public GameObject BackgroundDiceSpriteFront;
    public GameObject BackgroundDiceSpriteBehide;
    public GameObject BackgroundDiceImageFront;
    public GameObject BackgroundDiceImageBehide;

    [HideInInspector] public DicedSpriteRenderer DS_CG_Front;
    [HideInInspector] public DicedSpriteRenderer DS_CG_Behide;
    [HideInInspector] public Utage.DicingImage DI_CG_Front;
    [HideInInspector] public Utage.DicingImage DI_CG_Behide;

    float fadeoutTime;

    public System.Action<DicedSprite> OnReadCG;

    protected void Awake(){
        DS_CG_Front = BackgroundDiceSpriteFront.AddComponent<DicedSpriteRenderer>();
        DS_CG_Behide = BackgroundDiceSpriteBehide.AddComponent<DicedSpriteRenderer>();

        //DI_CG_Front = BackgroundDiceImageFront.AddComponent<Utage.DicingImage>();
        //DI_CG_Behide = BackgroundDiceImageBehide.AddComponent<Utage.DicingImage>();
    }
    
    public void FadeOutAllCG(){
        fadeoutTime = AdvManager.Instance.advStage.FadeDuration;

        if(DS_CG_Front.Color.a < 0.9f)
            DOTween.To(() => DS_CG_Behide.Color, x => DS_CG_Behide.Color = x, new Color(0, 0, 0, 0), fadeoutTime).SetEase(Ease.Linear);
        else
            DS_CG_Behide.Color = new Color(1, 1, 1, 0);
            
        DOTween.To(() => DS_CG_Front.Color, x => DS_CG_Front.Color = x, new Color(0, 0, 0, 0), fadeoutTime).SetEase(Ease.Linear);
    }

    public void FadeOutAllBackground(){

        fadeoutTime = AdvManager.Instance.advStage.FadeDuration;

        BackgroundColor.color = new Color(0, 0, 0, 0);
        if(BackgroundTexFront.color.a < 0.9f)
            BackgroundTexBehide.DOFade(0f, fadeoutTime);
        else
            BackgroundTexBehide.color = new Color(1, 1, 1, 0);
            
        BackgroundTexFront.DOFade(0f, fadeoutTime);
    }

}
