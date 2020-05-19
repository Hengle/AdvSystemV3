using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorConfig<T> : ScriptableObject where T : ScriptableObject
{
    public static T Instance
    {
        get
        {
#if UNITY_EDITOR
            var results = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
            
            if(results.Length == 0){
                var astMenu = AdvUtility.GetDomainName<T, UnityEngine.CreateAssetMenuAttribute>();
                string tip = (astMenu == null)? "" : astMenu.menuName;
                Debug.LogError("<color=red>專案尚未建立 " + typeof (T) + $" (Create -> {tip})</color>");
                return null;
            } 
            foreach (string guid in results)
            {
                T temp = (T)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(guid), typeof(T));
                return temp;
            }
#endif
            return null;
        }
    }
}
