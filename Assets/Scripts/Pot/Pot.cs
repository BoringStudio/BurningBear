using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Pot : MonoBehaviour
{
    public int souls = 0;
    public float power = 0;

    void Awake()
    {
    }

    public void TossObject(GameObject go)
    {
        Coal coal = go.GetComponent<Coal>();
        if (coal != null) {
            TossCoal(coal);
            return;
        }
        
        Sinner sinner = go.GetComponent<Sinner>();
        if (sinner != null) {
            TossSinner(sinner);
            return;
        }
    }

    void TossCoal(Coal coal)
    {
        power += coal.powerCapacity;
        coal.AsSpawnable().DoDespawnImmediately(gameObject);
    }

    void TossSinner(Sinner sinner)
    {
        souls += 1;
        sinner.AsSpawnable().DoDespawnImmediately(gameObject);
    }
}
