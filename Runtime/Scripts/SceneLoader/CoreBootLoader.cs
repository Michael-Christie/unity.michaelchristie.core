using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MC.Core
{
    public class CoreBootLoader : MonoBehaviour
    {
        public static CoreBootLoader Instance { get; private set; }

        [SerializeField] private SceneCollection[] sceneCollections;

        private int currentCollection = 0;

        private List<AsyncOperation> currentAsynList = new List<AsyncOperation>();

        private float totalProgress = 0;

        //
        #region Game Start Functions
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            StartCoroutine(OnApplicationStart());
        }

        private IEnumerator OnApplicationStart()
        {
            yield return GameUtilities.WaitTimers.waitForOneSecond;

            for (int i = 0; i < sceneCollections[0].scenes.Length; i++)
            {
                currentAsynList.Add(SceneManager.LoadSceneAsync(sceneCollections[0].scenes[i], LoadSceneMode.Additive));
            }

            for (int i = 0; i < currentAsynList.Count; i++)
            {
                while (!currentAsynList[i].isDone)
                {
                    totalProgress = 0;
                    foreach (AsyncOperation _operation in currentAsynList)
                    {
                        totalProgress += _operation.progress;
                    }

                    totalProgress /= currentAsynList.Count;

                    CoreCallback.Instance.updateLoadPercentage?.Invoke(totalProgress);

                    yield return GameUtilities.WaitTimers.waitForEndFrame;
                }
            }

            yield return GameUtilities.WaitTimers.waitForPointFive;

            CoreCallback.Instance.showLoadingScene?.Invoke(false, 0);
            yield return GameUtilities.WaitTimers.waitForPointFive;
            currentAsynList.Clear();

            currentCollection = 0;
        }
        #endregion


        #region Base Loading Functions
        public void ChangeSceneCollection(int _sceneCollection)
        {
            CoreCallback.Instance.updateLoadPercentage?.Invoke(0);

            StartCoroutine(SwitchSceneCollection(_sceneCollection));
        }

        private IEnumerator SwitchSceneCollection(int _sceneCollection)
        {
            //Hide the current Scene
            CoreCallback.Instance.showLoadingScene?.Invoke(true, _sceneCollection);
            yield return GameUtilities.WaitTimers.waitForPointFive;

            Application.backgroundLoadingPriority = ThreadPriority.High;

            //Unload the current scene collection
            for (int i = 0; i < sceneCollections[currentCollection].scenes.Length; i++)
            {
                //If the scene is already loaded, don't load it again
                if (!sceneCollections[_sceneCollection].Contains(sceneCollections[currentCollection].scenes[i]))
                {
                    currentAsynList.Add(SceneManager.UnloadSceneAsync(sceneCollections[currentCollection].scenes[i]));
                }
                else
                {
                    OnSceneChange();
                }
            }

            for (int i = 0; i < sceneCollections[_sceneCollection].scenes.Length; i++)
            {
                //Don't laod a scene in if its already loaded
                if (!sceneCollections[currentCollection].Contains(sceneCollections[_sceneCollection].scenes[i]))
                {
                    currentAsynList.Add(SceneManager.LoadSceneAsync(sceneCollections[_sceneCollection].scenes[i], LoadSceneMode.Additive));
                }
            }

            for (int i = 0; i < currentAsynList.Count; i++)
            {
                while (!currentAsynList[i].isDone)
                {
                    totalProgress = 0;
                    foreach (AsyncOperation _operation in currentAsynList)
                    {
                        totalProgress += _operation.progress;
                    }

                    totalProgress /= currentAsynList.Count;

                    CoreCallback.Instance.updateLoadPercentage?.Invoke(totalProgress);

                    yield return GameUtilities.WaitTimers.waitForEndFrame;
                }
            }

            currentCollection = _sceneCollection;

            //Call the scene functions
            OnSceneReady();

            yield return GameUtilities.WaitTimers.waitForPointFive;
            OnSceneStart();

            Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;

            CoreCallback.Instance.showLoadingScene?.Invoke(false, _sceneCollection);
            yield return GameUtilities.WaitTimers.waitForPointFive;

            currentAsynList.Clear();
        }

        public void AddScene(int _sceneIndex)
        {
            SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Additive);
        }

        public void RemoveScene(int _sceneIndex)
        {
            SceneManager.UnloadSceneAsync(_sceneIndex);
        }

        private void OnSceneReady()
        {
            CoreCallback.Instance.onSceneReady?.Invoke();
        }

        private void OnSceneStart()
        {
            CoreCallback.Instance.onSceneStart?.Invoke();
        }

        private void OnSceneChange()
        {
            CoreCallback.Instance.onSceneChange?.Invoke();
        }
        #endregion
    }

    public static class BootLoaderExtention
    {
        public static bool Contains(this SceneCollection _collection, int _scene)
        {
            for (int i = 0; i < _collection.scenes.Length; i++)
            {
                if (_collection.scenes[i] == _scene)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
