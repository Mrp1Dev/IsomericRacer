using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public event Action<PlayerInstance> OnCollected;

    public void Collect(PlayerInstance collectedBy)
    {
        OnCollected?.Invoke(collectedBy);
        Destroy(gameObject); 
    }
}
