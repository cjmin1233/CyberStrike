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
    }
    protected virtual void Update()
    {
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
            foundFood = hits[0].transform.GetComponent<Food>();
        }
        else foundFood = null;
    }
    public void EatFood()
    {
        energy++;
        transform.localScale = Vector3.one * (energy * Scaler + MinScale);
    }
    public void FinishEating()
    {
        foundFood = null;
    }
}
