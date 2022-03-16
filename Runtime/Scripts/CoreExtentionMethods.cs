using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Core
{
    public static class CoreExtentionMethods
    {
        public static bool Contains(this SceneCollection _collection, int _scene)
        {
            for (int i = 0; i < _collection.scenes.Length; i++)
            {
                if (_collection.scenes[i] == i)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
