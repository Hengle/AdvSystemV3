using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using DG.Tweening;
using SpriteDicing;

public class UIBillboardLayout : MonoBehaviour
{
    public Stage stage;

    [Header("1 的 Order 給 Sprite Dicing 使用, 排序以 Z 軸決定")]
    public List<GameObject> farPosition;
    public List<GameObject> middlePosition;
    public List<GameObject> nearPosition;

    public List<GameObject> farEnemyPosition;
    public List<GameObject> middleEnemyPosition;
    public List<GameObject> nearEnemyPosition;

    public Ease tweenEaseType;

    public Color beBackgroundColor;

    public float shiftPerUnit = 0.1f;
    

    [HideInInspector]
    public GameObject[] OnStageBillboard;
    private Dictionary<BillboardDistance,List<GameObject>> distanceHash;
    private Dictionary<BillboardDistance,List<GameObject>> distanceEnemyHash;


    
    float sortZ = 0;
    float sortStepValue = 0.01f;
    float waitTimer;

    void Start(){
        // 2019.07.10 目前共7個位置 , 左外, 左左, 左, 中, 右, 右右, 右外
        OnStageBillboard = new GameObject[Enum.GetNames(typeof(BillboardPosition)).Length];

        distanceHash = new Dictionary<BillboardDistance, List<GameObject>>();
        distanceEnemyHash = new Dictionary<BillboardDistance, List<GameObject>>();

        distanceHash[BillboardDistance.Far] = farPosition;
        distanceHash[BillboardDistance.Middle] = middlePosition;
        distanceHash[BillboardDistance.Near] = nearPosition;

        distanceEnemyHash[BillboardDistance.Far] = farEnemyPosition;
        distanceEnemyHash[BillboardDistance.Middle] = middleEnemyPosition;
        distanceEnemyHash[BillboardDistance.Near] = nearEnemyPosition;
    }

    public GameObject GetBillboardOnStage(BillboardPosition pos){
        //提取出哪個位置已存在的 Billboard by BillboardPosition
        return OnStageBillboard[(int)pos];
    }

    public GameObject GetBillboardOnStage(BillboardHideFlag pos){
        BillboardPosition map_pos = 0;
        switch(pos)
        {
            case BillboardHideFlag.OffscreenLeft:
                map_pos = BillboardPosition.OffscreenLeft;
                break;
            case BillboardHideFlag.LeftLeft:
                map_pos = BillboardPosition.LeftLeft;
                break;
            case BillboardHideFlag.Left:
                map_pos = BillboardPosition.Left;
                break;
            case BillboardHideFlag.Middle:
                map_pos = BillboardPosition.Middle;
                break;
            case BillboardHideFlag.Right:
                map_pos = BillboardPosition.Right;
                break;
            case BillboardHideFlag.RightRight:
                map_pos = BillboardPosition.RightRight;
                break;
            case BillboardHideFlag.OffscreenRight:
                map_pos = BillboardPosition.OffscreenRight;
                break;
            default:
                return null;
        }
        return GetBillboardOnStage(map_pos);
    }

    public void FadeOutAllBillboard(){
        foreach (var item in OnStageBillboard)
        {
            if(item != null){
                DicedSpriteRenderer oldRect = item.GetComponent<DicedSpriteRenderer>();
                if(oldRect != null){
                    DOTween.To(()=> oldRect.Color, x => oldRect.Color = x, new Color(1, 1, 1, 0), stage.FadeDuration).SetEase(tweenEaseType).OnComplete(() => {
                        Destroy(oldRect.gameObject);
                    });
                }
                SpriteRenderer oldRectEnemy = item.GetComponent<SpriteRenderer>();
                if(oldRectEnemy != null){
                    DOTween.To(()=> oldRectEnemy.color, x => oldRectEnemy.color = x, new Color(1, 1, 1, 0), stage.FadeDuration).SetEase(tweenEaseType).OnComplete(() => {
                        Destroy(oldRectEnemy.gameObject);
                    });
                }
            }
        }
        sortZ = 0;
    }

    public void ImmediateClearAllBillboard(){
        foreach (var item in OnStageBillboard)
        {
            if(item != null){
                Destroy(item);
            }
        }
        sortZ = 0;
    }

