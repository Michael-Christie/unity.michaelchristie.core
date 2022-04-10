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

        private List<BaseSceneLoader> baseSceneControllers = new List<BaseSceneLoader>();

        private int currentCollection = 0;

        private List<AsyncOperation> currentAsynList = new List<AsyncOperation>();

        private float totalProgress = 0;

        public Action<bool> ShowLoadingScene;
        public Action<float> UpdateLoadPercentage;

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

                    UpdateLoadPercentage?.Invoke(totalProgress);

                    yield return GameUtilities.WaitTimers.waitForEndFrame;
                }
            }

            yield return GameUtilities.WaitTimers.waitForPointFive;

            ShowLoadingScene?.Invoke(false);
            yield return GameUtilities.WaitTimers.waitForPointFive;
            currentAsynList.Clear();

            currentCollection = 0;
        }
        #endregion


        #region Base Loading Functions
        public void ChangeSceneCollection(int _sceneCollection)
        {
            UpdateLoadPercentage?.Invoke(0);

            StartCoroutine(SwitchSceneCollection(_sceneCollection));
        }

        private IEnumerator SwitchSceneCollection(int _sceneCollection)
        {
            //Hide the current Scene

            ShowLoadingScene?.Invoke(true);
            yield return GameUtilities.WaitTimers.waitForPointFive;

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

                    UpdateLoadPercentage?.Invoke(totalProgress);

                    yield return GameUtilities.WaitTimers.waitForEndFrame;
                }
            }

            currentCollection = _sceneCollection;

            //Call the scene functions
            OnSceneReady();

            yield return GameUtilities.WaitTimers.waitForPointFive;
            OnSceneStart();

            ShowLoadingScene?.Invoke(false);
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
            for (int i = 0; i < baseSceneControllers.Count; i++)
            {
                baseSceneControllers[i]?.OnSceneReady();
            }
        }

        private void OnSceneStart()
        {
            for (int i = 0; i < baseSceneControllers.Count; i++)
            {
                baseSceneControllers[i]?.OnSceneStart();
            }
        }

        private void OnSceneChange()
        {
            for (int i = 0; i < baseSceneControllers.Count; i++)
            {
                baseSceneControllers[i]?.OnSceneChange();
            }
        }
        #endregion

        public void AddActiveBaseSceneLoader(BaseSceneLoader _sceneLoader)
        {
            baseSceneControllers.Add(_sceneLoader);
        }

        public void RemoveBaseSceneLoader(BaseSceneLoader _sceneLoader)
        {
            baseSceneControllers.Remove(_sceneLoader);
        }
    }
}
