using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("GeneralStats")]
    [SerializeField] int CurrentHealth;
    [SerializeField] int MaxHealth = 100;

    [SerializeField] bool isDead = false;
    
    void Start()
    {
        CurrentHealth = MaxHealth;    
    }

    private void Update() {
        if(CurrentHealth <= 0){
            isDead = true;
        }
        
    }

    public void TakeDamage(int damage)
    {
        if(!isDead)
        {
            CurrentHealth -= damage;
        }
    }
}
