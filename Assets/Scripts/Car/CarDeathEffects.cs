using UnityEngine;

public class CarDeathEffects : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] private AudioSource explosion;
    private void Awake()
    {
        healthSystem.OnDeathByPossiblePlayer += OnDeath;
    }

    private void OnDeath(PlayerInstance killer)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Instantiate(explosion);
    }
}
