using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventNotifier : MonoBehaviour
{
    public UnityEvent action;

    public void Notify()
    {
        action.Invoke();
    }
}
