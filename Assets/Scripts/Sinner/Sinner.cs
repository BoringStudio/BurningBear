using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sinner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
