using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsManager : MonoBehaviour
{
    public static ObjectsManager instance;

    private const int PoolSize = 1024;

    private Dictionary<string, List<GameObject>> Pool = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Recycle the Used Obj
    /// </summary>
    /// <param name="obj">the gameobject should add</param>
    public void RecycleObj(GameObject obj)
    {
        Vector3 LocalPosition = obj.transform.position;
        obj.transform.SetParent(ObjectsManager.instance.transform);
        obj.transform.position = LocalPosition;
        obj.SetActive(false);
        #region if exist the list in pool, add object in list
        if (Pool.ContainsKey(obj.name))
        {
            if(Pool[obj.name].Count < PoolSize)
            {
                Pool[obj.name].Add(obj);
            }
        }
        #endregion 
        #region else, create new List in pool
        else
        {
            Pool.Add(obj.name, new List<GameObject>() { obj });
        }
        #endregion
    }

    public void RecycleObjAllChildren(GameObject parentobj)
    {
        for(;parentobj.transform.childCount > 0;)
        {
            var target = parentobj.transform.GetChild(0).gameObject;
            RecycleObj(target);
        }
    }
    /// <summary>
    /// Get the Target GameObject in ObjectPool; 
    /// WARNING: MUST INIT OBJ AFTER GetObj!!! 
    /// For the reason that GetObj don't remain the prebaf' information!!!
    /// </summary>
    /// <param name="prefab">the prefab obj</param>
    /// <returns></returns>
    public GameObject GetObj(GameObject prefab)
    {
        GameObject returnObj = null;
        #region if prefab is in pool, return it 
        if (Pool.ContainsKey(prefab.name))
        {
            if (Pool[prefab.name].Count > 0) 
            {
                returnObj = Pool[prefab.name][0];
                returnObj.SetActive(true);
                Pool[prefab.name].Remove(returnObj);
                return returnObj;
            }
        }
        #endregion
        returnObj = GameObject.Instantiate(prefab);
        returnObj.name = prefab.name;
        returnObj.SetActive(true);
        //should recycle when NOT USE:
        //RecycleObj(returnObj);
        //GetObj(returnObj);
        return returnObj;

    }
    /// <summary>
    /// ReLoad GetObj, Add param of Setting Obj Parent
    /// </summary>
    /// <param name="prefab">gameobject</param>
    /// <param name="parent">which should be set as prefab's parent</param>
    /// <returns></returns>
    public GameObject GetObj(GameObject prefab, Transform parent)
    {
        var returnObj = GetObj(prefab);
        Vector3 LocalPosition = returnObj.transform.position;
        returnObj.transform.SetParent(ObjectsManager.instance.transform);
        returnObj.transform.position = LocalPosition;
        return returnObj;
    }
    /// <summary>
    /// Return the Count of the targetObject list in pool;
    /// if not exist, return -1
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public int GetObjListCount(GameObject prefab)
    {
        if (Pool.ContainsKey(prefab.name))
        {
            return Pool[prefab.name].Count;
        }
        return -1;
    }

}
