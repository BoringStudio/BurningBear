using UnityEngine;

public class TestCameraRaycaster : MonoBehaviour
{
    public new Camera camera;
    public WaterArea waterArea;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            LayerMask layerMask = LayerMask.GetMask("TileMap");
            Debug.Log(layerMask);

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                Vector3 point = hit.point;
                Debug.Log(point);
                waterArea.Evaporate(point);
            }
        }
    }
}
