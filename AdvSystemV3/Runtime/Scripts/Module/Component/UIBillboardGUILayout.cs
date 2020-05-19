using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using DG.Tweening;

public class UIBillboardGUILayout : MonoBehaviour
{
    [Tooltip("List is : M OL LL L R RR OR")] public List<float> Horizon_posLocation;
    [Tooltip("List is : M Near Far")] public List<float> Vertical_posDistance;
    [Tooltip("List is : M Near Far")] public List<Vector2> Size_posDistance;

    public RectTransform [] billboardOnStage;

    public Transform BillboardPool;
    public Ease tweenEaseType;
    public float shiftPerUnit = 0.1f;

    float waitTimer;
    Stage stage;

    void Awake() {
        stage = GetComponentInParent<Stage>();
        InitBillboardOnStage();
    }

    protected void InitBillboardOnStage(){
        //Runtime時 , 紀錄哪個 Billboard 已經被使用
        billboardOnStage = new RectTransform[Enum.GetNames(typeof(BillboardPosition)).Length];
    }

    public RectTransform GetBillboardOnStage(BillboardPosition pos){
        //提取出哪個位置已存在的 Billboard by BillboardPosition
        return billboardOnStage[(int)pos];
    }

    public RectTransform GetBillboardOnStage(BillboardHideFlag pos){
        //提取出哪個位置已存在的 Billboard by BillboardHideFlag
        if(pos == BillboardHideFlag.Left)
            return billboardOnStage[(int)BillboardPosition.Left];
        else if(pos == BillboardHideFlag.Right)
            return billboardOnStage[(int)BillboardPosition.Right];
        else if(pos == BillboardHideFlag.Middle)
            return billboardOnStage[(int)BillboardPosition.Middle];
        else if(pos == BillboardHideFlag.OffscreenLeft)
            return billboardOnStage[(int)BillboardPosition.OffscreenLeft];
        else if(pos == BillboardHideFlag.OffscreenRight)
            return billboardOnStage[(int)BillboardPosition.OffscreenRight];
        else if(pos == BillboardHideFlag.LeftLeft)
            return billboardOnStage[(int)BillboardPosition.LeftLeft];
        else if(pos == BillboardHideFlag.RightRight)
            return billboardOnStage[(int)BillboardPosition.RightRight];
        return null;
    }

    public void SetBillboardOnStage(BillboardPosition pos, RectTransform obj){
        //Runtime時 , 設置哪個 Billboard 已經被使用 by 哪個 rt
        billboardOnStage[(int)pos] = obj;
    }

    public float GetPositionHorizen(BillboardPosition _position){
        return Horizon_posLocation[(int)_position];
    }
    public float GetPositionVertical(BillboardDistance _distance){
        return Vertical_posDistance[(int)_distance];
    }
    public Vector2 GetBillboardSize(BillboardDistance _distance){
        return Size_posDistance[(int)_distance];
    }

    /*
    public RectTransform GetPosition(BillboardDistance _distance, BillboardPosition _position){
        List<RectTransform> targetList = distanceHash[_distance];
        string targetString = "None";

            if(_position == BillboardPosition.Left)
            targetString = "Left";
        else if(_position == BillboardPosition.Right)
            targetString = "Right";
        else if(_position == BillboardPosition.Middle)
            targetString = "Middle";
        else if(_position == BillboardPosition.OffscreenLeft)
            targetString = "Offscreen Left";
        else if(_position == BillboardPosition.OffscreenRight)
            targetString = "Offscreen Right";
        else if(_position == BillboardPosition.LeftLeft)
            targetString = "LeftLeft";
        else if(_position == BillboardPosition.RightRight)
            targetString = "RightRight";

        foreach (var item in targetList)
        {
            if(item.gameObject.name == targetString){
                return item;
            }
        }

        return null;
    }

    public BillboardDistance GetDistanceFromRect(RectTransform obj){
        if(farPosition.Contains(obj))
            return BillboardDistance.Far;
        else if(middlePosition.Contains(obj))
            return BillboardDistance.Middle;
        else if(nearPosition.Contains(obj))
            return BillboardDistance.Near;

        return BillboardDistance.Middle;
    }
    */

    public virtual void RunBillboardGUIObjCommand(RectTransform Obj, BillboardOptions options, Action<GameObject> onPrepareSpawn, Action onComplete)
    {
        waitTimer = 0f;

        if (options.display == DisplayType.None)
        {
            onComplete();
            return;
        }

        //經由其他參數設置 from,to 相關訊息
        options = CleanBillboardOptions(options);
        options.onComplete = onComplete;

        switch (options.display)
        {
            case (DisplayType.Show):
                ShowBillboard(Obj, options, onPrepareSpawn);
                break;

            case (DisplayType.Hide):
                HideBillboard(options);
                break;

            case (DisplayType.Replace):
                ShowBillboard(Obj, options, onPrepareSpawn);
                break;

            case (DisplayType.MoveToFront):
                MoveToFrontBillboard(options);
                break;
        }
    }

