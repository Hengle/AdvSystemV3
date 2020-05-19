#if EA_EXTENTION
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;


namespace Fungus
{
    /// <summary>
    /// Plays a state of an animator according to the state name.
    /// </summary>
    [CommandInfo("Timeline", 
                 "Play Timeline", 
                 "只有選定 In Scene Target 為 SelectedGameObject時，才需要填入 Target 0")]
    [AddComponentMenu("")]
    public class PlayTimelineAsset : Command
    {
        [SerializeField] protected FungusExt.AdvTargetObject _inSceneTarget = FungusExt.AdvTargetObject.SelectedGameObject;

        [SerializeField] protected GameObject _target0;
        
        [HideInInspector]
        [Tooltip("Timeline Director want play !!!!")]
        [SerializeField] protected PlayableDirector playableDirectors;

        [Tooltip("Timeline Asset want play !!!!")]
        [SerializeField] protected TimelineAsset timelineAsset;

        [HideInInspector]
        [Tooltip("Timeline Binding Objects")]
        [SerializeField] protected GameObject bind1;

        [HideInInspector]
        [Tooltip("Timeline Binding Objects")]
        [SerializeField] protected GameObject bind2;

        [Tooltip("Is Waiting for Play finished")]
        [SerializeField] protected bool WaitForFinished;

        public override void OnEnter()
        {
            if(timelineAsset == null){
                Continue();
                return;
            }

            if(_inSceneTarget != FungusExt.AdvTargetObject.SelectedGameObject)
            {
                _target0 = AdvManager.Instance.GetAdvTargetObject(_inSceneTarget);
            }

            if(_target0 == null)
            {
                AdvUtility.LogWarning("> Timeline 指定的立繪已經不存在，因此跳過指令 ");
                Continue();
                return;
            }

            if(playableDirectors == null){
                PlayableDirector tempDirector = new GameObject("tempDirector").AddComponent<PlayableDirector>();
                tempDirector.stopped += delegate {
                    Destroy(tempDirector.gameObject);
                };

                playableDirectors = tempDirector;
            }
            playableDirectors.playableAsset = timelineAsset;
            playableDirectors.SetBindingGameObject("Target0", _target0);

            playableDirectors.Play(timelineAsset);

            if(WaitForFinished){
                playableDirectors.stopped += delegate {
                    Continue();
                };
            } else {
                Continue();
            }
        }

        
        public override string GetSummary()
        {
            if (timelineAsset == null)
            {
                return "Error: No Playable selected";
            }

            return timelineAsset.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(170, 204, 169, 255);
        }

        public void InitializeByParams(object[] param)
        {
            FungusExt.CommandParam data = param[0] as FungusExt.CommandParam;
            this._inSceneTarget = AdvUtility.GetAdvTargetObjectByString(data.target);

            AdvKeyContent ADVKeys = AdvKeyContent.GetCurrentInstance();
            if(ADVKeys != null){
                if(!string.IsNullOrEmpty(data.image)){
                    //從AdvKey 取得 Asset 資源
                    this.timelineAsset = ADVKeys.GetTimelineByKey(data.image);

                    //如果有填Key 但又找不到的話
                    if(this.timelineAsset == null){
                        AdvUtility.LogWarning("找不到Timeline檔:" + data.image + " , 於 行數 " + (this.itemId - 3));
                    }
                }
            }
        }
    }
}
#endif