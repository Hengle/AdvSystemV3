using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Fungus
{
    [CommandInfo("Animation", "Play Trainsition", "播放指定的轉場效果")]
    [AddComponentMenu("")]
    public class PlayTransitions : Command
    {
        string[] animationTypes = { "ColorFillMask", "TextureAlphaMask" };
        [SerializeField, ValueDropdown("animationTypes")] protected string animationType = "ColorFillMask";
        [SerializeField, ShowIf("isNeedAlphaTexture")] protected Texture2D maskTexture;
        [SerializeField] protected bool invert;
        [SerializeField] protected Color color = Color.black;
        [SerializeField] protected float duration = 1;
        [SerializeField] protected float delay = 0.1f;

        [SerializeField, Range(0.01f, 360f)] float rotation = 0.01f;

        // bool IsExitAdv = false;
        // public override void OnStopExecuting()
        // {
        //     IsExitAdv = true;
        // }

        void PerpareData()
        {
            float rotation = this.rotation * Mathf.PI / 180;
            AdvManager.instance.advStage.ForegroundLayout.SetupMaterial(animationType, maskTexture, color, rotation);
        }

        public override void OnEnter()
        {
            PerpareData();
            var value = AdvManager.instance.advStage.ForegroundLayout.fillValue = invert ? 1 : 0;
            var tagetValue = invert ? 0 : 1;
            DOTween.To(() => value
                        , v => AdvManager.instance.advStage.ForegroundLayout.fillValue = v
                        , tagetValue, duration).OnComplete(() => Invoke("Continue", delay));
        }
        public override Color GetButtonColor()
        {
            return new Color32(170, 204, 169, 255);
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Large)]
        void PlayPreview()
        {
            PerpareData();
            UnityEditor.EditorApplication.update += UpdateAni;
            // Debug.Log(AdvManager.instance);
        }
        float time = 0;
        void UpdateAni()
        {
            time += Time.deltaTime * 1 / duration;
            if (time >= 1)
            {
                UnityEditor.EditorApplication.update -= UpdateAni;
                time = 0;
                return;
            }
            AdvManager.instance.advStage.ForegroundLayout.fillValue = invert ? 1 - time : time;
        }

        bool isNeedAlphaTexture()
        {
            return animationType == "TextureAlphaMask";
        }
#endif

    }

}

