using System;
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
                if (_collection.scenes[i] == _scene)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Is T Equal to all of the critera passed in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_component"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        public static bool Equals<T>(this T _component, params T[] _criteria)
        {
            for (int i = 0; i < _criteria.Length; i++)
            {
                if (!_component.Equals(_criteria[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Does T equal one of the criteria passed to it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_component"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        public static bool Contains<T>(this T _component, params T[] _criteria)
        {
            for (int i = 0; i < _criteria.Length; i++)
            {
                if (_component.Equals(_criteria[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
