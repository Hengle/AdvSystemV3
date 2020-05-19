using System;
using UnityEngine;

namespace SingletonTool
{
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static GameObject _gameObject;
        private static object _lock = new object();
        private static bool _dontDestroyOnLoad = false;
        //private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (!Application.isPlaying){
                    var ins = (T)FindObjectOfType(typeof(T));
                    if(ins == null) Debug.LogError("Scene need a " + typeof(T));
                    return ins;
                }

                lock (_lock)
                {
                    if(_instance == null) Debug.LogError("Scene need a " + typeof(T));
                    return _instance;
                }
            }
        }

        protected void MarkAsCrossSceneSingleton()
        {
            _dontDestroyOnLoad = true;
            DontDestroyOnLoad(_gameObject);
            Debug.Log("[Singleton] An instance of " + typeof(T) + "was marked as DontDestroyOnLoad.");
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                _gameObject = this.gameObject;
            }
            else if (_instance != this)
            {
                if (_dontDestroyOnLoad)
                {
                    Debug.Log("[Singleton] An instance of " + typeof(T) + "has created with DontDestroyOnLoad. Destroy the new one.");
                    Destroy(this);
                }
                else
                {
                    throw new InvalidOperationException("[Singleton] There should never be more than 1 singleton instance of " + typeof(T));
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (_dontDestroyOnLoad)
            {
                //_applicationIsQuitting = true;
            }
            else
            {
                _instance = null;
                _gameObject = null;
            }
        }
    }
}