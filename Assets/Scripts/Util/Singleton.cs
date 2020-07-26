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
                }

                if (_instance == null)
                {
                    Debug.LogError("Singleton<" + typeof(T) + "> instance has been not found.");
                }

                return _instance;
            }
        }
    }
}