using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDP_inputManager : MonoBehaviour
{
    private static SDP_inputManager _instance;
    public static SDP_inputManager instance{
        get {
            return _instance;
            }
        set {return;}
    }
    
    private float _horizontal;
    private float _vertical;
    private float _mouseX;
    private float _mouseY;
    private float _scrollWheel;

    private void Awake()
    {
        CheckInstance();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");
        _scrollWheel = Input.GetAxis("Mouse ScrollWheel");
    }

    private void CheckInstance()
    {
        if (_instance == this) return;
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(_instance);
            _instance = this;
        }
    }

    public float[] GetInputData()
    {
        return new float[]
       {
        _horizontal,
        _vertical,
        _mouseX,
        _mouseY,
        _scrollWheel
       };
    }
}
