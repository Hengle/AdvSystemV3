using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvTestCanvasLayout : MonoBehaviour
{
    public List<Fungus.FlowchartExtend> allList;
    public Button BTNPreview;
    public Transform content;
    public List<Button> listAdvContentBTN;

    void Start()
    {
        //Get Adv
        foreach (var item in allList)
        {
            Color ButtonColor = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0.3f, 0.75f), 0.8f);
            //Get Block
            foreach (var block in item.GetComponents<Fungus.Block>())
            {
                Button temp = Instantiate(BTNPreview, content);
                temp.GetComponentInChildren<Text>().text = item.name + "." + block.BlockName;
                temp.GetComponent<Image>().color = ButtonColor;
                temp.onClick.AddListener(delegate{
                    PlayAdvContent(item, block.BlockName);
                    });
                listAdvContentBTN.Add(temp);
            }
        }
        FungusExt.LocalizeManager.SetLanguage (SystemLanguage.ChineseTraditional);
    }

    void PlayAdvContent(Fungus.FlowchartExtend content, string blockName){
        AdvManager.Instance.LoadContent(content, blockName, null);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.F1)){
            TurnSelf();
        }

        if(Input.GetKeyDown(KeyCode.F4)){
            if(AdvManager.Instance.templateFlowchart != null)
                AdvManager.Instance.templateFlowchart.StopAllBlocks();
            AdvManager.Instance.StopAdvScene();
        }
        if(Input.GetKeyDown(KeyCode.F5)){
            FungusExt.LocalizeManager.SetLanguage (SystemLanguage.ChineseTraditional);
        }
        if(Input.GetKeyDown(KeyCode.F6)){
            FungusExt.LocalizeManager.SetLanguage (SystemLanguage.English);
        }
        if(Input.GetKeyDown(KeyCode.F7)){
            FungusExt.LocalizeManager.SetLanguage (SystemLanguage.Japanese);
        }
        if(Input.GetKeyDown(KeyCode.F9)){
            AdvManager.Instance.useDebugMsg = !AdvManager.Instance.useDebugMsg;
        }
    }

    void TurnSelf(){
        CanvasGroup canvas = GetComponent<CanvasGroup>();
        bool src = canvas.interactable;
        canvas.alpha = (src == false) ? 1 : 0;
        canvas.interactable = (src == false) ? true : false ;
    }
}
