using System.Linq;
using UnityEngine;

public abstract class ReleaseConfig<T> : ScriptableObject where T : ScriptableObject
{
    const string resourcePath = "Assets/Resources";
    static T _instance = null;
    public static T Instance
    {
        get
        {
            if (!_instance)
                //_instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(); //will cause bug, only loaded assets can be find
                _instance = Resources.LoadAll<T>("").FirstOrDefault();

#if UNITY_EDITOR
            if (!_instance)
            {
                Debug.Log("<color=red>自動建立 " + typeof (T) + $" 於 {resourcePath} </color>");

                if(!UnityEditor.AssetDatabase.IsValidFolder(resourcePath))
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

                T asset = ScriptableObject.CreateInstance<T>();
                string path = resourcePath;
                string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
                UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);
                UnityEditor.AssetDatabase.SaveAssets();
                return asset;
            }
#endif
            return _instance;
        }
    }
}