﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinnerSpawnHandler : Spawnable
{
    [HideInInspector]
    public SinnerSpawner spawner = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    protected override void OnDespawnImmediately(GameObject by)
    {
        Destroy(gameObject);
    }
}
