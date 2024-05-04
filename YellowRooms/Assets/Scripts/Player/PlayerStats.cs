using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerStats : NetworkBehaviour
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
        if(!isLocalPlayer)
        {
            return;
        }
        
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
