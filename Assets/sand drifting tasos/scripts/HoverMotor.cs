using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverMotor : MonoBehaviour
{
    public float speed = 90f;
    public float turnSpeed = 5f;
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    private float powerInput;
    private float turnInput;
    private Rigidbody carRigidbody;


    void Awake () 
    {
        carRigidbody = GetComponent <Rigidbody>();
    }

    void Update () 
    {   
        Vector2 MovInput = Player._instance.MoveProperty;
        powerInput = MovInput.y;
        turnInput = MovInput.x;
    }

    void FixedUpdate()
    {
        Ray ray = new Ray (transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            carRigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);

            //Debug.Log(Vector3.Angle(hit.normal,Vector3.up));
            AlignRotationRelativeToGround(hit);
        }

        carRigidbody.AddRelativeForce(0f, 0f, powerInput * speed);
        carRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);

    }

    private void AlignRotationRelativeToGround(RaycastHit hit)
    {
        // What is Target Rotation?
        Vector3 direction = transform.position - hit.point;
        //Vector3 targetRotation = Vector3.Cross(direction, transform.right);
        float targetRotation = -Vector3.Angle(hit.normal,Vector3.up);
        carRigidbody.MoveRotation(Quaternion.Euler(targetRotation, transform.rotation.eulerAngles.y, 0 ));
    }
}
