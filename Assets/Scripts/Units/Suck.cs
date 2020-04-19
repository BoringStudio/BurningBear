using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suck : MonoBehaviour
{
    public WaterArea waterArea;
    public float cooldown = 10;

    public float _currentCooldown = 0;

    void FixedUpdate()
    {
        _currentCooldown -= Time.fixedDeltaTime;

        if (_currentCooldown <= 0)
        {
            _currentCooldown = cooldown;
            //waterArea.Evaporate(transform.position);
        }
    }
}
