using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("GeneralStats")]
    [SerializeField] int CurrentHealth;
    [SerializeField] int MaxHealth = 100;

    [SerializeField] bool isDead = false;

    [SerializeField] GameObject damageCounterPrefab;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        if (CurrentHealth <= 0)
        {
            isDead = true;
        }

    }

    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            CurrentHealth -= damage;

            // Spawn the damage counter
            SpawnDamageCounter(damage);

            if (CurrentHealth <= 0)
            {
                isDead = true;
            }
        }
    }

    void SpawnDamageCounter(int damage)
    {
        if (damageCounterPrefab != null)
        {
            // Instantiate the damage counter at the enemy's position
            GameObject counter = Instantiate(damageCounterPrefab, transform.position, Quaternion.identity);

            // Set the damage value
            DamageCounter counterScript = counter.GetComponent<DamageCounter>();
            if (counterScript != null)
            {
                counterScript.SetDamage(damage);
            }
        }
    }
}