using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssManager : MonoBehaviour
{
    public static EssManager instance;
    
    public GameObject blobDove;
    public GameObject blobHawk;
    public GameObject food;

    public List<Transform> foodList;

    public delegate void TickRate();
    public event TickRate TickEvent;
    public event TickRate FoodEatingEvent;
    public event TickRate FoodConsumingEvent;

    private const float width = 50f;
    private void Awake()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(gameObject);
        
        foodList = new List<Transform>();

        StartCoroutine(Timer());
        StartCoroutine(FoodEatingTimer());
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

}
