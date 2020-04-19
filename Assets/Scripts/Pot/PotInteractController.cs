using System.Collections;
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

        coal.StartDestroy(gameObject);
        _pot.power += coal.powerCapacity;
    }

    void TryOnSinnerEnter(Collider collider)
    {
        var sinner = collider.GetComponent<Sinner>();
        if (sinner == null)
        {
            return;
        }

        sinner.StartDestroy(gameObject);
        _pot.souls += 1;
    }
}
