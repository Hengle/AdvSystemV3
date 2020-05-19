using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonOnOffSprite : MonoBehaviour
{
    public Sprite SpriteOn;
    public Sprite SpriteOff;
    Image _image;

    public bool isRecieveStopAuto = false;
    public bool isRecieveStopSkip = false;
    public Color color { get => _image.color; set => _image.color = value; }

    void Start() {
        _image = GetComponent<Image>();
    }

    void OnEnable() {
        RecieveStopAuto(true);
        RecieveStopSkip(true);
    }

    void OnDisable() {
        RecieveStopAuto(false);
        RecieveStopSkip(false);
    }

    public void SetSpriteOnOff(bool turn){
        if(turn)
            _image.sprite = SpriteOn;
        else
            _image.sprite = SpriteOff;
    }

    public void RecieveStopAuto(bool on){
        if(isRecieveStopAuto == false)
            return;

        if(on)
            AdvSignals.AdvStopAutoWrite += BTNOFF;
        else
            AdvSignals.AdvStopAutoWrite -= BTNOFF;
    }

    public void RecieveStopSkip(bool on){
        if(isRecieveStopSkip == false)
            return;

        if(on)
            AdvSignals.AdvStopAutoSkip += BTNOFF;
        else
            AdvSignals.AdvStopAutoSkip -= BTNOFF;
    }

    void BTNOFF(){
        _image.sprite = SpriteOff;
    }
}
