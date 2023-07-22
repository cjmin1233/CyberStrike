using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class EssManager : MonoBehaviour
{
    public static Vector3 GetRandomPointOnNavmesh(Vector3 origin, float radius)
    {
        Vector3 randomPoint = Vector3.zero;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(origin + Random.insideUnitSphere * radius, out hit, radius, NavMesh.AllAreas))
        {
            randomPoint = hit.position;
        }
        randomPoint.y = 0f;
        return randomPoint;
    }

    public static EssManager instance;
    
    public GameObject blobDove;
    public GameObject blobHawk;
    public GameObject food;

    private int numOfDove = 1;
    private int numOfHawk = 1;

    public List<Transform> foodList;

    public delegate void TickRate();
    public event TickRate TickEvent;
    public event TickRate FoodEatingEvent;
    public event TickRate FoodConsumingEvent;

    private const float width = 50f;

    [SerializeField] private Slider DoveNumberSlider;
    [SerializeField] private Slider HawkNumberSlider;
    [SerializeField] private TextMeshProUGUI DoveNumText;
    [SerializeField] private TextMeshProUGUI HawkNumText;

    [SerializeField] private Slider DoveGraph;
    [SerializeField] private Slider HawkGraph;

    public int doveCount { get; set; }
    public int hawkCount { get; set; }
    private void Awake()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(gameObject);
        
        foodList = new List<Transform>();
        doveCount = 0;
        hawkCount = 0;
    }
    //public Vector3 NearestFood(Vector3 pos)
    //{
    //    print("returns nearest food");
    //    if (foodList.Count == 0) return Vector3.zero;

    //    var minPos = foodList[0].position;
    //    foreach(var _food in foodList)
    //    {
    //        if (Vector3.Distance(pos, _food.position) <
    //            Vector3.Distance(pos, minPos)) minPos = _food.position;
    //    }

    //    return minPos;
    //}
    private IEnumerator Timer()
    {
        while (true)
        {
            var xrand = Random.Range(-width, width);
            var zrand = Random.Range(-width, width);
            var foodObj = Instantiate(food, new Vector3(xrand, 0f, zrand), Quaternion.identity);

            foodList.Add(foodObj.transform);

            if (TickEvent != null) TickEvent();
            if (FoodConsumingEvent != null) FoodConsumingEvent();

            UpdateBlobSlider();
            yield return new WaitForSeconds(1f);
        }
    }
    private IEnumerator FoodEatingTimer()
    {
        while (true)
        {
            if (FoodEatingEvent != null) FoodEatingEvent();
             yield return new WaitForSeconds(.1f);
        }

    }
    private void UpdateBlobSlider()
    {
        var totalBlobs = doveCount + hawkCount;
        if (totalBlobs <= 0) return;
        //print(totalBlobs + "," + doveCount + "," + hawkCount);

        var doveRatio = (float)doveCount / totalBlobs;
        var hawkRatio = (float)hawkCount / totalBlobs;
        DoveGraph.value = doveRatio;
        HawkGraph.value = hawkRatio;
    }
    public void SetNumOfDove(float count)
    {
        numOfDove = (int)count;
        DoveNumText.text = ((int)count).ToString();
    }
    public void SetNumOfHawk(float count)
    {
        numOfHawk = (int)count;
        HawkNumText.text = ((int)count).ToString();
    }
    public void StartSimulation()
    {
        for (int i = 0; i < numOfDove; i++)
        {
            var randPos = GetRandomPointOnNavmesh(Vector3.zero, 10f);
            Instantiate(blobDove,
                randPos,
                Quaternion.identity);
        }
        for (int i = 0; i < numOfHawk; i++)
        {
            var randPos = GetRandomPointOnNavmesh(Vector3.zero, 10f);
            Instantiate(blobHawk,
                randPos,
                Quaternion.identity);
        }
        StartCoroutine(Timer());
        StartCoroutine(FoodEatingTimer());
    }
    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }
}
