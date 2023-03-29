using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Mountable {

    #region Public variables
    // Static
    public static Player _instance;
    // Constants 
    // Serialized
    // Unity Based
    public GameObject MountedOn; // currently object player mounts
    // Generic
    // Properties
    public Vector2 RotateProperty{
        get{ return rotate;}
        private set{ rotate = value;}
    }

     public Vector2 MoveProperty{
        get{ return move;}
        private set{ move = value;}
    }

    public bool InteractProperty
    {
        get{ return interact;}

        private set { interact = value;}

    }


    #endregion /Public Variables

    #region Private Variables
    // Static

    // Constants
    private const float Dzone = 0.1f;

    // Serialized
    [SerializeField]public float walkSpeed = 3f;
    [SerializeField]public float runSpeed = 6f;
    [SerializeField]public float verticalSpeed = 8f ;
    [SerializeField]public float jumpHeight = 2.5f;



    // Unity Based
    private GameObject player;  //Setting Game Object as player
    private Animator anim;  //Setting Animator
    private Camera cam; //Setting Camera
    private CharacterController controller; //Setting Controller
    private ControlsMap controls;   //Setting Action Map
    private Vector2 move;   // Vector for moving character
    private Vector2 rotate; // Vector for rotating character
    private Vector2 jump;   // Vector for character jump

     

    

    // Generic
    private bool isMounted; //Boolean for checking if is mounted or not
    private bool interact;  //Boolean for checking if interacting with something
    private bool isGrounded = true;
    private bool canJump;
    private bool canDoubleJump;
    private Vector3 gravity = Vector3.zero;
    private Vector3 lookDir;
    [SerializeField] public float cameraRot;
    private float lockPos = 0f;

    #endregion /Private Variables

    #region Unity Functions
    void Awake () 
    {
        SetInstance ();
        SetupInput ();
        GetPrerequisiteComponents ();
    }
    void Start () 
    {
        
    }

    void Update () 
    {   
        if(controller.isGrounded)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        cameraRot = Camera.main.transform.rotation.y;
        Move();    
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        // The speed on the x-z plane ignoring any speed
        float horizontalSpeed = horizontalVelocity.magnitude;
        // The speed from gravity or jumping
        float verticalSpeed  = controller.velocity.y;
        // The overall speed
        float overallSpeed = controller.velocity.magnitude;
        //Debug.Log("horizontalVelocity  +" + horizontalSpeed + "  verticalSpeed " + verticalSpeed + " overallSpeed  = " + overallSpeed);
        Debug.Log(isGrounded);
    }



    void OnEnable () 
    {
        controls.Gameplay.Enable ();
    }

    void OnDisable () 
    {
        controls.Gameplay.Disable ();
    }
    #endregion /Unity Functions
    


    private void SetupInput () 
    {
        controls = new ControlsMap ();


        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2> ();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;
        controls.Gameplay.Move.canceled += ctx => walkSpeed = 3f;
        controls.Gameplay.Move.canceled += ctx => anim.SetInteger("Walking", 0);

        controls.Gameplay.Rotate.performed += ctx => rotate = ctx.ReadValue<Vector2> ();
        controls.Gameplay.Rotate.canceled += ctx => rotate = Vector2.zero;

        //controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Jump.performed += ctx => anim.SetInteger("Jumping",1);

        controls.Gameplay.Dash.performed += ctx => walkSpeed = runSpeed;

        controls.Gameplay.Interact.performed += ctx => Interact(true);
        controls.Gameplay.Interact.canceled += ctx => Interact(false);
 
    }

    // Get Components Required For This Script To Function
    private void GetPrerequisiteComponents () 
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    private void SetInstance () 
    {
        if (_instance == null) 
        {
            _instance = this;
        } 
        else 
        {
            Debug.Log ("Instance of Player already exists. Destroying this one");
            Destroy (this);
        }
    }

    
    private void Move ()                                                                                            // Move Player Taking Into Acount Camera Rotation
    {   
        if(!isMounted)                                                                                              // Checking if the Character is on the hoverboard so it doesn't move while mounted
        {   
            Vector3 moveDirection = new Vector3(move.x, 0, move.y);
            var camForward = cam.transform.forward;
            camForward.y = 0.0f;
            camForward.Normalize();
            
            // Get the part of camera right that is on the XZ plane as a normalzied vector
            var camRight = cam.transform.right;
            camRight.y = 0.0f;
            camRight.Normalize();
            
            // Save 'y' direction for jumping but reset X and Z
            moveDirection.x = 0;
            moveDirection.z = 0;
            
            // Calculate new move direction
            moveDirection += move.y * camForward;
            moveDirection += move.x * camRight;
             
            //moveDirection = cam.transform.TransformDirection(moveDirection); //!!!!!!!!!!!!!!THIS IS THE PROBLEM !!!!!!!!                               // Moving towards where the camera is looking
            moveDirection *= walkSpeed ;                                      // Giving movement
            
            if(isGrounded)
            {   
                anim.SetInteger("Jumping",0);
                gravity += Physics.gravity* Time.deltaTime;
                if (walkSpeed == runSpeed)
                {
                    anim.SetInteger("Running",1);
                }
                else
                {
                    anim.SetInteger("Running",0);
                }
            }
            else                                                                                         // Checking if the Character is on the ground
            {
                gravity += Physics.gravity* Time.deltaTime; 
                if(canJump)
                {   
                    canDoubleJump = true;
                    canJump = false;
                }                                                                                        // Simulating gravity on y axis   
            }
            moveDirection.y += gravity.y; 
            //moveDirection.Normalize();                                                                                      
            controller.Move(moveDirection * Time.deltaTime);                                                                     // Moving the Character Controller
            if ((Mathf.Abs (move.x) == 0) && (Mathf.Abs (move.y) == 0))
            {   
                anim.SetInteger("Walking", 0); 
                return;
            }
            else
            {   
                moveDirection.y = 0;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(moveDirection), 0.1f);   // Rotating the character controller towards where is moving
                anim.SetInteger("Walking", 1);                                                                                       // Enabling the walking animation
            }
            
        }
        
    }

    private void Jump()
    {
        
        if(isGrounded)
        {
            isGrounded = false;
            gravity.y = verticalSpeed*jumpHeight;
            canJump = false;
            canDoubleJump = true;   
        }
        else if(!isGrounded && canDoubleJump)
        {                                                                           // Setting Vertical Speed to zero    
            gravity.y += verticalSpeed*(jumpHeight/2);
            canJump = false;
            canDoubleJump = false;    
        }
    }
    void Interact(bool b)
    {       
        if(isMounted)
        {
            Dismount();
            anim.SetInteger("Surfing",0);
            return;
        }
        interact = b;
    }
    #region interface Mountable
    public void Mount()
    {   
        controller.enabled = false;
        isMounted = true;
        anim.SetInteger("Surfing",1);
    }

    public void Dismount()
    {
        MountedOn.GetComponent<HoverboardTrigger>().Dismount();
        controller.enabled = true;
        isMounted = false;
        MountedOn = null;
        transform.parent = null;
    }

    #endregion

}