using UnityEngine;
using UnityEngine.Assertions;

public class Pot : MonoBehaviour
{
    public int souls { get; private set; } = 0;

    public float power { get; private set; } = 0;

    [SerializeField] private int _initialSouls = 10;
    [SerializeField] private int _initialPower = 1000;

    [SerializeField] private int _maxSouls = 10;
    [SerializeField] private int _maxPower = 1000;

    [SerializeField] private float _maxSmokingDuration = 10.0f;
    [SerializeField] private float _maxSmokingRate = 10.0f;
    [SerializeField] private float _minSmokingRate = 0.0f;

    [SerializeField] private SphereIndicator soulIndicator;
    [SerializeField] private SphereIndicator powerIndicator;

    [SerializeField] private WaterArea _waterArea;
    [SerializeField] private GameController _gameController;

    private ParticleSystem _bigSmokeParticle = null;
    private float _currentSmokingDuration = 0.0f;
    private bool _isSmoking = false;

    void Awake()
    {
        souls = _initialSouls;
        power = _initialPower;

        soulIndicator.SetFilled(souls, _maxSouls);
        powerIndicator.SetFilled(power, _maxPower);

        _bigSmokeParticle = _bigSmokeParticle ?? GetComponentInChildren<BigSmokePartical>().GetComponent<ParticleSystem>();

        Assert.IsNotNull(_waterArea, "[Pot]: Water area is null");
        Assert.IsNotNull(_gameController, "[Pot]: Game controller is null");

        Assert.IsNotNull(_bigSmokeParticle, "[Pot]: Big smoke particle is null");

        SetSmokingRate(_minSmokingRate);
    }

    private void FixedUpdate()
    {
        _currentSmokingDuration += Time.fixedDeltaTime;
        if (_currentSmokingDuration >= _maxSmokingDuration)
        {
            if (_isSmoking)
            {
                DisableBigSmoking();
            }
            _currentSmokingDuration = 0.0f;
        }

        var total = _waterArea.Calculate(transform.position);
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
        powerIndicator.SetFilled(power, _maxPower);
    }

    void TossSinner(Sinner sinner)
    {
        EnableBigSmoking();

        souls += 1;
        sinner.AsSpawnable().DoDespawnImmediately(gameObject);
        soulIndicator.SetFilled(souls, _maxSouls);
    }

    public void TakeSouls(int spent)
    {
        souls -= spent;
        soulIndicator.SetFilled(souls, _maxSouls);
    }

    public void TakePower(float spent)
    {
        power -= spent;
        powerIndicator.SetFilled(power, _maxPower);
        if (power <= 0)
        {
            _gameController.DoGameOver();
        }
    }

    void EnableBigSmoking()
    {
        _isSmoking = true;
        SetSmokingRate(_maxSmokingRate);
        _currentSmokingDuration = 0.0f;
    }

    void DisableBigSmoking()
    {
        _isSmoking = false;
        SetSmokingRate(_minSmokingRate);
        _currentSmokingDuration = 0.0f;
    }

    void SetSmokingRate(float rate)
    {
        var emission = _bigSmokeParticle.emission;
        emission.rateOverTime = rate;
    }
}
