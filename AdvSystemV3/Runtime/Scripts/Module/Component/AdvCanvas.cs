using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]

[ExecuteInEditMode]
public class AdvCanvas : MonoBehaviour
{
    public int canvasPlaneDistance = 10;
    public bool ForceOrderInLayer = false;
    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector] public Canvas canvas;
    public float Alpha { get => canvasGroup.alpha; set => canvasGroup.alpha = value; }
    public bool Interactable { get => canvasGroup.blocksRaycasts; set => canvasGroup.blocksRaycasts = value; }
    public bool CanContrl { get => canvasGroup.enabled; set => canvasGroup.enabled = value; }
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = true;
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        if(!ForceOrderInLayer) canvas.sortingOrder = 110 - canvasPlaneDistance;
        canvas.planeDistance = canvasPlaneDistance;
    }

    void Start()
    {
        if(AdvUtility.IsInPrefabStage())
            canvas.worldCamera = Camera.main;

        ExitCanvas();
    }

    void OnEnable() {
        SceneManager.sceneLoaded += ReloadCatchMainCamera;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= ReloadCatchMainCamera;
    }

    void ReloadCatchMainCamera(Scene scene, LoadSceneMode mode){
        canvas.worldCamera = Camera.main;
    }

    public void ActiveCanvas()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
    }

    public void ExitCanvas()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }
}
