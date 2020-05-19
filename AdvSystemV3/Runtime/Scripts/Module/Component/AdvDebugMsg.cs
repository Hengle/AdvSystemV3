using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvDebugMsg : MonoBehaviour
{
    public static AdvDebugMsg instance;

    public Text text_Message;

    public Queue<string> MessageQueue = new Queue<string>();
    public int Max_Message = 30;
    
    void Awake() {
        instance = this;
        text_Message = GetComponent<Text>();
    }

    public static void AddMessage(string msg){
        if(instance == null)
            return;
        
        instance.MessageQueue.Enqueue(msg);
        if(instance.MessageQueue.Count > instance.Max_Message){
            instance.MessageQueue.Dequeue();
        }

        if(instance.text_Message == null)
            return;

        if(AdvManager.Instance.useDebugMsg == false)
            return;

        instance.text_Message.text = "";
        foreach (var item in instance.MessageQueue)
        {
            instance.text_Message.text += item + "\n";
        }
    }

    public static void ClearMessage(){
        if(instance == null)
            return;
        if(instance.text_Message == null)
            return;
        instance.MessageQueue.Clear();
        instance.text_Message.text = "";
    }
}
