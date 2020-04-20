using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Pot : Singleton<Pot>
{
    public int souls { get; private set; } = 0;
    public float power { get; private set; } = 0;

    [SerializeField] private float _maxSmokingDuration = 10.0f;
    [SerializeField] private float _maxSmokingRate = 10.0f;
    [SerializeField] private float _minSmokingRate = 0.0f;

    private ParticleSystem _bigSmokeParticle = null;
    private float _curSmokingDuration = 0.0f;
    private bool _isSmoking = false;

    void Awake()
    {
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
    }

    void TossSinner(Sinner sinner)
    {
        EnableBigSmoking();
        
        souls += 1;
        sinner.AsSpawnable().DoDespawnImmediately(gameObject);
    }

    public void TakeSouls(int spent)
    {
        souls -= spent;
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
