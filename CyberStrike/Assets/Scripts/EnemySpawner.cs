using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private GameObject[] enemies;
    private MultiQueue<GameObject> enemyQueue;
    public enum EnemyType
    {
        Slime,
        Beholder
    }
    private void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        int enumLength = Enum.GetValues(typeof(EnemyType)).Length;
        enemyQueue = new MultiQueue<GameObject>(enumLength);
    }
    private void GrowPool(int index)
    {
        for(int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(enemies[index]);
            instanceToAdd.transform.SetParent(transform);
            Add2Pool(index, instanceToAdd);
        }
    }

    public  void Add2Pool(int index, GameObject instanceToAdd)
    {
        instanceToAdd.SetActive(false);
        enemyQueue.Enqueue(index, instanceToAdd);
    }
    public GameObject GetFromPool(int index)
    {
        if (enemyQueue.Count(index) <= 0) GrowPool(index);
        var instance = enemyQueue.Dequeue(index);

        return instance;
    }
}
