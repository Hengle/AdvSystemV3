using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class AdvLogLayout : MonoBehaviour
{
    public RecyclingListViewItem LogContentPrefab;
    public Button buttonBack;
    [HideInInspector] public ScrollRect ScrollRectObject;

    CanvasGroup _canvasGroup;

    public CanvasGroup CanvasGroupUsed { get {return _canvasGroup; }}
    public bool IsOpen { get{ return _canvasGroup.blocksRaycasts; }}
    public bool IsScrollBottom { get { return isScrollBottom; }}


    bool isScrollBottom;
    bool isTweening;
    bool isOpened;
    float fadeDuration = 0.5f;
    bool fading = false;

    void Awake(){
        _canvasGroup = GetComponent<CanvasGroup>();
        ScrollRectObject = GetComponentInChildren<ScrollRect>();
    }

    void Start(){
        CloseSelf();
        StartCoroutine(ScrollBottomChecker());
        if(AdvManager.Instance != null){
            AdvManager.Instance.lastLogLayout = this;
        }
    }

    IEnumerator ScrollBottomChecker(){
        while(true){
            if(ScrollRectObject.normalizedPosition.y <= 0.001){
                isScrollBottom = true;
            }
            else {
                isScrollBottom = false;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    public void ScrollToBottom(){
        ScrollRectObject.normalizedPosition = new Vector2(0, 0);
    }

    void Update() {
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if(IsOpen && IsScrollBottom)
                CloseSelf();
        }
        if (Input.GetMouseButtonDown(1)){
            if(IsOpen)
                CloseSelf();
        }
    }

    void OnEnable() {
        buttonBack.onClick.AddListener (CloseSelf);
    }

    void OnDisable() {
        buttonBack.onClick.RemoveAllListeners();
    }

    public void OpenSelf(){
        if(_canvasGroup == null)
            return;
        
        if(fading)
            return;
        
        fading = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1, fadeDuration).OnComplete(()=>{
            _canvasGroup.alpha = 1;
            fading = false;
        });
    }
    
    public void CloseSelf(){
        if(_canvasGroup == null)
            return;

        if(fading)
            return;
        
        fading = true;
        _canvasGroup.DOFade(0, fadeDuration).OnComplete(()=>{
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
            fading = false;
        });
    }

    //Old Version , will create many Log content
    // [Space(20)] public int MaxLogContent = 100;
    // public List<AdvLogContentLayout> LogContents;
    // public AdvLogContentLayout CreateAdvLogContent(){
    //     if(ScrollContainer == null || PrefabAdvLogContent == null)
    //         return null;
        
    //     GameObject ob = Instantiate(PrefabAdvLogContent, Vector3.zero, Quaternion.identity, ScrollContainer.transform);
    //     AdvLogContentLayout content = ob.GetComponent<AdvLogContentLayout>();
    //     LogContents.Add(content);

    //     if(LogContents.Count > MaxLogContent){
    //         Destroy(LogContents[0].gameObject);
    //         LogContents.RemoveAt(0);
    //     }

    //     return content;
    // }

    // [ContextMenu("Refresh Log UI Content")]
    // public void RefreshLogContentUI(SystemLanguage targetLanguage){
    //     foreach (AdvLogContentLayout item in LogContents)
    //     {
    //         item.UpdateUIContent(targetLanguage);
    //     }
    // }

}
