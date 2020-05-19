using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fungus
{
    [CreateAssetMenu(menuName = "Adv Content/Character Content")]
    public class AdvDataCharacter : ScriptableObject
    {
        [Tooltip("Display Name key in Multi-Language")]
        public string nameTerm;

        [Tooltip("Color to display the character name in Say Dialog.")]
        public Color nameColor = Color.white;

        [Tooltip("Color to display the character Say in Say Dialog.")]
        public Color textColor = Color.white;

        [Tooltip("Sound effect to play when this character is speaking.")]
        public AudioClip soundEffect;

        [Tooltip("List of portrait images that can be displayed for this character.")]
        public List<Sprite> portraits;

        [Tooltip("Direction that portrait sprites face.")]
        public FacingDirection portraitsFace;

        [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. This Say Dialog will be used whenever the character speaks.")]
        public SayDialog setSayDialog;

        [TextArea(5, 10)]
        public string description;

        public virtual List<Sprite> Portraits => portraits;
        public virtual SayDialog SetSayDialog => setSayDialog;
        public virtual Sprite ProfileSprite { get; set; }


        public virtual Sprite GetDefaultPortrait()
        {
            if(portraits == null || portraits.Count == 0)
                return null;

            return portraits[0];
        }
        public virtual Sprite GetPortrait(string portraitName)
        {
            return portraits.Find(x => x.name == portraitName);
        }
    }
}