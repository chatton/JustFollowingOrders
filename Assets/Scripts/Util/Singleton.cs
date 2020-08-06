using UnityEngine;

namespace Util
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject();
                        T t = go.AddComponent<T>();
                        _instance = t;
                    }
                }

                if (_instance == null)
                {
                    Debug.LogError("Singleton<" + typeof(T) + "> instance has been not found.");
                }
                
                // DontDestroyOnLoad(_instance);
                return _instance;
            }
        }
    }
}