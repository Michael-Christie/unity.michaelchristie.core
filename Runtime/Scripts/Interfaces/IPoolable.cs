using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    int PoolID { get; }

    bool IsInScene { get; set; }

    //
    void ReturnToPool();
}
