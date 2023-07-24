using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshUtility : MonoBehaviour
{
    public static Vector3 GetRandomPointOnNavmesh(Vector3 origin, float radius, int areaMask = NavMesh.AllAreas)
    {
        Vector3 randomPoint = Vector3.zero;
        if (NavMesh.SamplePosition(origin + Random.insideUnitSphere * radius, out NavMeshHit hit, radius, areaMask))
        {
            randomPoint = hit.position;
        }
        randomPoint.y = 0f;
        return randomPoint;
    }
}
