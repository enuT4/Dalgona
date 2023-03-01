using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPoolManager : MonoBehaviour
{
    [Serializable]
    public struct MemoryPoolItem
    {
        public GameObject poolObject;
        public Transform poolTr;
        public int itemNumber;
    }

    public MemoryPoolItem[] poolingList;

    List<GameObject[]> objectsList = new List<GameObject[]>();
    Dictionary<string, int> poolDictionary = new Dictionary<string, int>();
    

    public static MemoryPoolManager instance;

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < poolingList.Length; i++)
        {
            if (poolingList[i].poolTr == null)
            {
                GameObject parent = new GameObject();
                parent.transform.parent = this.gameObject.transform;
                parent.gameObject.name = poolingList[i].poolObject.gameObject.name;
                poolDictionary.Add(parent.gameObject.name, i);
                GameObject[] objects = new GameObject[poolingList[i].itemNumber];

                for (int j = 0; j < poolingList[i].itemNumber; j++)
                {
                    GameObject pool = Instantiate(poolingList[i].poolObject);
                    pool.transform.parent = parent.transform;
                    pool.gameObject.SetActive(false);
                    if (pool.TryGetComponent(out MemoryPoolObject ob))
                    {
                        ob.listIdx = i;
                        ob.itemNumber = j;
                        ob.InitObject();
                    }
                    objects[j] = pool;
                }

                objectsList.Add(objects);
            }
            else
            {
                GameObject[] objects = new GameObject[poolingList[i].itemNumber];
                poolDictionary.Add(poolingList[i].poolTr.gameObject.name, i);

                for (int j = 0; j < poolingList[i].itemNumber; j++)
                {
                    GameObject pool = Instantiate(poolingList[i].poolObject);
                    pool.transform.parent = poolingList[i].poolTr.transform;
                    pool.gameObject.SetActive(false);
                    if (pool.TryGetComponent(out MemoryPoolObject ob))
                    {
                        ob.listIdx = i;
                        ob.itemNumber = j;
                        ob.InitObject();
                    }
                    objects[j] = pool;
                }

                objectsList.Add(objects);
            }
        }
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
    }

    public void ObjectReturn(int idx, int itemNumber)
    {
        objectsList[idx][itemNumber].gameObject.SetActive(false);
    }

    public GameObject GetObject(string objectName)
    {
        if (poolDictionary.ContainsKey(objectName))
        {
            int idx = poolDictionary[objectName];
            for(int i = 0; i < objectsList[idx].Length; i++)
            {
                if(!objectsList[idx][i].gameObject.activeSelf)
                {
                    objectsList[idx][i].gameObject.SetActive(true);
                    return objectsList[idx][i].gameObject;
                }
            }
        }
        else
            return null;

        return null;
    }
}