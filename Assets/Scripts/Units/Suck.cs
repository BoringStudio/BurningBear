using UnityEngine;
using UnityEngine.Assertions;

public class Suck : Unit
{
    public override int cost { get => 3; }

    [SerializeField] private Texture2D _idleTexture;
    [SerializeField] private Texture2D[] _suckTextures;

    [SerializeField] private Color _originColor;
    [SerializeField] private Color _dyingColor;

    [SerializeField] public float requiredPower = 10.0f;
    [SerializeField] public float powerConsumptionTime = 10.0f;

    [SerializeField] private float timeToLive = 10.0f;
    [SerializeField] private float cooldown = 1.0f;

    private float _curPowerConsumptionTime = 0.0f;

    private float _currentCooldown = 0;

    private int _currentFrame = 0;
    private float _currentFrameTime = 0.0f;

    private bool _isSucking = false;
    private bool _isActive = false;

    private float _remainingTimeToLive;

    private Pot _pot = null;
    private Inferno _inferno = null;
    private WaterArea _waterArea = null;

    private MaterialPropertyBlock _materialPropertyBlock;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _remainingTimeToLive = timeToLive;

        _pot = FindObjectOfType<Pot>();
        _inferno = FindObjectOfType<Inferno>();
        _waterArea = FindObjectOfType<WaterArea>();

        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        Assert.IsNotNull(_pot, "[Suck]: Pot is null");
        Assert.IsNotNull(_inferno, "[Suck]: Inferno is null");
        Assert.IsNotNull(_waterArea, "[Suck]: Water area is null");
        Assert.IsNotNull(_meshRenderer, "[Suck]: Mesh renderer is null");
    }

    void FixedUpdate()
    {
        _isActive = _pot.power >= requiredPower;

        if (!_isActive)
        {
            return;
        }

        _curPowerConsumptionTime += Time.deltaTime;
        if (_curPowerConsumptionTime >= powerConsumptionTime)
        {
            if (_isActive)
            {
                _pot.TakePower(requiredPower);
            }
            _curPowerConsumptionTime = 0.0f;
        }

        _currentCooldown -= Time.fixedDeltaTime;

        if (_currentCooldown <= 0)
        {
            _currentCooldown = cooldown;
            _waterArea.EvaporateCircle(transform.position);

            _isSucking = true;
            _currentFrameTime = 0.0f;
        }

        if (_isSucking)
        {
            _currentFrameTime -= Time.fixedDeltaTime;

            if (_currentFrameTime <= 0.0f)
            {
                _currentFrameTime = 0.1f;

                if (_currentFrame + 1 > _suckTextures.Length)
                {
                    _materialPropertyBlock.SetTexture("_BaseMap", _idleTexture);
                    _currentFrame = 0;
                    _isSucking = false;
                }
                else
                {
                    _materialPropertyBlock.SetTexture("_BaseMap", _suckTextures[_currentFrame++]);
                }
            }
        }

        _materialPropertyBlock.SetColor("_Color", Color.Lerp(_dyingColor, _originColor, _remainingTimeToLive / timeToLive));
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);

        _remainingTimeToLive -= Time.deltaTime;
        if (_remainingTimeToLive <= 0.0f)
        {
            _isActive = false;
            _inferno.Despawn(this);
        }
    }

    protected override void OnSpawnEnd(GameObject by)
    {
        _isActive = true;
        _remainingTimeToLive = timeToLive;
    }

    protected override void OnDespawnEnd(GameObject by)
    {
        Destroy(gameObject);
    }
}
