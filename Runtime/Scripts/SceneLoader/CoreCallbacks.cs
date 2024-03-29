using System;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Core
{
    public class CoreCallbacks : ScriptableObject
    {
        public Action onSceneReady;
        public Action onSceneStart;
        public Action onSceneChange;
        public Action<bool, int> showLoadingScene;
        public Action<float> updateLoadPercentage;
    }

    public static class CoreCallback
    {
        private static CoreCallbacks _instance;
        public static CoreCallbacks Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<CoreCallbacks>(GameUtilities.CoreCallbackPath);

                    if(_instance == null)
                    {
                        Debug.LogError("Cannot find Core Callback Scriptable Object.");
                        return null;
                    }
                }

                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }
    }
}
