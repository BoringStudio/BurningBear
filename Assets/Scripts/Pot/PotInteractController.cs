﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Pot))]
public class PotInteractController : MonoBehaviour
{
    private Pot _pot = null;

    // Start is called before the first frame update
    void Awake()
    {
        _pot = _pot ?? GetComponent<Pot>();

        Assert.IsNotNull(_pot, "[PotInteractController]: Pot is null");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        TryOnCoalEnter(collider);
        TryOnSinnerEnter(collider);
    }

    void TryOnCoalEnter(Collider collider)
    {
        var coal = collider.GetComponent<Coal>();
        if (coal == null)
        {
            return;
        }

        _pot.power += coal.powerCapacity;

        var coalSpawnHandler = coal.GetComponent<Spawnable>();
        coalSpawnHandler.DoDespawnImmediately(gameObject);
    }

    void TryOnSinnerEnter(Collider collider)
    {
        var sinner = collider.GetComponent<Sinner>();
        if (sinner == null)
        {
            return;
        }

        _pot.souls += 1;

        var sinnerSpawnHandler = sinner.GetComponent<Spawnable>();
        sinnerSpawnHandler.DoDespawnImmediately(gameObject);
    }
}
