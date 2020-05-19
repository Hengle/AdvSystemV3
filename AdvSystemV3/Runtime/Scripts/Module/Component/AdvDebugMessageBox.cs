using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvDebugMessageBox : MonoBehaviour
{
    public static AdvDebugMessageBox instance;
    public TMP_InputField outputBox;

    private void Awake() {
        instance = this;
    }

    public static void AddMessage(string msg){
        if(instance == null)
            return;
        if(instance.outputBox == null)
            return;
        
        TMP_InputField box = instance.outputBox;

        box.text += msg + "\n";

    }

    public static void ClearMessage(){
        if(instance == null)
            return;
        if(instance.outputBox == null)
            return;

        instance.outputBox.text = "";
    }
}
