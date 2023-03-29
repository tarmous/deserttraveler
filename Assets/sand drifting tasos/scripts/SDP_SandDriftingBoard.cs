using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SDP_SandDriftingBoard : MonoBehaviour
{
    #region variables
    private SDP_inputManager inputManagerInstace;
    private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;
    [SerializeField] private Transform[] raycastPoints;
    [SerializeField] private LayerMask layerMask;
    const float inputDeadzone = 0.05f;

    [SerializeField] private float maxMovementSpeed = 10;
    [SerializeField] private float reachMaxSpeedInSeconds = 2.5f;
    [SerializeField] private float haltInSecondsFromMax = 2.5f;
    private float decceleration = 1f;
    private float acceleration = 1f;
    private float movementSpeed = 0;
    [SerializeField] private float rotationRate = 1;
    [SerializeField] private float verticalRotationRate = 48;
    private float isGroundedTimer;

    [SerializeField] float limitRotationX = 45f;
    #endregion /variables

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = this.centerOfMass.localPosition;
        acceleration = maxMovementSpeed / reachMaxSpeedInSeconds;
        decceleration = -maxMovementSpeed / haltInSecondsFromMax;
        if (verticalRotationRate < limitRotationX + 3)  verticalRotationRate = limitRotationX + 3;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputManagerInstace = SDP_inputManager.instance;
    }

    void FixedUpdate()
    {
        // TODO:
        // Add a bool and check if this is possessed by player
        // dont read input/ function unless we want to

        // While not grounded use W-S to balance (front-back) the board

        if (raycastPoints.Length > 0)
        {
            UpdateIsGroundedTimer(raycastPoints);
        }

        float steer = ReadInput()[0]; // Horizontal
        float throttle = ReadInput()[1]; // Vertical
        //CalculateMovementSpeed(throttle);

        Vector3 deltaMov = Vector3.zero;
        Vector3 deltaRot = CalculateRotationSpeed(steer);
        if (!IsGrounded())
        {
            //AirControls(throttle);
            RestoreRotationZ();
            RestoreRotationX(-throttle);
            //rb.AddTorque(deltaMov, ForceMode.VelocityChange);
        }
        else
        {
            deltaMov = CalculateMovementSpeed(throttle);
        }

        if (IsGrounded() && Mathf.Abs(throttle) >= inputDeadzone)
        {
            rb.AddForce(deltaMov, ForceMode.Acceleration);
        }
        rb.AddTorque(deltaRot, ForceMode.VelocityChange);


        //rb.rotation = Quaternion.Euler(rb.rotation.eulerAngles + new Vector3(0, steer * rotationRate * Time.deltaTime, 0));

    }

    private float[] ReadInput()
    {
        return inputManagerInstace.GetInputData();
    }

    private void RestoreRotationZ()
    {
        const float interp = 0.125f;
        Vector3 desiredRot = new Vector3(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y, 0);

        rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.Euler(desiredRot), interp);

    }

    private void RestoreRotationX(float throttle)
    {
        Vector3 desiredRot = rb.rotation.eulerAngles;
        if (Mathf.Abs(throttle) >= inputDeadzone)
        {
            desiredRot.x = Mathf.Sign(throttle) * limitRotationX;
        }
        else if (Mathf.Abs(throttle) <= inputDeadzone)
        {
            desiredRot.x = 0;
        }
        rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.Euler(desiredRot), 1 - limitRotationX/verticalRotationRate);
    }

    private bool IsGrounded()
    {
        if (raycastPoints.Length == 0) return true;
        const float isGroundedThreshold = 0.15f;
        return isGroundedTimer <= isGroundedThreshold;
    }

    private void UpdateIsGroundedTimer(Transform[] points)
    {
        const float range = 1.5f;
        int hitCount = 0;

        foreach (Transform t in points)
        {
            RaycastHit hit;
            Ray ray = new Ray(t.position, Vector3.down);

            if (Physics.Raycast(ray, out hit, range, layerMask))
            {
                hitCount++;
            }
        }

        if (hitCount > points.Length / 2)
        {
            isGroundedTimer = 0;
        }
        else
        {
            isGroundedTimer += Time.deltaTime;
        }
        //Debug.Log(hitCount);
    }

    private Vector3 CalculateMovementSpeed(float throttle)
    {

        if (Mathf.Abs(throttle) >= inputDeadzone)
        {
            movementSpeed += Mathf.Sign(throttle) * acceleration * Time.deltaTime;
            movementSpeed = Mathf.Min(movementSpeed, maxMovementSpeed);
        }
        else if (Mathf.Abs(throttle) <= inputDeadzone)
        {
            movementSpeed += decceleration * Time.deltaTime;
            movementSpeed = Mathf.Max(movementSpeed, 0f);
        }
        Vector3 vel = transform.forward * movementSpeed;
        return vel - new Vector3(rb.velocity.x, vel.y + rb.velocity.y, rb.velocity.z);
    }

    private Vector3 CalculateRotationSpeed(float steer)
    {
        float vel = steer * rotationRate * Time.deltaTime;
        float delta = vel - rb.angularVelocity.y;

        return transform.up * delta;
    }

    private void AirControls(float throttle)
    {
        float vel = throttle * verticalRotationRate * Time.deltaTime;
        float delta = vel;
        
        transform.Rotate(delta, 0, 0);

        //return delta;
    }
}
