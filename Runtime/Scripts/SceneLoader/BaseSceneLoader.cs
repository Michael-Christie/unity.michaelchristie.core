using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Core
{
    public abstract class BaseSceneLoader : MonoBehaviour
    { 
        private void Awake()
        {
            //Assign this to the boot loader
            CoreCallback.Instance.onSceneReady += OnSceneReady;
            CoreCallback.Instance.onSceneStart += OnSceneStart;
            CoreCallback.Instance.onSceneChange += OnSceneChange;
        }

        private void OnDestroy()
        {
            CoreCallback.Instance.onSceneReady -= OnSceneReady;
            CoreCallback.Instance.onSceneStart -= OnSceneStart;
            CoreCallback.Instance.onSceneChange -= OnSceneChange;
        }

        /// <summary>
        /// Called as soon as all the scens are loaded in from the boot loader.
        /// </summary>
        public abstract void OnSceneReady();

        /// <summary>
        /// Called after all scenes are loaded in from the boot loader.
        /// </summary>
        public abstract void OnSceneStart();

        /// <summary>
        /// Called when the scene has been kept from one scene to another
        /// </summary>
        public abstract void OnSceneChange();
    }
}
