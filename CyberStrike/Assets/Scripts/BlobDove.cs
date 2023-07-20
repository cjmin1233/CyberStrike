using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobDove : Blob
{
    public void QuitEating()
    {
        FinishEating();

    }
    protected override void StateEnter()
    {
        switch (curState)
        {
            case BlobState.Idle:
                break;
            case BlobState.Wandering:
                targetPos = transform.position;
                EssManager.instance.TickEvent += FoodFinding;
                break;
            case BlobState.FoodTracing:
                if(foundFood) targetPos = foundFood.transform.position;
                agent.SetDestination(targetPos);

                break;
            case BlobState.Eating:
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                foundFood?.BlobRegistration(this);
                //if(!foundFood) foundFood.BlobRegistration(this);
                break;
            default:
                break;
        }
    }


    protected override void StateUpdate()
    {
        switch (curState)
        {
            case BlobState.Idle:
                break;
            case BlobState.Wandering:
                if (Vector3.Distance(transform.position,
                    targetPos) < 0.1f)
                {
                    //var randCircle = Random.insideUnitCircle;
                    //targetPos = new Vector3(randCircle.x, 0f, randCircle.y) * WanderingRange;
                    targetPos = GetRandomPointOnNavmesh(transform.position);
                    agent.SetDestination(targetPos);
                }
                break;
            case BlobState.FoodTracing:                
                break;
            case BlobState.Eating:
                break;
            default:
                break;
        }
    }
    protected override void StateExit()
    {
        switch (curState)
        {
            case BlobState.Idle:
                break;
            case BlobState.Wandering:
                EssManager.instance.TickEvent -= FoodFinding;
                break;
            case BlobState.FoodTracing:
                break;
            case BlobState.Eating:
                agent.isStopped = false;
                break;
            default:
                break;
        }
    }

    protected override bool TransitionCheck()
    {
        switch (curState)
        {
            case BlobState.Idle:
                nextState = BlobState.Wandering;
                return true;
            case BlobState.Wandering:
                if (foundFood != null)
                {
                    nextState = BlobState.FoodTracing;
                    return true;
                }
                break;
            case BlobState.FoodTracing:
                if (!foundFood)
                {
                    nextState = BlobState.Wandering;
                    return true;
                }
                if (Vector3.Distance(transform.position,
                    targetPos) < 1f)
                {
                    nextState = BlobState.Eating;
                    return true;
                }
                break;
            case BlobState.Eating:
                if (foundFood == null)
                {
                    nextState = BlobState.Idle;
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EssManager.instance.TickEvent -= FoodFinding;
        EssManager.instance.doveCount--;
    }
}
