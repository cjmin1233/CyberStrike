using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUtility : MonoBehaviour
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
}
