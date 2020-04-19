using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sinner : MonoBehaviour, Attachable
{
    public enum State
    {
        Grabbed,
        Released,
    }

    public State state = State.Released;
    public Transform grabbedOrigin = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Grabbed:
                if (grabbedOrigin == null)
                {
                    Debug.LogError("Grabbed origin is null");
                    break;
                }

                transform.position = grabbedOrigin.position;
                break;
            case State.Released:
                // Do nothing
                break;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "Water")
        {
            return;
        }

        Destroy(gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag != "Water")
        {
            return;
        }
    }

    public void Attach(Transform origin) {
        grabbedOrigin = origin;
        state = Sinner.State.Grabbed;
    }

    public void Deattach(Vector3 releasedPosition) {
        grabbedOrigin = null;
        transform.position = releasedPosition;
        state = Sinner.State.Released;
    }
}