    protected virtual BillboardOptions CleanBillboardOptions(BillboardOptions options)
    {
        // Use default stage settings
        if (options.useDefaultSettings)
        {
            options.fadeDuration = stage.FadeDuration;
            options.moveDuration = stage.MoveDuration;
            options.shiftOffset = stage.ShiftOffset;
        }

        // if portrait not moving, use from position is same as to position
        if (!options.move)
        {
            options.fromPosition = options.toPosition;
        }

        return options;
    }

    public virtual void ShowBillboard(RectTransform obj, BillboardOptions options, Action<GameObject> onPrepareSpawn)
    {
        //use: toPosition, fromPosition
        //RectTransform fromRT = stage.GetPosition(options.toDistance, options.fromPosition);
        //RectTransform toRT = GetPosition(options.toDistance, options.toPosition);

        Vector3 fromPos = new Vector3(GetPositionHorizen(options.fromPosition), GetPositionVertical(options.toDistance), 0);
        Vector3 toPos = new Vector3(GetPositionHorizen(options.toPosition), GetPositionVertical(options.toDistance), 0);

        //float horizon = GetPositionHorizen(options.toPosition);
        //float vertical = GetPositionVertical(options.toDistance);
        Vector2 size = GetBillboardSize(options.toDistance);

        //options.fromRect = fromRT;
        //options.toRect = toRT;


        //座標偏移移動
        if (options.shiftIntoPlace && options.move)
        {
            //複製一份TO, 以座標代替 From
            //options.fromRect = Instantiate(toRT) as RectTransform;
            //options.fromRect.anchoredPosition =
            //    new Vector2(options.fromRect.anchoredPosition.x + options.shiftOffset.x,
            //                options.fromRect.anchoredPosition.y + options.shiftOffset.y);
            fromPos = new Vector2(toPos.x + options.shiftOffset.x,
                            toPos.y + options.shiftOffset.y);
        }

        // LeanTween doesn't handle 0 duration properly
        float durationFade = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;
        // LeanTween doesn't handle 0 duration properly
        float durationMove = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

        // Fade out the existing portrait image
        RectTransform oldObj = GetBillboardOnStage(options.toPosition);
        if (oldObj != null)
        {
            CanvasGroup canvas = oldObj.GetComponent<CanvasGroup>();
            if(canvas != null)
                canvas.DOFade(0, durationFade).SetEase(tweenEaseType).OnComplete(() => {
                    Destroy(oldObj.gameObject);
                });
        }

        if(obj != null)
        {
            RectTransform copyObj = GameObject.Instantiate(obj, BillboardPool);
            copyObj.localPosition = fromPos;
            copyObj.localScale = size;
            copyObj.name = obj.name + "(Runtime)";
            onPrepareSpawn?.Invoke(copyObj.gameObject);

            if (options.flipFace == true)
                copyObj.localScale = new Vector3(-copyObj.localScale.x, copyObj.localScale.y, copyObj.localScale.z);

            CanvasGroup copyCanvas = copyObj.GetComponent<CanvasGroup>();
            if(copyCanvas == null)
                copyObj.gameObject.AddComponent<CanvasGroup>();

            copyCanvas.alpha = 0;

            copyCanvas.DOFade(1, durationFade).SetEase(tweenEaseType);

            toPos = ShiftPosition(toPos, options);
            copyObj.DOLocalMove(toPos, durationMove).SetEase(tweenEaseType);


            if (options.waitUntilFinished)
                waitTimer = durationMove;

            //Save Upload billboard to stage
            SetBillboardOnStage(options.toPosition, copyObj);

            //GameObject tempGO = GameObject.Instantiate(fromRT.gameObject);
            //tempGO.transform.SetParent(stage.BillboardPool.transform, false);
            //tempGO.transform.localPosition = Vector3.zero;
            //tempGO.transform.localScale = fromRT.localScale;
            //tempGO.name = options.billboardSprite.name;

            //Image tempImage = tempGO.GetComponent<Image>();
            //tempImage.sprite = options.billboardSprite;
            //tempImage.preserveAspect = true;
            //tempImage.color = new Color(1f, 1f, 1f, 0f);

            //BillboardData tempBillboard = tempGO.AddComponent<BillboardData>();
            //tempBillboard.SetData(tempImage);

            //Setup From position and Flip
            

            //Do Alpha Tween
            //LeanTween.alpha(tempImage.rectTransform, 1f, durationFade).setEase(stage.FadeEaseType).setRecursive(false);

            //Do Move Tween
            //CleanBillboardOptions(options);
            // LeanTween.move uses the anchoredPosition, so all position images must have the same anchor position
            //LeanTween.move(tempGO, options.toRect.position, durationMove).setEase(stage.FadeEaseType);
            
        }

        //Call Contunue with Wait until Finished
        FinishCommand(options);
    }
    public virtual void HideBillboard(BillboardOptions options)
    {
        List<RectTransform> workGroup = new List<RectTransform>();

        // Fade out the existing portrait image
        if(options.hideWhich != BillboardHideFlag.All){
            RectTransform oldObj = GetBillboardOnStage(options.hideWhich);
            workGroup.Add(oldObj);

        } else {
            foreach (var item in billboardOnStage)
            {
                if(item != null){
                    workGroup.Add(item);
                }
            }
        }

        foreach (var item in workGroup)
        {
            HideRectransform(item, options);
        }

        FinishCommand(options);
    }

