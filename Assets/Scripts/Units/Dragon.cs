using UnityEngine;
using UnityEngine.Assertions;

public class Dragon : Unit
{
    public override int cost { get => 6; }

    [SerializeField] private Color _originColor;
    [SerializeField] private Color _dyingColor;

    [SerializeField] private DragonFire _firePrefab;
    [SerializeField] private Transform _fireSpawningPoint;

    [SerializeField] private float _requiredPower = 10.0f;
    [SerializeField] private float _powerConsumptionTime = 10.0f;

    [SerializeField] private MeshRenderer _dragonMeshRenderer;

    [SerializeField] private float _timeToLive = 10.0f;
    [SerializeField] private float _cooldown = 1.0f;

    private float _currentPowerConsumptionTime = 0.0f;

    private float _currentCooldown = 0;
    private float _drainingRemaining = 0.0f;

    private bool _isShooting = false;
    private bool _isActive = true;

    public float rotateAfter = 2.0f;
    public float _currentRotationTimer = 0.0f;

    private float _remainingTimeToLive;

    private Pot _pot = null;
    private Inferno _inferno = null;
    private WaterArea _waterArea = null;

    private Animator _animator;

    void Awake()
    {
        _remainingTimeToLive = _timeToLive;

        _pot = FindObjectOfType<Pot>();
        _inferno = FindObjectOfType<Inferno>();
        _waterArea = FindObjectOfType<WaterArea>();

        _animator = GetComponentInChildren<Animator>();

        Assert.IsNotNull(_pot, "[Dragon]: Pot is null");
        Assert.IsNotNull(_inferno, "[Dragon]: Inferno is null");
        Assert.IsNotNull(_waterArea, "[Dragon]: Water area is null");
        Assert.IsNotNull(_animator, "[Dragon]: Animator is null");

        var animationEventNotifier = GetComponentInChildren<AnimationEventNotifier>();
        animationEventNotifier.action.AddListener(Fire);
    }

    void Update()
    {
        _isActive = _pot.power >= _requiredPower;

        _currentPowerConsumptionTime += Time.deltaTime;
        if (_currentPowerConsumptionTime >= _powerConsumptionTime && _isActive)
        {
            _pot.TakePower(_requiredPower);
            _currentPowerConsumptionTime = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        if (!_isActive)
        {
            return;
        }

        if (!_isShooting)
        {
            _currentCooldown -= Time.fixedDeltaTime;

            if (_currentCooldown <= 0)
            {
                _currentCooldown = _cooldown;
                _animator.SetTrigger("Fire");

                _isShooting = true;
            }
        }

        var scale = _dragonMeshRenderer.transform.localScale;
        Debug.Log(scale);

        _currentRotationTimer += Time.fixedDeltaTime;
        if (_currentRotationTimer > rotateAfter)
        {
            scale.x *= -1.0f;
            _dragonMeshRenderer.transform.localScale = scale;
            _currentRotationTimer = 0.0f;
        }

        if (_drainingRemaining >= 0.0f)
        {
            _waterArea.EvaporateSector(transform.position, scale.x > 0 ? 0 : 1, 5, 100);
            _drainingRemaining -= Time.fixedDeltaTime;
        }
        else
        {
            _waterArea.EvaporateSector(transform.position, 0, 5, 10);
        }

        _remainingTimeToLive -= Time.deltaTime;
        if (_remainingTimeToLive <= 0.0f)
        {
            _isActive = false;
            _inferno.Despawn(this);
        }
    }

    public void Fire()
    {
        for (int i = -3; i < 2; ++i)
        {
            var sign = Mathf.Sign(_dragonMeshRenderer.transform.localScale.x);

            var direction = (Vector3.left * 5 + Vector3.forward * i - Vector3.forward * sign * 2).normalized * sign;
            var fire = Instantiate(_firePrefab, _fireSpawningPoint.transform.position + Random.Range(0.0f, 1.0f) * Vector3.right, Quaternion.identity);
            var firescale = fire.transform.localScale;
            firescale.x *= sign;
            fire.transform.localScale = firescale;
            fire.direction = direction;
        }

        _isShooting = false;
        _drainingRemaining = _cooldown * 0.99f;
    }

    protected override void OnSpawnEnd(GameObject by)
    {
        _isActive = true;
        _remainingTimeToLive = _timeToLive;
    }

    protected override void OnDespawnEnd(GameObject by)
    {
        Destroy(gameObject);
    }
}
