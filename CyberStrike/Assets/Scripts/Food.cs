using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private const float Scaler = 0.01f;
    public int energy = 100;

    public List<Blob> blobs;

    public bool isHawkEating = false;
    public int hawkCount = 0;
    private void Awake()
    {
        blobs = new List<Blob>();
        EssManager.instance.FoodEatingEvent += FoodConsume;
    }
    private void FoodConsume()
    {
        isHawkEating = false;
        hawkCount = 0;
        foreach(var blob in blobs)
        {
            if (blob.GetType() == typeof(BlobHawk))
            {
                isHawkEating = true;
                hawkCount++;
            }
        }
        foreach (var blob in blobs)
        {
            blob.EatFood();
            if (isHawkEating && blob.GetType() == typeof(BlobDove)) continue;
            energy--;
            if (energy <= 0)
            {
                blob.FinishEating();
                EssManager.instance.FoodEatingEvent -= FoodConsume;
                Destroy(this);
                return;
            }
            transform.localScale = energy * Scaler * Vector3.one;
        }
    }
    public void BlobRegistration(Blob blob)
    {
        blobs.Add(blob);
    }
}
