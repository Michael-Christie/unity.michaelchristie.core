using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    int PoolID { get; }

    bool IsInScene { get; set; }

    //
    void ReturnToPool();
    void SetPosition(Transform _newParent);
    void SetPosition(Transform _newParent, Vector3 _position);
}
