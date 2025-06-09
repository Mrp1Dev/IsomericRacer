using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IOwnedByPlayer
{
    [SerializeField] private float MaxHealth;
    public float Health { get; private set; }
    public event Action<float> OnDamageTaken;
    public event Action<PlayerInstance> OnDeathByPossiblePlayer;

    private PlayerInstance lastDamageDealer;
    public bool Dead { get; private set; } = false;
    public PlayerInstance OwnerPlayer { get; set; }

    private void Awake()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (Dead) return;
        Health -= damage;
        if (Health < 0)
        {
            Health = 0.0f;
            PerformDeath();
        }
        OnDamageTaken?.Invoke(damage);
    }

    public void TakeDamage(float damage, PlayerInstance damageDealer)
    {
        if (Dead) return;
        lastDamageDealer = damageDealer;
        TakeDamage(damage);
    }

    private void PerformDeath()
    {
        Dead = true;
        OwnerPlayer.DestroyCharacter();
        OnDeathByPossiblePlayer?.Invoke(lastDamageDealer);
    }

    public float GetHealthPercentage() => Health / MaxHealth;
}
