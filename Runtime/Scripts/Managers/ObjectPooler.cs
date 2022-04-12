using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public struct PooledObjcts
    {
        public GameObject prefab;

        public IPoolable poolRef;

        public int amountToCreate;
    }

    public static ObjectPooler Instance { get; private set; }

    private Dictionary<int, List<IPoolable>> pooledDictionary = new Dictionary<int, List<IPoolable>>();

    [SerializeField] private PooledObjcts[] pooledObjects;

    private List<IPoolable> cachedList = new List<IPoolable>();

    //
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;
        
        //Doing this here so its creating objects with the scene loading to hide any lag.
        Create();
    }

    private void Create()
    {
        List<IPoolable> _creationList = new List<IPoolable>();
        for (int i = 0; i < pooledObjects.Length; i++)
        {
            if(pooledObjects[i].poolRef == null)
            {
                pooledObjects[i].poolRef = pooledObjects[i].prefab.GetComponent<IPoolable>();
            }

            for (int j = 0; j < pooledObjects[i].amountToCreate; j++)
            {
                _creationList.Add(Instantiate(pooledObjects[i].prefab, transform).GetComponent<IPoolable>());
            }

            pooledDictionary.Add(pooledObjects[i].poolRef.PoolID, _creationList);
            _creationList.Clear();
        }
    }

    /// <summary>
    /// Gets a pooled object for you
    /// </summary>
    /// <param name="_objectID">The ID of the pooled item you want</param>
    /// <returns>The IPoolable Interface</returns>
    public IPoolable GetObject(int _objectID)
    {
        if (!pooledDictionary.ContainsKey(_objectID))
        {
            //TODO make this add an object to the pooled Dictionary;
            PooledObjcts _foundObjects = pooledObjects.Single(_item => _item.poolRef.PoolID == _objectID);

            if (_foundObjects.prefab == null)
            {
                Debug.LogError($"Object Pooler : Object ID {_objectID} was not already created, and is not a pre defined object to pool. Please set it up as a pooledObject.");
                return null;
            }

            cachedList.Clear();
            IPoolable _newObjects = Instantiate(_foundObjects.prefab, transform).GetComponent<IPoolable>();
            cachedList.Add(_newObjects);

            _newObjects.IsInScene = true;

            pooledDictionary.Add(_objectID, cachedList);

            return cachedList[0];
        }

        cachedList = pooledDictionary[_objectID];
        for (int i = 0; i < cachedList.Count; i++)
        {
            if (!cachedList[i].IsInScene)
            {
                cachedList[i].IsInScene = true;

                return cachedList[i];
            }
        }

        PooledObjcts _foundObject = pooledObjects.Single(_item => _item.poolRef.PoolID == _objectID);
        IPoolable _newObject = Instantiate(_foundObject.prefab, transform).GetComponent<IPoolable>();

        _newObject.IsInScene = true;

        pooledDictionary[_objectID].Add(_newObject);

        return _newObject;
    }

    /// <summary>
    /// Removes all Pooled Objects from the scene.
    /// </summary>
    public void ClearAllPooledObjects()
    {
        foreach (KeyValuePair<int, List<IPoolable>> _values in pooledDictionary)
        {
            for (int i = 0; i < _values.Value.Count; i++)
            {
                _values.Value[i].ReturnToPool();
            }
        }
    }

    /// <summary>
    /// Removes all Pooled objects of an ID
    /// </summary>
    /// <param name="_objectID">ID of object to remove</param>
    public void ClearPooledObject(int _objectID)
    {
        for (int i = 0; i < pooledDictionary[_objectID].Count; i++)
        {
            pooledDictionary[_objectID][i].ReturnToPool();
        }
    }
}
