using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ItemType
{
    HealthPack,
}
public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance { get; private set; }

    [SerializeField] private GameObject[] itemPrefabs;
    private MultiQueue<GameObject> itemQueue;

    private Coroutine itemSpawnProcess;

    [SerializeField] private float itemSpawnDuration;
    private float itemSpawnTimer;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        int enumLength = Enum.GetValues(typeof(ItemType)).Length;
        itemQueue = new MultiQueue<GameObject>(enumLength);
    }
    private void GrowPool(int index)
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(itemPrefabs[index]);
            instanceToAdd.transform.SetParent(transform);
            Add2Pool(index, instanceToAdd);
        }
    }
    public void Add2Pool(int index, GameObject instanceToAdd)
    {
        instanceToAdd.SetActive(false);
        itemQueue.Enqueue(index, instanceToAdd);
    }
    public GameObject GetFromPool(int index)
    {
        if (itemQueue.Count(index) <= 0) GrowPool(index);
        var instance = itemQueue.Dequeue(index);

        return instance;
    }
    public void StartItemSpawn()
    {
        itemSpawnProcess = StartCoroutine(ItemSpawnProcess());
    }

    private IEnumerator ItemSpawnProcess()
    {
        itemSpawnTimer = itemSpawnDuration;

        while (true)
        {
            if (itemSpawnTimer <= 0f)
            {
                SpawnItem();
            }
            itemSpawnTimer -= Time.deltaTime;
            yield return null;
        }
    }

    private void SpawnItem()
    {
        itemSpawnTimer = itemSpawnDuration;
        int randIndex = Random.Range(0, Enum.GetValues(typeof(ItemType)).Length);

        var instance = GetFromPool(randIndex);
        instance.transform.position = NavMeshUtility.GetRandomPointOnNavmesh(Vector3.zero, 20f);
        instance.SetActive(true);
    }
}
