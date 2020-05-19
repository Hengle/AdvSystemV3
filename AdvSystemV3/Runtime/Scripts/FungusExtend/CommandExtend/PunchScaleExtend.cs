using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("iTween", 
                 "Punch Scale (Extend)", 
                 "Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale.")]
    [AddComponentMenu("")]
    public class PunchScaleExtend : PunchScale, ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}
        [SerializeField] protected AdvTargetObject _advTarget = AdvTargetObject.SelectedGameObject;

        public override void OnEnter(){
            if(_advTarget != AdvTargetObject.SelectedGameObject){

                _targetObject.Value = AdvManager.IO.GetAdvTargetObject(_advTarget);

                if(_advTarget == AdvTargetObject.SayIcon || _advTarget == AdvTargetObject.Dialog)
                    //Dialog Object never Wait
                     waitUntilFinished = false;
            }

            if (_targetObject.Value == null)
            {
                Debug.LogWarning("找不到選擇對象(_advTarget)! 確認指定的目標正確 ! : " + _advTarget.ToString());
            }

            base.OnEnter();
        }

        public override string GetSummary()
        {
            if(_advTarget != AdvTargetObject.SelectedGameObject)
            {
                return _advTarget.ToString();
            }
            if (_targetObject.Value == null)
            {
                return "Error: No target object selected";
            }

            return _targetObject.Value.name + " over " + _duration.Value + " seconds";
        }

        public void InitializeByParams(object[] param)
        {
            CommandParam data = param[0] as CommandParam;

            this._advTarget = AdvManager.IO.GetAdvTargetObjectByString(data.target);

            if (float.TryParse(data.arg1, out float val)){
                this._amount.Value = new Vector3(val, val, val);
            }
            if (float.TryParse(data.arg2, out float _time)){
                this._duration = new FloatData(_time);
            }
        }
    }
}