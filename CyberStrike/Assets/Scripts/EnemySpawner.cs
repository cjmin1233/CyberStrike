using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private GameObject[] enemies;
    private MultiQueue<GameObject> enemyQueue;

    private Coroutine enemySpawnProcess;
    [SerializeField] private float commonSpawnDuration;
    [SerializeField] private float eliteSpawnDuration;
    private float commonSpawnTimer;
    private float eliteSpawnTimer;
    public enum EnemyType
    {
        Slime,
        Beholder,
        Golem
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
    public void StartEnemySpawn()
    {
        enemySpawnProcess = StartCoroutine(EnemySpawnProcess());
    }
    public IEnumerator EnemySpawnProcess()
    {
        commonSpawnTimer = 0f;
        eliteSpawnTimer = 0f;

        while (true)
        {
            if (commonSpawnTimer >= commonSpawnDuration)
            {
                // 老馆 利 积己
            }
            if (eliteSpawnTimer >= eliteSpawnDuration)
            {
                // 郡府飘 利 积己
            }

            commonSpawnTimer += Time.deltaTime;
            eliteSpawnTimer += Time.deltaTime;
            yield return null;
        }
    }
}
