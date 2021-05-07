using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool incognito = false;

    private Animator playerAnim;
    private Rigidbody playerRB;
    private int isWalkingHash;
    private int isRunningHash;
    [SerializeField] private float speed = 100.0f;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float maxSqrtVelocity = 500.0f;
    private bool isWalking;
    private bool isRunning;
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        direction = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        isWalking = playerAnim.GetBool(isWalkingHash);
        isRunning = playerAnim.GetBool(isRunningHash);
        bool WASDPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        if (Input.GetKey(KeyCode.W))
        {
            direction = Vector3.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction = Vector3.left;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction = Vector3.back;
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction = Vector3.right;
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            direction = new Vector3(-1, 0, 1);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            direction = new Vector3(1, 0, 1);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            direction = new Vector3(-1, 0, -1);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            direction = new Vector3(1, 0, -1);
        }

        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        // Idle to Walking
        if (!isWalking && WASDPressed)
        {
            playerAnim.SetBool(isWalkingHash, true);
            playerAnim.SetBool(isRunningHash, false);
        }
        // Walking to Idle
        else if (isWalking && !WASDPressed)
        {
            playerAnim.SetBool(isWalkingHash, false);
            playerAnim.SetBool(isRunningHash, false);
        }

        // Walking to Running
        if (!isRunning && WASDPressed && runPressed)
        {
            playerAnim.SetBool(isWalkingHash, true);
            playerAnim.SetBool(isRunningHash, true);
        }
        // Running to Walking
        else if (isRunning && WASDPressed && !runPressed)
        {
            playerAnim.SetBool(isWalkingHash, true);
            playerAnim.SetBool(isRunningHash, false);
        }

        // Idle to Running
        if (!isWalking && !isRunning && WASDPressed && runPressed)
        {
            playerAnim.SetBool(isWalkingHash, false);
            playerAnim.SetBool(isRunningHash, true);
        }
        // Running to Idle
        else if (!isWalking && isRunning && !WASDPressed && !runPressed)
        {
            playerAnim.SetBool(isWalkingHash, false);
            playerAnim.SetBool(isRunningHash, false);
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        //Rotate smoothly to this target:
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
    }

    private void FixedUpdate()
    {
        if (isWalking)
        {
            if (playerRB.velocity.sqrMagnitude < maxSqrtVelocity)
            {
                playerRB.AddRelativeForce(Vector3.forward * speed);
            }
        }

        if (isRunning)
        {
            if (playerRB.velocity.sqrMagnitude < (maxSqrtVelocity * 2))
            {
                playerRB.AddRelativeForce(Vector3.forward * speed);
            }
        }
    }
}
