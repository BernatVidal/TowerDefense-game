using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Abstract class that holds main logic of any IPoolable Pools Manager
/// </summary>
public abstract class Poolables_Manager : MonoBehaviour
{
    protected HashSet<IPoolable> unpooledObjects;
    protected abstract void FillPool();

    protected void Update()
    {
        foreach(IPoolable poolable in unpooledObjects)
        {
            poolable.DoStuff();
        }
    }

    /// <summary>
    /// Pool IPoolables at the end of frame, to avoid issues trying to Move it after despawning
    /// </summary>
    protected virtual IEnumerator PoolObject(IPoolable pool)
    {
        yield return new WaitForEndOfFrame();
    }

}
