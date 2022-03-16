using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Core
{
    [CreateAssetMenu(fileName = "SceneCollection")]
    public class SceneCollection : ScriptableObject
    {
        [SerializeField] private string title;

        public int[] scenes;
    }
}
