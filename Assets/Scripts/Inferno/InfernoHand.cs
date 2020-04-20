using UnityEngine;
using System.Collections;

public class InfernoHand : MonoBehaviour
{
    enum State
    {
        None,
        MovingUp,
        MovingDown,
        Releasing,
        Grabbing,
        WaitingAfterRelease,
        WaitingAfterGrab,
    }

    enum Action
    {
        None,
        Spawn,
        Despawn
    }

    [SerializeField] private Texture2D _textureReleased;
    [SerializeField] private Texture2D _textureGrabbed;
    [SerializeField] private MeshRenderer _handMeshRenderer;

    [SerializeField] private float _startHeight;
    [SerializeField] private float _downSpeed;
    [SerializeField] private float _upSpeed;
    [SerializeField] private float _releaseDelay;
    [SerializeField] private float _afterReleaseDelay;
    [SerializeField] private float _grabDelay;
    [SerializeField] private float _afterGrabDelay;

    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _handTransform;

    private Spawnable _entity;
    private Spawnable _attachedEntity;
    private Vector3 _attachmentOffset;
    private float _endHeight;
    private float _timer = 0.0f;
    private Transform _followTarget = null;

    private MaterialPropertyBlock _materialPropertyBlock;

    private State _state = State.None;
    private Action _action = Action.None;

    void Update()
    {
        switch (_state)
        {
            case State.MovingUp:
                _handTransform.localPosition = _handTransform.localPosition + Vector3.up * _upSpeed * Time.deltaTime;
                if (_handTransform.localPosition.y >= _startHeight)
                {
                    if (_attachedEntity)
                    {
                        if (_action == Action.Spawn)
                        {
                            _attachedEntity.DoSpawnEnd(gameObject);
                        }
                        else if (_action == Action.Despawn)
                        {
                            _attachedEntity.DoDespawnEnd(gameObject);
                        }
                    }
                    ResetAndDestroy();
                }
                break;
            case State.MovingDown:
                _handTransform.localPosition = _handTransform.localPosition + Vector3.down * _downSpeed * Time.deltaTime;
                if (_handTransform.localPosition.y <= _endHeight)
                {
                    if (_action == Action.Spawn)
                    {
                        Release();
                    }
                    if (_action == Action.Despawn)
                    {
                        Grab();
                    }
                }
                break;
            case State.Releasing:
                _timer -= Time.deltaTime;
                if (_timer <= 0.0f)
                {
                    _state = State.WaitingAfterRelease;
                    _timer = _afterReleaseDelay;
                    if (_attachedEntity)
                    {
                        _attachedEntity.DoSpawnEnd(gameObject);
                        _attachedEntity = null;
                    }
                    SetTexture(_textureReleased);
                }
                break;
            case State.Grabbing:
                _timer -= Time.deltaTime;
                if (_timer <= 0.0f)
                {
                    _state = State.WaitingAfterGrab;
                    _timer = _afterGrabDelay;

                    if (_entity)
                    {
                        _entity.DoDespawnStart(gameObject);
                        _attachedEntity = _entity;
                    }

                    SetTexture(_textureGrabbed);
                }
                break;
            case State.WaitingAfterRelease:
            case State.WaitingAfterGrab:
                _timer -= Time.deltaTime;
                if (_timer <= 0.0f)
                {
                    MoveUp();
                }
                break;
        }
    }

    private void LateUpdate()
    {
        if (_attachedEntity)
        {
            var position = _attachmentOffset + _spawnPoint.position;
            _attachedEntity.transform.position = new Vector3(position.x, Mathf.Max(position.y, 0.0f), position.z);
        }

        if (_followTarget != null)
        {
            transform.position = _followTarget.position;
        }
    }

    public void PlaceEntity(Spawnable entity, Transform followTarget)
    {
        _followTarget = followTarget;
        _entity = entity;
        _attachedEntity = entity;
        _action = Action.Spawn;

        var attachmentPoint = entity.handAttachmentPoint;
        _attachmentOffset = entity.transform.position - attachmentPoint.position;

        _endHeight = _handTransform.InverseTransformPoint(_spawnPoint.position - _attachmentOffset).y;

        MoveDown();

        _entity.DoSpawnStart(gameObject);
        SetTexture(_textureGrabbed);
    }

    public void RemoveEntity(Spawnable entity)
    {
        _entity = entity;
        _action = Action.Despawn;

        var attachmentPoint = entity.handAttachmentPoint;
        _attachmentOffset = entity.transform.position - attachmentPoint.position;

        _endHeight = _handTransform.InverseTransformPoint(_spawnPoint.position - _attachmentOffset).y;

        MoveDown();

        _entity.DoDespawnStart(gameObject);
    }

    private void MoveUp()
    {
        _state = State.MovingUp;
    }

    private void MoveDown()
    {
        _handTransform.localPosition = Vector3.up * _startHeight;
        _state = State.MovingDown;
    }

    private void Release()
    {
        _state = State.Releasing;
        _timer = _releaseDelay;
    }

    private void Grab()
    {
        _state = State.Grabbing;
        _timer = _grabDelay;
    }

    private void ResetAndDestroy()
    {
        _state = State.None;
        _action = Action.None;
        Destroy(gameObject);
    }

    private void SetTexture(Texture2D texture)
    {
        if (_materialPropertyBlock == null)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        _materialPropertyBlock.SetTexture("_BaseMap", texture);
        _handMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
