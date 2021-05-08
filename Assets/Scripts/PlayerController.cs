using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private Animator playerAnim;
    private Rigidbody playerRB;
    private int isWalkingHash;
    private int isRunningHash;
    [SerializeField] private float speed = 100.0f;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float maxSqrtVelocity = 500.0f;
    [SerializeField] private float cooldownTime = 5;
    [SerializeField] private bool incognito = false;
    private bool isWalking;
    private bool isRunning;
    private bool canAttack;
    private Vector3 direction;
    private float cooldownElapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
        playerAnim = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        direction = Vector3.forward;
        canAttack = true;
        cooldownElapsedTime = cooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.IsGameActive())
        {
            MovePlayer();

            DetectNearbyCivilians();

            if (!canAttack)
            {
                if (cooldownElapsedTime < 0)
                {
                    canAttack = true;
                    cooldownElapsedTime = cooldownTime;
                }
                else
                {
                    cooldownElapsedTime -= Time.deltaTime;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.IsGameActive())
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

    public bool IsPlayerIncognito()
    {
        return this.incognito;
    }

    private void MovePlayer()
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

        // Rotate smoothly to this target:
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
    }

    private void DetectNearbyCivilians()
    {
        // Detect nearby colliders
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1);
        // Two of them are the player and the ground respectively
        if (hitColliders.Length >= 4)
        {
            incognito = true;
            gameManager.SetPlayerStatus(0);

            // Show attack button on UI

            // Commit murder
            if (canAttack)
            {
                AttackCivilian(hitColliders);
            }
        }
        else if (hitColliders.Length >= 3)
        {
            // Show attack button on UI

            // Commit murder
            if (canAttack)
            {
                AttackCivilian(hitColliders);
            }
        }
        else
        {
            incognito = false;
            gameManager.SetPlayerStatus(1);
        }
    }

    private void AttackCivilian(Collider[] hitColliders)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            int nearestCivilian = 0;
            float minDistance = 100;

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (!hitColliders[i].gameObject.CompareTag("Player") && !hitColliders[i].gameObject.CompareTag("Ground"))
                {
                    float distance = Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position);
                    if (distance < minDistance)
                    {
                        nearestCivilian = i;
                        minDistance = distance;
                    }
                }
            }

            // Play meelee attack for player
            // Play death animation for civilian
            // Deactivate its collider
            canAttack = false;
            int murderCommitted = hitColliders[nearestCivilian].gameObject.GetComponent<EscapeToSafety>().GetCurrentCluster();
            gameManager.SetMurderCommitted(murderCommitted);

            Destroy(hitColliders[nearestCivilian].gameObject);
        }
    }
}
