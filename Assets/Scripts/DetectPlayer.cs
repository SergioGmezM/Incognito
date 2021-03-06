using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public AudioClip hitSound;

    private GameManager gameManager;
    private Transform playerTransform;
    private PlayerController playerController;
    private Patrol policemanPatrol;
    private Rigidbody policemanRB;
    private Animator objectAnim;
    private AudioSource objectAudio;
    private int isWalkingHash;
    private int isRunningHash;
    private int isDeadHash;
    private int attackTrigHash;
    [SerializeField] private float detectionDistance;
    [SerializeField] private float detectionAngle;
    private bool detected = false;
    private bool playerCaught = false;
    private bool isPlayerIncognito = false;
    private float speed;
    private float maxSqrtVelocity;
    [SerializeField] float maxWaitTime = 2.0f;
    private float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        policemanPatrol = GetComponent<Patrol>();
        policemanRB = GetComponent<Rigidbody>();
        objectAnim = GetComponent<Animator>();
        objectAudio = GetComponent<AudioSource>();
        
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isDeadHash = Animator.StringToHash("isDead");
        attackTrigHash = Animator.StringToHash("attackTrig");

        objectAnim.SetBool(isWalkingHash, false);

        speed = policemanPatrol.GetSpeed();
        maxSqrtVelocity = policemanPatrol.GetMaxSqrtVelocity() * 3;

        waitTime = maxWaitTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.IsGameActive())
        {
            Vector3 targetDirection = playerTransform.position - transform.position;
            float angle = Vector3.Angle(targetDirection, transform.forward);
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            isPlayerIncognito = playerController.IsPlayerIncognito();

            if (!playerCaught && distance < detectionDistance && angle < detectionAngle && !isPlayerIncognito)
            {
                detected = true;
                policemanPatrol.StopPatrolling();
                // Player needs to run
                gameManager.SetPlayerStatus(2);
            }

            if (!playerCaught && detected && isPlayerIncognito)
            {
                if (waitTime <= 0)
                {
                    objectAnim.SetBool(isRunningHash, true);
                    policemanPatrol.ResumePatrolling();
                    // Player is incognito
                    gameManager.SetPlayerStatus(0);
                    waitTime = maxWaitTime;
                    detected = false;
                }
                else
                {
                    objectAnim.SetBool(isRunningHash, false);
                    waitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.IsGameActive() && !playerCaught && detected && !isPlayerIncognito)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        transform.LookAt(playerTransform);
        objectAnim.SetBool(isRunningHash, true);

        if (policemanRB.velocity.sqrMagnitude < maxSqrtVelocity)
        {
            policemanRB.AddRelativeForce(Vector3.forward * speed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameManager.IsGameActive() && collision.gameObject.CompareTag("Player") && !isPlayerIncognito)
        {
            objectAnim.SetBool(isRunningHash, false);
            objectAnim.SetTrigger(attackTrigHash);
            policemanRB.velocity = Vector3.zero;
            objectAudio.PlayOneShot(hitSound);
            collision.gameObject.GetComponent<Animator>().SetBool(isDeadHash, true);
            playerCaught = true;
            gameManager.GameOver();
        }
    }
}
