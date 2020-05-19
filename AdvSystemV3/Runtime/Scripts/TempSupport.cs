using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TempSupport : MonoBehaviour , IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData){

    }
}

#if DICE_SPRITE

// namespace SpriteDicing
// {
//     public class DicedSpriteAtlas
//     {
//         public string name;
//         public DicedSprite GetSprite(string str){
//             return null;
//         }

//         public DicedSprite GetSpriteContainName(string str){
//             return null;
//         }
//     }

//     public class DicedSprite
//     {
//         public string name;
//     }
// }

// public class UIBillboardController
// {
//     public string name;
// }
#endif