using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDP_CameraFollow : MonoBehaviour
{
    private SDP_inputManager inputManagerInstace;

    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0);
    public float speed = 0.05f;
    public float cameraRotSpeed = 10f;
    public float zoomSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        inputManagerInstace = SDP_inputManager.instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target) return;

        transform.position = Vector3.Lerp(transform.position, target.position + offset, speed);

        float inputH = inputManagerInstace.GetInputData()[2];
        float inputV = inputManagerInstace.GetInputData()[3];

        transform.Rotate(0, inputH * cameraRotSpeed * Time.deltaTime, 0, Space.World);
        transform.Rotate(-inputV * cameraRotSpeed * Time.deltaTime, 0, 0, Space.Self);
    }

    void LateUpdate()
    {
        ZoomCamera(inputManagerInstace.GetInputData()[4]);
    }

    private void ZoomCamera(float input)
    {
        transform.GetChild(0).localPosition = transform.GetChild(0).localPosition + new Vector3(0, 0, input * zoomSpeed);
    }
}
