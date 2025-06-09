using UnityEngine;

public class TriggerDamageDealer : MonoBehaviour, IOwnedByPlayer
{
    [SerializeField] private float damage;
    [SerializeField] private LayerMask targetLayers;
    private BoxCollider box;
    public PlayerInstance OwnerPlayer { get; set; }

    /*private void OnTriggerEnter(Collider other)
    {
        var health =  other.GetComponentInParent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damage, OwnerPlayer);
            Debug.Log("applied damage");
        }
    }*/

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        var size = box.size;
        size.x *= transform.lossyScale.x;
        size.y *= transform.lossyScale.y;
        size.z *= transform.lossyScale.z;
        var result = Physics.OverlapBox(transform.TransformPoint(box.center),  size/ 2.0f, transform.rotation, targetLayers);
        for (int i = 0; i < result.Length; i++)
        {
            var health = result[i].GetComponentInParent<HealthSystem>();
            if(health != null)
            {
                health.TakeDamage(damage, OwnerPlayer);
            }
        }

    }
}
