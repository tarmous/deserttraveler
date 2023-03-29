using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MCam : MonoBehaviour
{   
    ControlsMap controls;
    Vector2 move;
    Vector2 rotate;
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0);
    public float speed = 0.05f;
    public float cameraRotSpeed = 10f;
    public float zoomSpeed = 2f;
    float input;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target) return;

        transform.position = Vector3.Lerp(transform.position, target.position + offset, speed);

        float inputH = rotate.x;
        float inputV = rotate.y;

        transform.Rotate(0, inputH * cameraRotSpeed * Time.deltaTime, 0, Space.Self);
        transform.Rotate(-inputV * cameraRotSpeed * Time.deltaTime, 0, 0, Space.Self);
    }

    void Awake()
    {
        controls = new ControlsMap();
        controls.Gameplay.Rotate.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        controls.Gameplay.Rotate.canceled += ctx => rotate = Vector2.zero;

    }
    void LateUpdate()
    {
        ZoomCamera(rotate.x);
    }

    private void ZoomCamera(float input)
    {   
        GameObject player = GameObject.Find("Player");

        transform.localPosition = player.transform.localPosition + offset;
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

}
