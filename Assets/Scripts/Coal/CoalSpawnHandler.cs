using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalSpawnHandler : Spawnable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnSpawnImmediately(GameObject by) { }
    protected override void OnSpawnStart(GameObject by) { }
    protected override void OnSpawnEnd(GameObject by) { 
        Player.Instance.AddToHand(GetComponent<Attachable>());
     }
    protected override void OnDespawnImmediately(GameObject by)
    {
        Pot pot = by.GetComponent<Pot>();
        if (pot == null)
        {
            return;
        }

        Destroy(gameObject);
    }
    protected override void OnDespawnStart(GameObject by) { }
    protected override void OnDespawnEnd(GameObject by) { }
}
