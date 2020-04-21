using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TestCameraRaycaster : MonoBehaviour
{
    [SerializeField] LayerMask _groundMask = default;

    private Camera _camera;
    private WaterArea _waterArea;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _waterArea = WaterArea.Instance;
    }

    void Update()
    {
    }
}
