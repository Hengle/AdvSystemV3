using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

public static class WifeEditorUtility
{

    /// <summary>
    /// Create new asset from <see cref="ScriptableObject"/> type with unique name at
    /// selected folder in project window. Asset creation can be cancelled by pressing
    /// escape key when asset is initially being named.
    /// </summary>
    /// <typeparam name="T">Type of scriptable object.</typeparam>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
    }

    //Prefab Editor Helper
    public static bool isInPrefabStage()
    {
#if UNITY_2018_3_OR_NEWER
        var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        return stage != null;
#else
        return false;
#endif
    }

    public static void UpdatePrefab()
    {
#if UNITY_EDITOR
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
#endif
    }
}