using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BlobState
{
    Idle,
    Wandering,
    FoodTracing,
    Eating
}

public abstract class Blob : MonoBehaviour
{
    protected static Vector3 GetRandomPointOnNavmesh(Vector3 origin)
    {
        Vector3 randomPoint = Vector3.zero;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(origin + Random.insideUnitSphere * WanderingRange, out hit, WanderingRange, NavMesh.AllAreas))
        {
            randomPoint = hit.position;
        }
        randomPoint.y = 0f;
        return randomPoint;
    }
    private const float Scaler = 0.01f;
    private const float MinScale = 0.5f;

    protected NavMeshAgent agent;
    [SerializeField] protected LayerMask foodLayer;
    protected Food foundFood;

    protected int energy = 50;

    protected BlobState curState = BlobState.Idle;
    protected BlobState nextState = BlobState.Idle;
    protected const float WanderingRange = 10f;
    protected Vector3 targetPos;
    protected bool isStateChanged = true;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        EssManager.instance.FoodConsumingEvent += FoodConsume;

        if (this is BlobDove) EssManager.instance.doveCount++;
        else if (this is BlobHawk) EssManager.instance.hawkCount++;
    }
    protected virtual void Update()
    {
        transform.localScale = Vector3.one * (energy * Scaler + MinScale);

        if (energy >= 100)
        {
            Duplication();
        }
        curState = nextState;

        if (isStateChanged) StateEnter();
        isStateChanged = false;

        StateUpdate();

        isStateChanged = TransitionCheck();
        if (isStateChanged) StateExit();
    }
    protected abstract void StateEnter();
    protected abstract void StateUpdate();
    protected abstract void StateExit();
    protected abstract bool TransitionCheck();
    protected void FoodFinding()
    {
        RaycastHit[] hits =
        Physics.SphereCastAll(transform.position,
        WanderingRange, Vector3.up, WanderingRange, foodLayer);

        Debug.DrawRay(transform.position,
            transform.forward * WanderingRange, Color.red, 1f);

        if (hits.Length > 0)
        {
            foreach (var _ in hits)
            {
                var food = _.transform.GetComponent<Food>();
                if (!food) continue;
                // dove는 hawk가 먹고 있으면 노터치
                if (GetType() == typeof(BlobDove) && food.hawkCount > 0) foundFood = null;
                else
                {
                    foundFood = food;
                    break;
                }
            }
            //var food = hits[0].transform.GetComponent<Food>();
            //if (!food) return;
            //// dove는 hawk가 먹고 있으면 노터치
            //if (GetType() == typeof(BlobDove) && food.hawkCount > 0) foundFood = null;
            //else foundFood = food;
        }
        else foundFood = null;
    }
    public void EatFood()
    {
        //if (this.GetType() == typeof(BlobDove) && foundFood.isHawkEating)
        //{
        //    FinishEating();
        //    return;
        //}
        if (this is BlobHawk && foundFood.hawkCount >= 2) return;
        energy++;
    }
    public void FinishEating()
    {
        foundFood = null;
    }
    private void FoodConsume()
    {
        energy-=1;
        if (energy <= 0)
        {
            Destroy(gameObject);
        }
    }
    protected virtual void OnDestroy()
    {
        EssManager.instance.FoodConsumingEvent -= FoodConsume;
    }
    private void Duplication()
    {
        //var rand = Random.Range(0f, Mathf.PI * 2f);
        //var rand_x = Mathf.Cos(rand);
        //var rand_y = Mathf.Sin(rand);

        //var randPos = new Vector3(rand_x, 0f, rand_y) * WanderingRange;
        var randPos = GetRandomPointOnNavmesh(transform.position);
        if (this.GetType() == typeof(BlobDove))
        {
            Instantiate(EssManager.instance.blobDove, 
                randPos,
                Quaternion.identity);
        }
        if (this.GetType() == typeof(BlobHawk))
        {
            Instantiate(EssManager.instance.blobHawk,
                randPos,
                Quaternion.identity);
        }
        energy -= 50;
    }
}