    public BillboardController FindBillboardWithTerm(string term){
        foreach (var item in OnStageBillboard)
        {
            if(item == null)
                continue;
            BillboardController atlasData = item.GetComponent<BillboardController>();
            if(atlasData == null)
                continue;
            if(atlasData.BelongAtlas == null)
                continue;
            
            if(atlasData.BelongAtlas.characterTerm == term){
                return atlasData;
            }
        }
        return null;
    }

    public BillboardDistance FindObjectInDistance(GameObject obj){
        // SpriteRender 是給 Enemy 用的
        if(obj.GetComponent<SpriteRenderer>() != null){
            if(farEnemyPosition.Contains(obj))
            return BillboardDistance.Far;
        else if(middleEnemyPosition.Contains(obj))
            return BillboardDistance.Middle;
        else if(nearEnemyPosition.Contains(obj))
            return BillboardDistance.Near;
        }
        // 反之 其他 是給 角色立繪 用的
        else {
            if(farPosition.Contains(obj))
            return BillboardDistance.Far;
        else if(middlePosition.Contains(obj))
            return BillboardDistance.Middle;
        else if(nearPosition.Contains(obj))
            return BillboardDistance.Near;
        }

        return BillboardDistance.Middle;
    }

    public void SetBillboardOnStage(BillboardPosition pos, GameObject rt){
        //Runtime時 , 設置哪個 Billboard 已經被使用 by 哪個 rt
        OnStageBillboard[(int)pos] = rt;
    }

    public GameObject GetPosition(BillboardDistance _distance, BillboardPosition _position){
        List<GameObject> targetList = distanceHash[_distance];
        string targetString = "None";

        switch(_position)
        {
            case BillboardPosition.OffscreenLeft:
                targetString = "OffscreenLeft";
                break;
            case BillboardPosition.LeftLeft:
                targetString = "LeftLeft";
                break;
            case BillboardPosition.Left:
                targetString = "Left";
                break;
            case BillboardPosition.Middle:
                targetString = "Middle";
                break;
            case BillboardPosition.Right:
                targetString = "Right";
                break;
            case BillboardPosition.RightRight:
                targetString = "RightRight";
                break;
            case BillboardPosition.OffscreenRight:
                targetString = "OffscreenRight";
                break;
            default:
                return null;
        }

        foreach (var item in targetList)
        {
            if(item.gameObject.name == targetString){
                return item;
            }
        }

        return null;
    }

    public GameObject GetPositionEnemy(BillboardDistance _distance, BillboardPosition _position){
        List<GameObject> targetList = distanceEnemyHash[_distance];
        string targetString = "None";

        switch(_position)
        {
            case BillboardPosition.OffscreenLeft:
                targetString = "OffscreenLeft";
                break;
            case BillboardPosition.LeftLeft:
                targetString = "LeftLeft";
                break;
            case BillboardPosition.Left:
                targetString = "Left";
                break;
            case BillboardPosition.Middle:
                targetString = "Middle";
                break;
            case BillboardPosition.Right:
                targetString = "Right";
                break;
            case BillboardPosition.RightRight:
                targetString = "RightRight";
                break;
            case BillboardPosition.OffscreenRight:
                targetString = "OffscreenRight";
                break;
            default:
                return null;
        }

        foreach (var item in targetList)
        {
            if(item.gameObject.name == targetString){
                return item;
            }
        }

        return null;
    }

    public void PopUpBillboard(BillboardController src){
        foreach (var item in OnStageBillboard)
        {
            if(item == null)
                continue;

            BillboardController _ctrl = item.GetComponent<BillboardController>();
            if(_ctrl == null)
                continue;
            
            if(src == _ctrl){
                IncreaseSortLayer();
                _ctrl.SetToForeground(sortZ);
            } else {
                _ctrl.SetToBackground(beBackgroundColor);
            }
        }
    }

    public void ResumeBillboardPop(){
        foreach (var item in OnStageBillboard)
        {
            if(item == null)
                continue;
            
            BillboardController _ctrl = item.GetComponent<BillboardController>();
            if(_ctrl == null)
                continue;
            
            IncreaseSortLayer();
            _ctrl.SetToForeground(sortZ);
        }
    }