    public virtual void MoveToFrontBillboard(BillboardOptions options)
    {
        RectTransform obj = GetBillboardOnStage(options.toPosition);

        obj.transform.SetSiblingIndex(obj.transform.parent.childCount);
        //Debug.Log(fromRT + ", to :" + fromRT.transform.parent.childCount + "," + options.toPosition);
        FinishCommand(options);
    }

    protected virtual void HideRectransform(RectTransform obj, BillboardOptions options){
        if (obj != null)
        {
            // LeanTween doesn't handle 0 duration properly
            float durationFade = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;
            // LeanTween doesn't handle 0 duration properly
            float durationMove = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

            CanvasGroup canvas = obj.GetComponent<CanvasGroup>();
            if(canvas != null)
                canvas.DOFade(0, durationFade).SetEase(tweenEaseType).OnComplete(() => {
                    if(obj != null)
                        Destroy(obj.gameObject);
                });

            //Vector3 targetPos = options.toRect.position;
            Vector3 toPos = new Vector3(GetPositionHorizen(options.toPosition), GetPositionVertical(options.toDistance), 0);
            toPos.y = obj.localPosition.y;
            
            if(options.move == false){
                toPos = obj.localPosition;
                durationMove = durationFade;
            }

            obj.DOLocalMove(toPos, durationMove).SetEase(tweenEaseType).OnComplete(() => {
                if(obj != null)
                    Destroy(obj.gameObject);
            });

            if (options.waitUntilFinished)
            {
                waitTimer = durationMove;
            }
        }
    }

    protected virtual void FinishCommand(BillboardOptions options)
    {
        if (options.onComplete != null)
        {
            if (!options.waitUntilFinished)
            {
                options.onComplete();
            }
            else
            {
                StartCoroutine(WaitUntilFinished(options.fadeDuration, options.onComplete));
            }
        }
        else
        {
            StartCoroutine(WaitUntilFinished(options.fadeDuration));
        }
    }

    protected virtual IEnumerator WaitUntilFinished(float duration, Action onComplete = null)
    {
        // Wait until the timer has expired
        // Any method can modify this timer variable to delay continuing.
        waitTimer = duration;
        while (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            yield return null;
        }

        // Wait until next frame just to be safe
        yield return new WaitForEndOfFrame();

        if (onComplete != null)
        {
            onComplete();
        }
    }
    
    public Vector3 ShiftPosition(Vector3 src, BillboardOptions options){
        Vector3 pos = src;
        int amount = options.shiftAmount;
        switch(options.toPositionShift)
        {
            case ToPositionShift.Up:
                pos += new Vector3(0, shiftPerUnit * amount, 0);
                break;
            case ToPositionShift.Down:
                pos += new Vector3(0, -shiftPerUnit * amount, 0);
                break;
            case ToPositionShift.Left:
                pos += new Vector3(-shiftPerUnit * amount, 0, 0);
                break;
            case ToPositionShift.Right:
                pos += new Vector3(shiftPerUnit * amount, 0, 0);
                break;
        }
        return pos;
    }

    public void ImmediateClearAllBillboard(){
        //Clear Background , and Billboard
        foreach (var item in billboardOnStage)
        {
            if(item != null){
                Destroy(item.gameObject);
            }
        }
    }

    public UIBillboardController FindBillboardWithTerm(string term){
        foreach (var item in billboardOnStage)
        {
            if(item == null)
                continue;
            UIBillboardController uiData = item.GetComponent<UIBillboardController>();
            if(uiData == null)
                continue;
            
            if(uiData.DataTerm == term){
                return uiData;
            }
        }
        return null;
    }
}
