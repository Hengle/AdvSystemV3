using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteDicing;
using DG.Tweening;

public class BillboardController : MonoBehaviour
{
    public DicedSpriteAtlas BelongAtlas;
    bool inForeGround = true;

    public DicedSpriteRenderer GetRenderer(){
        return GetComponent<DicedSpriteRenderer>();
    }

    public DicedSprite GetSprite(string _name){
        if(BelongAtlas == null)
            return null;
        
        return BelongAtlas.GetSprite(_name);
    }

    public void SetRendererSprite(DicedSprite _sprite){
        GetRenderer().SetDicedSprite(_sprite);
    }

    public void SetRendererSprite(string _sprite){
        DicedSprite _target = GetSprite(_sprite);
        if(_target == null)
            return;

        SetRendererSprite(_target);
    }

    public void SetRendererSprite(Sprite _sprite){
        if(_sprite == null)
            return;

        SetRendererSprite(_sprite.name);
    }

    public void SetToBackground(Color backColor){
        GetRenderer().Color = backColor;
        if(inForeGround){
            transform.DOLocalMoveY(transform.localPosition.y - AdvManager.Instance.advStage.DimYValue, AdvManager.Instance.advStage.DimYDuration);
            //transform.position += new Vector3(0, -, 0);
        }
        inForeGround = false;
    }

    public void SetToForeground(float sortZ){
        GetRenderer().Color = new Color(1, 1, 1, 1);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, sortZ);
        if(!inForeGround){
            transform.DOLocalMoveY(transform.localPosition.y + AdvManager.Instance.advStage.DimYValue, AdvManager.Instance.advStage.DimYDuration);
            //transform.localPosition += new Vector3(0, AdvManager.Instance.mainStage.DimYValue, 0);
        }
        inForeGround = true;
    }
}
