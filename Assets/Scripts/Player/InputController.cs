﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public float horizontalAxis
    {
        get;
        private set;
    }
    public float verticalAxis
    {
        get;
        private set;
    }
    public bool interactButtonDown
    {
        get;
        private set;
    }
    public bool interactButtonUp
    {
        get;
        private set;
    }
    public bool firstButtonPressed
    {
        get;
        private set;
    }
    public bool secondButtonPressed
    {
        get;
        private set;
    }
    public bool thirdButtonPressed
    {
        get;
        private set;
    }
    public bool fourthButtonPressed
    {
        get;
        private set;
    }
    public Vector3 mousePosition
    {
        get;
        private set;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");

        interactButtonDown = Input.GetMouseButtonDown(0);
        interactButtonUp = Input.GetMouseButtonUp(0);

        firstButtonPressed = Input.GetKeyDown(KeyCode.Alpha1);
        secondButtonPressed = Input.GetKeyDown(KeyCode.Alpha2);
        thirdButtonPressed = Input.GetKeyDown(KeyCode.Alpha3);
        fourthButtonPressed = Input.GetKeyDown(KeyCode.Alpha4);

        mousePosition = Input.mousePosition;
    }
}
