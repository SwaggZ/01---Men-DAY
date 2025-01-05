using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AttachmentsSystem : NetworkBehaviour
{
    [Header("Scopes")]
    public GameObject[] Scopes;
    public Transform[] ScopeAimingPositions; // Aiming positions for each scope
    
    public GameObject[] Lasers;
    public GameObject[] Supressors;

    int selectedScope = 0;
    int selectedLaser = 0;
    int selectedSupressor = 0;

    public int ScopeIndex;
    public int LaserIndex;
    public int SupressorIndex;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // if(Input.GetKeyDown(KeyCode.V))
        // {
        //     selectScope();
        //     selectLaser();
        //     selectSupressor();
        // }
    }

    void selectScope()
    {
        ScopeIndex = Random.Range(0, Scopes.Length + 1);
        if (ScopeIndex == Scopes.Length)
        {
            for (int j = 0; j <= Scopes.Length; j++)
            {
                Scopes[j].SetActive(false);
            }
        }
        else
        {
            Scopes[ScopeIndex].SetActive(true);
            for (int j = 0; j < Scopes.Length; j++)
            {
                if (j != ScopeIndex)
                {
                    Scopes[j].SetActive(false);
                }
            }
        }
    }

    void selectLaser()
    {
        LaserIndex = Random.Range(0, Lasers.Length + 1);
        if (LaserIndex == Lasers.Length)
        {
            for (int j = 0; j <= Lasers.Length; j++)
            {
                Lasers[j].SetActive(false);
            }
        }
        else
        {
            Lasers[LaserIndex].SetActive(true);
            for (int j = 0; j < Lasers.Length; j++)
            {
                if (j != LaserIndex)
                {
                    Lasers[j].SetActive(false);
                }
            }
        }
    }

    void selectSupressor()
    {
        SupressorIndex = Random.Range(0, Supressors.Length + 1);
        if (SupressorIndex == Supressors.Length)
        {
            for (int j = 0; j <= Supressors.Length; j++)
            {
                Supressors[j].SetActive(false);
            }
        }
        else
        {
            Supressors[SupressorIndex].SetActive(true);
            for (int j = 0; j < Supressors.Length; j++)
            {
                if (j != SupressorIndex)
                {
                    Supressors[j].SetActive(false);
                }
            }
        }
    }
}
