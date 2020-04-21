using UnityEngine;
using System.Collections;

public class WaterSpawner : MonoBehaviour
{
    private WaterArea _waterArea;

    public float liquifyEvery = 1.0f;
    public float enableAfter = 0.0f;

    private float _currentTimer = 0.0f;
    private float _liquifyTimer = 0.0f;

    private void Awake()
    {
        _waterArea = WaterArea.Instance;
    }

    private void FixedUpdate()
    {
        _currentTimer += Time.fixedDeltaTime;

        if (_currentTimer > enableAfter) {
            _liquifyTimer += Time.fixedDeltaTime;
            if (_liquifyTimer >= liquifyEvery)
            {
                _waterArea.Liquify(transform.position);
            }
        }
    }
}
