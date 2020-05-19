using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow", 
                 "Wait For Input", 
                 "等待直到點擊")]
    [AddComponentMenu("")]
    public class WaitForInput : Command
    {
        [SerializeField] protected EventType eventType = EventType.MouseDown;
        
        IEnumerator DoWaitForInput(){
            while(true){
                if(eventType == EventType.MouseDown){
                    if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)){
                        yield return null;
                        break;
                    }
                }
                if(!IsExecuting){
                    yield break;
                }
                yield return null;
            }
            Continue();
        }

        public override void OnEnter()
        {
            StartCoroutine(DoWaitForInput());
        }

        public override string GetSummary()
        {
            return eventType.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}