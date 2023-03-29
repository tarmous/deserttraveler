using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent (typeof (CinemachineFreeLook))]
public class FLCInputSystemLink : MonoBehaviour {

    private CinemachineFreeLook freeLookCamera;
    private const float rotDeadzone = 0.001f;

    void Awake () {
        freeLookCamera = GetComponent<CinemachineFreeLook> ();
    }

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (!Player._instance) return;
        Vector2 rotInput = Player._instance.RotateProperty;

        freeLookCamera.m_XAxis.m_InputAxisValue = rotInput.x;
        freeLookCamera.m_YAxis.m_InputAxisValue = rotInput.y;

    }
}