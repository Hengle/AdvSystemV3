using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;

public class UIForegroundLayout : MonoBehaviour
{
    [SerializeField] protected Image fillMask;
    [SerializeField, AssetList(AutoPopulate = true, Path = "Plugins/AdvSystemV3/Runtime/Shaders")] protected Material[] materials;
    [SerializeField, Range(0, 1f), OnValueChanged("OnFillValueChanged")] protected float _fillValue;
    public float fillValue { get { return _fillValue; } set { OnFillValueChanged(_fillValue = value); } }


    public void SetupMaterial(string materialName, Texture2D masktexture, Color color, float rotation)
    {
        fillMask.material = materials.Where(t => t.name == materialName).First();
        fillMask.material.SetTexture("_MaskTexture", masktexture);
        fillMask.material.SetColor("_Color", color);
        fillMask.material.SetFloat("_Rotation", rotation);
    }

    // Material FindMaterial(string name)
    // {
    //     return materials.Where( t => t.name == name).First();
    // }

    void OnFillValueChanged(float v)
    {
        fillMask.material.SetFloat("_Threshold", v);
    }

    void OnFillValueChanged()
    {
        OnFillValueChanged(_fillValue);
    }
}
