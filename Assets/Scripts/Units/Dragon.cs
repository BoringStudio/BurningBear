﻿using UnityEngine;
using System.Collections;

public class Dragon : Spawnable
{
    [SerializeField] private Color _originColor;
    [SerializeField] private Color _dyingColor;

    [SerializeField] private DragonFire _firePrefab;
    [SerializeField] private Transform _fireSpawningPoint;

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
        _waterArea.EvaporateSector(transform.position, 0);

        var step = Mathf.PI / 5;
        for (int i = -3; i < 2; ++i)
        {
            var direction = (Vector3.left * 5 + Vector3.forward * i - Vector3.forward * 2).normalized;
            var fire = Instantiate(_firePrefab, _fireSpawningPoint.transform.position + Random.Range(0.0f, 1.0f) * Vector3.right, Quaternion.identity);
            fire.direction = direction;
        }

        _isShooting = false;
    }
}
