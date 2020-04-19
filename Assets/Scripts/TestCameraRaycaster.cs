using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TestCameraRaycaster : MonoBehaviour
{
    [SerializeField] Suck _suck;

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, _groundMask))
            {
                Vector3 point = hit.point;
                _waterArea.Liquify(point);
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, _groundMask))
            {
                Vector3 point = hit.point;
                point.y = 0;

                Instantiate(_suck, point, Quaternion.identity);
            }
        }
    }
}
