using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Pot : Singleton<Pot>
{
    public int souls { get; private set; } = 0;

    public float power { get; private set; } = 0;

    public int maxSouls = 10;
    public int maxPower = 1000;

    [SerializeField] private float _maxSmokingDuration = 10.0f;
    [SerializeField] private float _maxSmokingRate = 10.0f;
    [SerializeField] private float _minSmokingRate = 0.0f;

    private ParticleSystem _bigSmokeParticle = null;
    private float _curSmokingDuration = 0.0f;
    private bool _isSmoking = false;

    [SerializeField] private SphereIndicator soulIndicator;
    [SerializeField] private SphereIndicator powerIndicator;

    void Awake()
    {
        souls = 0;
        power = maxPower;
        soulIndicator.SetFilled(souls, maxSouls);
        powerIndicator.SetFilled(power, maxPower);

        _bigSmokeParticle = _bigSmokeParticle ?? GetComponentInChildren<BigSmokePartical>().
                                                 GetComponent<ParticleSystem>();

        Assert.IsNotNull(_bigSmokeParticle, "[Pot]: Big smoke particle is null");

        var em = _bigSmokeParticle.emission;
        em.rateOverTime = _minSmokingRate;
    }

    void Update()
    {
        _curSmokingDuration += Time.deltaTime;
        if (_curSmokingDuration >= _maxSmokingDuration)
        {
            if (_isSmoking)
            {
                DisableBigSmoking();
            }
            _curSmokingDuration = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        var total = WaterArea.Instance.Calculate(transform.position);

        if (total > 200)
        {
            TakePower(1);
        }
    }

    public void TossObject(GameObject go)
    {
        Coal coal = go.GetComponent<Coal>();
        if (coal != null)
        {
            TossCoal(coal);
            return;
        }

        Sinner sinner = go.GetComponent<Sinner>();
        if (sinner != null)
        {
            TossSinner(sinner);
            return;
        }
    }

    void TossCoal(Coal coal)
    {
        power += coal.powerCapacity;
        coal.AsSpawnable().DoDespawnImmediately(gameObject);
        powerIndicator.SetFilled(power, maxPower);
    }

    void TossSinner(Sinner sinner)
    {
        EnableBigSmoking();

        souls += 1;
        sinner.AsSpawnable().DoDespawnImmediately(gameObject);
        soulIndicator.SetFilled(souls, maxSouls);
    }

    public void TakeSouls(int spent)
    {
        souls -= spent;
        soulIndicator.SetFilled(souls, maxSouls);
    }

    public void TakePower(float spent)
    {
        power -= spent;
        powerIndicator.SetFilled(power, maxPower);
    }

    void EnableBigSmoking()
    {
        _isSmoking = true;
        SetSmokingRate(_maxSmokingRate);
        _curSmokingDuration = 0.0f;
    }

    void DisableBigSmoking()
    {
        _isSmoking = false;
        SetSmokingRate(_minSmokingRate);
        _curSmokingDuration = 0.0f;
    }

    void SetSmokingRate(float v)
    {
        var em = _bigSmokeParticle.emission;
        em.rateOverTime = v;
    }
}
