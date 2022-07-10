using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundDistance = 0.4f;
    [SerializeField]
    private LayerMask groundMask;

    private float halfScreenWidth;
    private int leftFingerId, rightFingerId;
    private Vector2 moveInput;
    private Vector2 moveTouchStartPosition;
    private Vector2 lookInput;
    private float cameraSensitivity;
    private float cameraPitch;
    public Transform characterCamera;

    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool isGrounded = true;
    private float jumpHeight = 2f;

    // Start is called before the first frame update
    void Start()
    {
        leftFingerId = -1;
        rightFingerId = -1;
        halfScreenWidth = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        GetTouchInput();
        if(leftFingerId != -1)
        {
            MovePlayer();
        }

        if(rightFingerId != -1)
        {
            LookAround();
        }
    }

    private void MovePlayer()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        } 

        Vector3 move = transform.right * moveInput.normalized.x + transform.forward * moveInput.normalized.y;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);


        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);    
        }
    }

    void GetTouchInput()
    {
       for(int i = 0; i < Input.touchCount; i++)
       {
            Touch t = Input.GetTouch(i);
            
            if(t.phase == TouchPhase.Began)
            {
                if (t.position.x < halfScreenWidth && leftFingerId == -1)
                {
                    leftFingerId = t.fingerId;
                    moveTouchStartPosition = t.position;
                }
                if(t.position.x > halfScreenWidth && rightFingerId == -1)
                {
                    rightFingerId = t.fingerId;
                }
            }
                   
            if (t.phase == TouchPhase.Moved)
            {
                if(leftFingerId == t.fingerId)
                {
                    moveInput = t.position = moveTouchStartPosition;
                }
                if(rightFingerId == t.fingerId)
                {
                    lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                }
            }

            if (t.phase == TouchPhase.Stationary)
            {
                if (rightFingerId == t.fingerId)
                {
                    lookInput = Vector2.zero;
                }
            }
            if (t.phase == TouchPhase.Canceled)
            {

            }
            if (t.phase == TouchPhase.Ended)
            {
                if(leftFingerId == t.fingerId)
                {
                    leftFingerId = -1;
                }
                if (rightFingerId == t.fingerId)
                {
                    rightFingerId = -1;
                }
            }
            
       }
    }
    void LookAround()
    {
        cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
        characterCamera.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(transform.up, lookInput.x);
    }
}
