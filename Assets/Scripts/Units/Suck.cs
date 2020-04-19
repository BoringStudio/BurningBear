using UnityEngine;

public class Suck : MonoBehaviour
{
    [SerializeField] public Texture2D _idleTexture;
    [SerializeField] public Texture2D[] _suckTextures;

    public float cooldown = 10;

    private float _currentCooldown = 0;
    private WaterArea _waterArea;

    private int _currentFrame = 0;
    private float _currentFrameTime = 0.0f;

    private MaterialPropertyBlock _materialPropertyBlock;
    private MeshRenderer _meshRenderer;

    private bool _isSucking = false;

    private void Awake()
    {
        _waterArea = WaterArea.Instance;

        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void FixedUpdate()
    {
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

                _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
            }
        }
    }
}