    public void IncreaseSortLayer(){
        sortZ -= sortStepValue;
    }

    public virtual void RunBillboardCommand(BillboardOptions options, Action onComplete){
        waitTimer = 0f;

        if (options.display == DisplayType.None)
        {
            onComplete();
            return;
        }

        //經由其他參數設置 from,to 相關訊息 , onComplete 預設是帶入 Continue 涵式
        options = CleanBillboardOptions(options);
        options.onComplete = onComplete;

        //以Sprite 是不是 null 來決定是不是怪物圖

        switch (options.display)
        {
            case (DisplayType.Show):
                if(options.billboardSprite == null)
                    ShowBillboard(options);
                else
                    ShowBillboardEnemy(options);
                break;

            case (DisplayType.Hide):
                HideBillboard(options);
                break;

            case (DisplayType.Replace):
                if(options.billboardSprite == null)
                    ShowBillboard(options);
                else
                    ShowBillboardEnemy(options);
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

    public virtual void ShowBillboard(BillboardOptions options)
    {
        //正常情況下，擷取 來自位置 以及 目標位置
        GameObject fromOb = GetPosition(options.toDistance, options.fromPosition);
        GameObject toOb = GetPosition(options.toDistance, options.toPosition);

        // LeanTween doesn't handle 0 duration properly
        float durationFade = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;
        // LeanTween doesn't handle 0 duration properly
        float durationMove = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

        //淡出已存在於位置上的舊立繪
        GameObject oldObj = GetBillboardOnStage(options.toPosition);
        if (oldObj != null)
        {
            DicedSpriteRenderer oldRect = oldObj.GetComponent<DicedSpriteRenderer>();
            if(oldRect != null){
                DOTween.To(()=> oldRect.Color, x => oldRect.Color = x, new Color(1, 1, 1, 0), durationFade).SetEase(tweenEaseType).OnComplete(() => {
                    Destroy(oldRect.gameObject);
                });
            }
            SpriteRenderer oldRectEnemy = oldObj.GetComponent<SpriteRenderer>();
            if(oldRectEnemy != null){
                DOTween.To(()=> oldRectEnemy.color, x => oldRectEnemy.color = x, new Color(1, 1, 1, 0), durationFade).SetEase(tweenEaseType).OnComplete(() => {
                    Destroy(oldRectEnemy.gameObject);
                });
            }
        }

        //新立繪進來
        if(options.billboardDiceSprite != null)
        {
            //以'來自位置'取出 Stage 上的 位置 ， 並以此當作新的立繪存在於場景
            GameObject tempGO = null;

            //當選用座標偏移移動時，取代掉 from來自位置
            if (options.shiftIntoPlace && options.move)
            {
                //複製一份TO, 用以代替 from 來進行 offset 滑入
                tempGO = Instantiate(toOb) as GameObject;
                tempGO.transform.position =
                    new Vector2(tempGO.transform.position.x + options.shiftOffset.x,
                                tempGO.transform.position.y + options.shiftOffset.y);
            } else {
                //一般狀況, 使用 from 訊息
                tempGO = GameObject.Instantiate(fromOb);
                if (!options.move)
                {
                    tempGO.transform.position = ShiftPosition(tempGO.transform.position, options);
                }
            }
            
            tempGO.transform.SetParent(transform, false);
            //tempGO.transform.localPosition = Vector3.zero;
            //tempGO.transform.localScale = fromRT.transform.localScale;
            tempGO.name = options.billboardDiceSprite.name;
            //啟用物件，複製出來時預設為關閉
            tempGO.SetActive(true);
            //設置SortOrder, 以Z 軸來 sort
            IncreaseSortLayer();
            tempGO.transform.position += new Vector3(0, 0, sortZ);

            //設置立繪圖案，一開始給透明
            DicedSpriteRenderer tempImage = tempGO.GetComponent<DicedSpriteRenderer>();
            tempImage.DicedSprite = options.billboardDiceSprite;
            tempImage.Color = new Color(1f, 1f, 1f, 0f);

            //設置 Atlas
            BillboardController tempAtlas = tempGO.GetComponent<BillboardController>();
            tempAtlas.BelongAtlas = options.billboardDiceAtlas;
            if(tempAtlas.BelongAtlas == null){
                Debug.Log("動態立繪 Atlas 設置失敗, 以純立繪使用");
            }

            //處理翻轉選項
            if (options.flipFace == true)
            {
                tempGO.transform.localScale = new Vector3(-tempGO.transform.localScale.x, tempGO.transform.localScale.y, tempGO.transform.localScale.z);
            }

            //Do Alpha Tween 淡入場璟
            //LeanTween.alpha(tempGO, 1f, durationFade).setEase(stage.FadeEaseType).setRecursive(false);
            DOTween.To(()=> tempImage.Color, x => tempImage.Color = x, new Color(1, 1, 1, 1), durationFade).SetEase(tweenEaseType);

            //Do Move Tween 滑入場景
            //LeanTween.move(tempGO, toOb.transform.position, durationMove).setEase(stage.FadeEaseType);
            Vector3 posTo = toOb.transform.position + new Vector3(0, 0, sortZ);
            posTo = ShiftPosition(posTo, options);
            tempGO.transform.DOMove(posTo, durationMove).SetEase(tweenEaseType);
            if (options.waitUntilFinished)
            {
                waitTimer = Mathf.Max(durationMove, durationFade);
            }

            //Save Upload billboard to stage
            SetBillboardOnStage(options.toPosition, tempGO);
        }

        //Call Contunue with Wait until Finished
        FinishCommand(options);
    }
    public virtual void HideBillboard(BillboardOptions options)
    {
        List<GameObject> workGroup = new List<GameObject>();

        // Fade out the existing portrait image
        if(options.hideWhich != BillboardHideFlag.All){
            GameObject oldRect = GetBillboardOnStage(options.hideWhich);
            workGroup.Add(oldRect);

        } else {
            foreach (var item in OnStageBillboard)
            {
                if(item != null){
                    workGroup.Add(item);
                }
            }
        }

        foreach (var item in workGroup)
        {
            //淡出Billboard 至 相對位置
            HideObject(item, options);
        }

        workGroup.Clear();
        workGroup = null;

        FinishCommand(options);
    }

    public virtual void MoveToFrontBillboard(BillboardOptions options)
    {
        GameObject fromObj = GetBillboardOnStage(options.toPosition);

        //fromObj.transform.SetSiblingIndex(fromObj.transform.parent.childCount);
        IncreaseSortLayer();
        fromObj.transform.position = new Vector3(fromObj.transform.position.x, fromObj.transform.position.y, sortZ);
        FinishCommand(options);
    }

    protected virtual void HideObject(GameObject obj, BillboardOptions options){

        if (obj != null)
        {
            //先找出這個Billboard 在哪個距離段, 再找出要淡出的位置
            //TODO: 這個設計不好
            options.toDistance = FindObjectInDistance(obj);

            // LeanTween doesn't handle 0 duration properly
            float durationFade = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;
            // LeanTween doesn't handle 0 duration properly
            float durationMove = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

            GameObject toPos = null;

            DicedSpriteRenderer objRenderDice = obj.GetComponent<DicedSpriteRenderer>();
            SpriteRenderer objRender = obj.GetComponent<SpriteRenderer>();
            if(objRenderDice != null){
                toPos = GetPosition(options.toDistance, options.toPosition);

                //淡出圖案
                DOTween.To(()=> objRenderDice.Color, x => objRenderDice.Color = x, new Color(1, 1, 1, 0), durationFade).SetEase(tweenEaseType).OnComplete(() => {
                    if(objRenderDice != null)
                        Destroy(objRenderDice.gameObject);
                });
            }
            if(objRender != null){
                toPos = GetPositionEnemy(options.toDistance, options.toPosition);

                //淡出圖案
                DOTween.To(()=> objRender.color, x => objRender.color = x, new Color(1, 1, 1, 0), durationFade).SetEase(tweenEaseType).OnComplete(() => {
                    if(objRender != null)
                        Destroy(objRender.gameObject);
                });
            }

            Vector3 targetPos = new Vector3(toPos.transform.position.x, obj.transform.position.y,  toPos.transform.position.z);
            
            //如果不勾選移動的話, 目標位置就是自己位置
            if(options.move == false){
                targetPos = obj.transform.position;
            }

            obj.transform.DOMove(targetPos + new Vector3(0, 0, obj.transform.position.z), durationMove).SetEase(tweenEaseType);

            if (options.waitUntilFinished)
            {
                waitTimer = Mathf.Max(durationMove, durationFade);
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
                StartCoroutine(WaitUntilFinished(waitTimer, options.onComplete));
            }
        }
        else
        {
            StartCoroutine(WaitUntilFinished(waitTimer));
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

    public virtual void ShowBillboardEnemy(BillboardOptions options)
    {
        //正常情況下，擷取 來自位置 以及 目標位置
        GameObject fromOb = GetPositionEnemy(options.toDistance, options.fromPosition);
        GameObject toOb = GetPositionEnemy(options.toDistance, options.toPosition);

        // LeanTween doesn't handle 0 duration properly
        float durationFade = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;
        // LeanTween doesn't handle 0 duration properly
        float durationMove = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

        //淡出已存在於位置上的舊立繪
        GameObject oldObj = GetBillboardOnStage(options.toPosition);
        if (oldObj != null)
        {
            DicedSpriteRenderer oldRect = oldObj.GetComponent<DicedSpriteRenderer>();
            if(oldRect != null){
                DOTween.To(()=> oldRect.Color, x => oldRect.Color = x, new Color(1, 1, 1, 0), durationFade).SetEase(tweenEaseType).OnComplete(() => {
                    Destroy(oldRect.gameObject);
                });
            }
            SpriteRenderer oldRectEnemy = oldObj.GetComponent<SpriteRenderer>();
            if(oldRectEnemy != null){
                DOTween.To(()=> oldRectEnemy.color, x => oldRectEnemy.color = x, new Color(1, 1, 1, 0), durationFade).SetEase(tweenEaseType).OnComplete(() => {
                    Destroy(oldRectEnemy.gameObject);
                });
            }
        }

        //新Enemy進來
        if(options.billboardSprite != null)
        {
            //以'來自位置'取出 Stage 上的 位置 ， 並以此當作新的立繪存在於場景
            GameObject tempGO = null;

            //當選用座標偏移移動時，取代掉 from來自位置
            if (options.shiftIntoPlace && options.move)
            {
                //複製一份TO, 用以代替 from 來進行 offset 滑入
                tempGO = Instantiate(toOb) as GameObject;
                tempGO.transform.position =
                    new Vector2(tempGO.transform.position.x + options.shiftOffset.x,
                                tempGO.transform.position.y + options.shiftOffset.y);
            } else {
                //一般狀況, 使用 from 訊息
                tempGO = GameObject.Instantiate(fromOb);
            }
            
            tempGO.transform.SetParent(transform, false);
            tempGO.name = options.billboardSprite.name;
            tempGO.SetActive(true);
            //設置SortOrder, 以Z 軸來 sort
            IncreaseSortLayer();
            tempGO.transform.position += new Vector3(0, 0, sortZ);

            //設置立繪圖案，一開始給透明
            SpriteRenderer tempImage = tempGO.GetComponent<SpriteRenderer>();
            tempImage.sprite = options.billboardSprite;
            tempImage.color = new Color(1f, 1f, 1f, 0f);

            //處理翻轉選項
            if (options.flipFace == true)
            {
                tempGO.transform.localScale = new Vector3(-tempGO.transform.localScale.x, tempGO.transform.localScale.y, tempGO.transform.localScale.z);
            }

            //Do Alpha Tween 淡入場璟
            DOTween.To(()=> tempImage.color, x => tempImage.color = x, new Color(1, 1, 1, 1), durationFade).SetEase(tweenEaseType);

            //Do Move Tween 滑入場景
            tempGO.transform.DOMove(toOb.transform.position + new Vector3(0, 0, sortZ), durationMove).SetEase(tweenEaseType);
            if (options.waitUntilFinished)
            {
                waitTimer = Mathf.Max(durationMove, durationFade);
            }

            //Save Upload billboard to stage
            SetBillboardOnStage(options.toPosition, tempGO);
        }

        //Call Contunue with Wait until Finished
        FinishCommand(options);
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
}
