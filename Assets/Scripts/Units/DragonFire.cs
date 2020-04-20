using UnityEngine;

public class DragonFire : MonoBehaviour
{
    public Vector3 direction { get; set; }

    public float speed = 1.0f;
    public float timeToLive = 1.0f;

    private float _remainingTime = 0.0f;

    private void Awake()
    {
        _remainingTime = timeToLive;
    }

    void FixedUpdate()
    {
        transform.position += direction * speed * Time.fixedDeltaTime;

        _remainingTime -= Time.fixedDeltaTime;
        if (_remainingTime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
