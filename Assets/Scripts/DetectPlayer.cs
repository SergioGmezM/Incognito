using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Patrol policemanPatrol;
    private Rigidbody policemanRB;
    private Animator objectAnim;
    private int isWalkingHash;
    private int isRunningHash;
    [SerializeField] private float detectionDistance;
    [SerializeField] private float detectionAngle;
    private bool detected = false;
    private bool playerCaught = false;
    private bool isPlayerIncognito = false;
    private float speed;
    private float maxSqrtVelocity;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        policemanPatrol = GetComponent<Patrol>();
        policemanRB = GetComponent<Rigidbody>();
        objectAnim = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        objectAnim.SetBool(isWalkingHash, false);

        speed = policemanPatrol.GetSpeed();
        maxSqrtVelocity = policemanPatrol.GetMaxSqrtVelocity() * 3;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = playerTransform.position - transform.position;
        float angle = Vector3.Angle(targetDirection, transform.forward);
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        isPlayerIncognito = playerController.incognito;

        if (!playerCaught && distance < detectionDistance && angle < detectionAngle && !isPlayerIncognito)
        {
            detected = true;
            policemanPatrol.StopPatrolling();
        }

        if (!playerCaught && detected && isPlayerIncognito)
        {
            policemanPatrol.ResumePatrolling();
        }
    }

    private void FixedUpdate()
    {
        if (!playerCaught && detected && !isPlayerIncognito)
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
        if (collision.gameObject.CompareTag("Player"))
        {
            // Game should be over here
            objectAnim.SetBool(isRunningHash, false);
            policemanRB.velocity = Vector3.zero;
            playerCaught = true;
        }
    }
}
