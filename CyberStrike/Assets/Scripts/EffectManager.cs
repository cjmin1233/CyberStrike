using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager m_instance;
    private static GameObject container;
    public static EffectManager instance
    {
        get
        {
            if (!m_instance)
            {
                container = new GameObject();
                container.name = "EffectManager";
                m_instance = container.AddComponent(typeof(EffectManager)) as EffectManager;
                DontDestroyOnLoad(container);
            }
            return m_instance;
        }
    }
    private class MultiQueue<GameObject>
    {
        private Queue<GameObject>[] queues;

        public MultiQueue(int count)
        {
            queues = new Queue<GameObject>[count];
            for (int i = 0; i < count; i++) queues[i] = new Queue<GameObject>();
        }
        public void Enqueue(int index, GameObject item)
        {
            queues[index].Enqueue(item);
        }
        public GameObject Dequeue(int index)
        {
            return queues[index].Dequeue();
        }
        public int Count(int index)
        {
            return queues[index].Count;
        }
        public GameObject Peek(int index)
        {
            return queues[index].Peek();
        }
    }
    [SerializeField] GameObject[] effectPrefabs;
    private MultiQueue<GameObject> effectQueue;
    public enum EffectType
    {
        Common,
        Flesh,
        Line
    }
    //[SerializeField] ParticleSystem commonHitEffectPrefab;
    //[SerializeField] ParticleSystem fleshHitEffectPrefab;

    private void Awake()
    {
        if (!m_instance) m_instance = this;
        else if (m_instance != this) Destroy(gameObject);

        //
        effectQueue = new MultiQueue<GameObject>(effectPrefabs.Length);

        for (int i = 0; i < effectPrefabs.Length; i++) GrowPool(i);
    }
    private void GrowPool(int index)
    {
        for(int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(effectPrefabs[index]);
            instanceToAdd.transform.SetParent(transform);
            Add2Pool(index, instanceToAdd);
        }
    }
    public void Add2Pool(int index, GameObject instance)
    {
        instance.SetActive(false);
        effectQueue.Enqueue(index, instance);
    }
    public GameObject GetFromPool(int index)
    {
        if (effectQueue.Count(index) == 0) GrowPool(index);

        var instance = effectQueue.Dequeue(index);

        return instance;
    }
    public void PlayHitEffect(Vector3 point, Vector3 normal, EffectType effectType = EffectType.Common)
    {
        int index = (int)effectType;
        //var targetPrefab = effectPrefabs[index];
        //if (effectType == EffectType.Flesh) targetPrefab = fleshHitEffectPrefab;

        var effect = GetFromPool(index);
        effect.transform.SetPositionAndRotation(point, Quaternion.LookRotation(normal));
        effect.SetActive(true);

        effect.GetComponent<ParticleSystem>().Play();
    }
}
