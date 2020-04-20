using UnityEngine;
using System.Collections;

public class Dragon : Spawnable
{
    [SerializeField] private Color _originColor;
    [SerializeField] private Color _dyingColor;

    public float timeToLive = 10.0f;
    public float cooldown = 1.0f;

    private float _currentCooldown = 0;
    private WaterArea _waterArea;

    private MaterialPropertyBlock _materialPropertyBlock;
    private MeshRenderer _meshRenderer;

    private Animator _animator;

    private bool _isShooting = false;
    private bool _isActive = true;

    void Awake()
    {
        _waterArea = WaterArea.Instance;

        _materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();

        _animator = GetComponentInChildren<Animator>();

        var animationEventNotifier = GetComponentInChildren<AnimationEventNotifier>();
        animationEventNotifier.action.AddListener(Fire);
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
                _currentCooldown = cooldown;
                _animator.SetTrigger("Fire");

                _isShooting = true;
            }
        }
    }

    public void Fire()
    {
        Debug.Log("FIRE");

        _isShooting = false;
    }
}
