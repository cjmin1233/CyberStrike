using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] EnemyType[] commonEnemyTypes;
    [SerializeField] EnemyType[] eliteEnemyTypes;

    public Action<float> onDifficultyIncrease;
    private Action<float, float> enemyBoostAction;
    //private float enemyMoveSpeedBoost
    private float difficulty = 1f;
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
    public void Add2Pool(int index, GameObject instanceToAdd)
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
            if (commonSpawnTimer <= 0f)
            {
                // 일반 적 생성
                SpawnCommon();
            }
            if (eliteSpawnTimer <= 0f)
            {
                // 난이도 증가
                IncreaseDifficulty();
                // 엘리트 적 생성
                SpawnElite();
            }

            commonSpawnTimer -= Time.deltaTime;
            eliteSpawnTimer -= Time.deltaTime;
            yield return null;
        }
    }
    private void SpawnCommon()
    {
        commonSpawnTimer = commonSpawnDuration;
        EnemyType randType = commonEnemyTypes[Random.Range(0, commonEnemyTypes.Length)];

        SpawnEnemy((int)randType);
    }
    private void SpawnElite()
    {
        eliteSpawnTimer = eliteSpawnDuration;
        EnemyType randType = eliteEnemyTypes[Random.Range(0, eliteEnemyTypes.Length)];

        SpawnEnemy((int)randType);
    }
    private void SpawnEnemy(int index)
    {
        var instance = GetFromPool(index);
        instance.GetComponent<Enemy>().Setup(difficulty);
        instance.transform.position = NavMeshUtility.GetRandomPointOnNavmesh(Vector3.zero, 20f);
        instance.SetActive(true);
    }
    public void IncreaseDifficulty()
    {
        difficulty += .1f;

        onDifficultyIncrease(difficulty);
        print("난이도 증가!");
    }
    public void EnemyBoost()
    {
        enemyBoostAction(0.5f, 0.5f);
    }
}
