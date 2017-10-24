using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObjectPoolData
{
    public GameObject originalObject;
    public GameObject[] poolObjects;
}

public class ObjectPoller : MonoBehaviour
{
    public ObjectPoolData[] poolDatas;
    bool isPooled = false;

    //static oulText text = null;

    // Use this for initialization
    void Start()
    {
        //if (!text) text = GameObject.Find("Util/Text").GetComponent<oulText>();
        if (poolDatas.Length > 0) Pool();
    }

#if UNITY_EDITOR
    //void Update()
    //{
    //    // 稼働状況の表示
    //    for(int i=0; i<poolDatas.Length;i++)
    //    {
    //        text.Set(poolDatas[i].originalObject.name + ": " + GetActiveCount(i) + " / " + poolDatas[i].poolObjects.Length);
    //    }
    //}
#endif

    public GameObject GetPoolObject(int ID = 0, bool isRandom = false)
    {
        if (!isPooled) Pool();

        // 例外処理
        if (ID >= poolDatas.Length)
        {
            Debug.LogWarning("配列参照外です。無効な値が入力されているか、データのセットを忘れています。" + Environment.StackTrace);
        }
        //if (!isPooled)
        //Debug.LogWarning("オブジェクトがプールされていません。インスペクタで設定するか、スクリプトで設定した後にPool()関数を呼んでください");

        int[] indexArray;
        int numArray = poolDatas[ID].poolObjects.Length;


        if (isRandom)
        {
            indexArray = oulRandom.GetRandomArray(0, numArray);
        }
        else
        {
            indexArray = new int[numArray];
            for (int i = 0; i < numArray; i++)
            {
                indexArray[i] = i;
            }
        }

        // アクティブでないオブジェクトの検索
        for (int i = 0; i < numArray; i++)
        {
            if (!poolDatas[ID].poolObjects[indexArray[i]]) Debug.LogWarning("ぬるぽ" + ID + "," + i);
            // アクティブじゃないやつがいたらそいつをアクティブにして返す
            if (!poolDatas[ID].poolObjects[indexArray[i]].activeInHierarchy)
            {
                poolDatas[ID].poolObjects[indexArray[i]].SetActive(true);
                //Debug.Log(i);
                return poolDatas[ID].poolObjects[indexArray[i]];
            }
        }

        //if (isAllowIncrease)
        //{
        //    GameObject gameObject = (GameObject)Instantiate(pollObject);
        //    pollList.Add(gameObject);
        //    return gameObject;
        //}

        //Debug.LogWarning("プールオブジェクト数が足りてない！" + Environment.StackTrace);
        return null;
    }

    public bool Pool()
    {
        if (isPooled) return false;
        for (int i = 0; i < poolDatas.Length; i++)
        {
            for (int j = 0; j < poolDatas[i].poolObjects.Length; j++)
            {
                if(!poolDatas[i].originalObject)
                {
                    string trace = Environment.StackTrace;
                    Debug.LogWarning("Error ObjectPoller. originalObject is null." + trace);
                    return false;
                }
                poolDatas[i].poolObjects[j] = Instantiate(poolDatas[i].originalObject, transform);
                poolDatas[i].poolObjects[j].SetActive(false);
            }
        }
        isPooled = true;

        return true;
    }

    int GetActiveCount(int ID)
    {
        int count = 0;
        for(int i=0;i<poolDatas[ID].poolObjects.Length;i++)
        {
            if (poolDatas[ID].poolObjects[i].activeInHierarchy) count++;
        }
        return count;
    }
}
