using System.Collections.Generic;
using UnityEngine;


public struct PoolInfo
{
    public string nameID;
    public int amount;
    public GameObject prefab;
    public Transform parentContainer;

    public Stack<GameObject> pool;

    public PoolInfo(int amount, GameObject prefab, Transform parentContainer)
    {
        this.nameID = prefab.name;
        this.amount = amount;
        this.prefab = prefab;
        this.parentContainer = parentContainer;
        pool = new Stack<GameObject>();
    }
}

/// <summary>
/// Singleton class capable of manage multiple Pool systems of different objects at the same time.
/// </summary>
public class Pools_Manager : MonoBehaviour
{
    #region Variables
    public static Pools_Manager Instance { get; private set; }

    HashSet<PoolInfo> _listOfPools = new HashSet<PoolInfo>();

    #endregion

    #region Unity Methods
    private void Awake()
    {
        /// Singleton Pattern
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// Given the Pool info or pool parameters, it will generate a new Pool or fill an existing one
    /// </summary>
    public void FillPool(PoolInfo info)
    {
        _listOfPools.Add(info);
        for (int i = 0; i < info.amount; i++)
        {
            GameObject objInstance = Instantiate(info.prefab, info.parentContainer);
            objInstance.name = info.nameID;
            PoolObject(objInstance);
        }
    }


    /// <summary>
    /// Give an object to be pooled
    /// </summary>
    public void PoolObject(GameObject go)
    {
        go.SetActive(false);

        PoolInfo selectedPool = GetPoolByID(go.name);

        if (!selectedPool.pool.Contains(go))
            selectedPool.pool.Push(go);
    }

    /// <summary>
    /// Returns a object unpooled or creates a new one if required
    /// </summary>
    public GameObject UnPoolObject(string objectID)
    {
        PoolInfo selectedPool = GetPoolByID(objectID);

        GameObject objInstance = null;
        /// Unpool object if pool is not empty
        if (selectedPool.pool.Count > 0)
        {
            objInstance = selectedPool.pool.Pop();
            objInstance.SetActive(true);
        }
        /// Else Instantiate a new Object 
        else
        {
            objInstance = Instantiate(selectedPool.prefab, selectedPool.parentContainer);
            objInstance.name = selectedPool.nameID;
        }

        return objInstance;
    }

    #endregion


    #region Private Methods
    /// <summary>
    /// Returns a Pool by the type requested
    /// </summary>
    private PoolInfo GetPoolByID(string objectID)
    {
        foreach (PoolInfo pool in _listOfPools)
        {
            if (pool.nameID.Equals(objectID))
                return pool;
        }
        return default(PoolInfo);
    }

    #endregion
}
