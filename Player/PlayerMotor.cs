using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMotor : Bolt.EntityBehaviour<IPlayer>
{
    private CharacterController Controller;

    [SerializeField]
    private Camera Cam;

    [SerializeField]
    private float WalkingSpeed;

    [SerializeField]
    private float RunningSpeed;

    private float ActualSpeed;

    private Vector3 GravitationalVelocity;
    private float GravitationalConstant = 98f;

    [SerializeField]
    private float JumpPower;

    [SerializeField]
    private bool Jumped;

    [SerializeField]
    private bool IsRunning;

    [SerializeField]
    private bool SetFootstep = true;

    [SerializeField]
    private float LookSensitivity;

    [SerializeField]
    private bool IsGrounded;

    private int FootstepsTimer = 15;

    private int CurrentFootstepTime;

    [SerializeField]
    private AudioSource FootSteps;

    public override void Attached()
    {
        state.SetTransforms(state.PlayerTransform, transform);
        Controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        IsGrounded = Controller.isGrounded;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Jumped == false)
            {
                ActualSpeed = RunningSpeed;
                IsRunning = true;
            }
        }
        else
        {
            ActualSpeed = WalkingSpeed;
            IsRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && Controller.isGrounded && Jumped == false)
        {
            Jump();
        }       
    }

    public override void SimulateOwner()
    {
        if (Controller.isGrounded != true)
        {
            GravitationalVelocity.y -= GravitationalConstant * Time.deltaTime;
        }
        else if (Jumped == true)
        {
            GravitationalVelocity.y += JumpPower;
            Jumped = false;
        }
        else
        {
            GravitationalVelocity.y = -1f;
        }

        if(IsRunning == true)
        {
            if (SetFootstep == true)
            {
                FootSteps.Play();
                SetFootstep = false;
            }                        
        }

        if(SetFootstep == false)
        {
            CurrentFootstepTime--;

            if(CurrentFootstepTime <= 0)
            {
                SetFootstep = true;
                CurrentFootstepTime = FootstepsTimer;
            }
        }

        float xMovement = Input.GetAxisRaw("Horizontal");
        float zMovement = Input.GetAxisRaw("Vertical");
        float yRotation = Input.GetAxisRaw("Mouse X");
        float xRotation = Input.GetAxisRaw("Mouse Y");

        Vector3 xVector = transform.right * xMovement;
        Vector3 zVector = transform.forward * zMovement;

        Vector3 yVector_r = new Vector3(0f, yRotation, 0f) * LookSensitivity;
        Vector3 xVector_r = new Vector3(-xRotation, 0f, 0f) * LookSensitivity;

        Vector3 Velocity = (xVector + zVector).normalized * ActualSpeed;

        Move(Velocity, GravitationalVelocity);
        Rotate(yVector_r, xVector_r);
    }

    private void Move(Vector3 Velocity, Vector3 GravitationalVelocity)
    {
        Controller.Move(Velocity * BoltNetwork.FrameDeltaTime);

        Controller.Move(GravitationalVelocity * BoltNetwork.FrameDeltaTime);
    }

    private void Rotate(Vector3 Rotation, Vector3 Camera_Rotation)
    {
        transform.Rotate(transform.rotation * Rotation);
        Cam.transform.Rotate(Camera_Rotation);
    }

    private void Jump()
    {
        Jumped = true;
    }
}
