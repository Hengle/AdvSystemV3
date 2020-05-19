using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DialogMouseDetector : MonoBehaviour ,  IPointerDownHandler , IScrollHandler
{
    [Adv.HelpBox] public string tip = "Default has added advDialogInput.SetDialogClickedFlag.";
    public UnityEvent OnRaycastLeftClick;
    public UnityEvent OnRaycastRightClick;
    protected virtual void Start(){
        OnRaycastLeftClick.AddListener(AdvManager.Instance.advDialogInput.SetDialogClickedFlag);
    }

    public void OnPointerDown (PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            OnRaycastLeftClick.Invoke();
        }
        if (eventData.button == PointerEventData.InputButton.Right) {
            OnRaycastRightClick.Invoke();
            AdvManager.Instance.SwitchAdvUIShow();
        }
    }

    public void OnScroll(PointerEventData eventData){
        //Debug.Log(eventData.scrollDelta);
        if(eventData.scrollDelta.y < 0) {   // backwards
            OnRaycastLeftClick.Invoke();
        }
        if(eventData.scrollDelta.y > 0) {   // forward
            if(AdvManager.Instance != null){
                AdvManager.Instance.AdvMethod_Log();
            }
        }
    }
}