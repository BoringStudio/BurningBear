using UnityEngine;

public class Suck : Spawnable
{
    [SerializeField] private Texture2D _idleTexture;
    [SerializeField] private Texture2D[] _suckTextures;

    [SerializeField] private Color _originColor;
    [SerializeField] private Color _dyingColor;

    public float timeToLive = 10.0f;

    public float cooldown = 1.0f;

    private float _currentCooldown = 0;
    private WaterArea _waterArea;

    private int _currentFrame = 0;
    private float _currentFrameTime = 0.0f;

    private MaterialPropertyBlock _materialPropertyBlock;
    private MeshRenderer _meshRenderer;

    private bool _isSucking = false;
    private bool _isActive = false;

    private float _remainingTime;

    private void Awake()
    {
        _waterArea = WaterArea.Instance;

        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void FixedUpdate()
    {
        if (!_isActive)
        {
            return;
        }

        _currentCooldown -= Time.fixedDeltaTime;

        if (_currentCooldown <= 0)
        {
            _currentCooldown = cooldown;
            _waterArea.Evaporate(transform.position);

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

        _materialPropertyBlock.SetColor("_Color", Color.Lerp(_dyingColor, _originColor, _remainingTime / timeToLive));
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);

        _remainingTime -= Time.deltaTime;
        if (_remainingTime <= 0.0f)
        {
            _isActive = false;
            Inferno.Instance.Despawn(this);
        }
    }

    protected override void OnSpawnEnd(GameObject by)
    {
        _isActive = true;
        _remainingTime = timeToLive;
    }

    protected override void OnDespawnEnd(GameObject by)
    {
        Destroy(gameObject);
    }
}
