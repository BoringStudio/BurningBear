using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinnerInteractController : MonoBehaviour, Interactable
{
    private Collider _collider = null;

    void Awake() {
        _collider = _collider ?? GetComponentInChildren<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseInteractStart(Player player, Vector3 position) {
        switch(player.state) {
            case Player.State.Normal:
                player.state = Player.State.Grab;
                _collider.enabled = false;
                transform.SetParent(player.interactionPoint.transform);
                transform.localPosition = Vector3.zero;
            break;
        }
    }

    public void OnMouseInteractEnd(Player player, Vector3 position) {

    }

    public void OnInteractStart(Player player) {}
	public void OnInteractEnd(Player player) {}
}
