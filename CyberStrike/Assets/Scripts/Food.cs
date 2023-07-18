using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private const float Scaler = 0.01f;
    public int energy = 100;

    public List<Blob> blobs;

    private void Awake()
    {
        blobs = new List<Blob>();
        EssManager.instance.FoodEatingEvent += FoodConsume;
    }
    private void FoodConsume()
    {
        foreach (var blob in blobs)
        {
            blob.EatFood();
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
