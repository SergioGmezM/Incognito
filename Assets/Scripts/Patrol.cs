using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    private const float THRESHOLD = 2.0f;

    public Transform[] patrolPoints;

    private GameManager gameManager;
    private Rigidbody policemanRB;
    private Animator objectAnim;
    private int isWalkingHash;
    private int isRunningHash;
    [SerializeField] private float speed = 200.0f;
    [SerializeField] private float maxSqrtVelocity = 600.0f;
    [SerializeField] private float startWaitTime = 3.0f;
    private float waitTime;
    private int nextPoint;
    private bool pointReached;
    private bool stopPatrolling;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
        policemanRB = GetComponent<Rigidbody>();
        objectAnim = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        objectAnim.SetBool(isWalkingHash, false);
        objectAnim.SetBool(isRunningHash, false);

        waitTime = startWaitTime;
        nextPoint = -1;
        pointReached = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameManager.IsGameActive() && !stopPatrolling)
        {
            PatrolScene();
        }
    }

    private void PatrolScene()
    {
        if (pointReached)
        {
            if (waitTime < 0)
            {
                nextPoint++;
                nextPoint = nextPoint % patrolPoints.Length;
                waitTime = startWaitTime;
                pointReached = false;

                transform.LookAt(patrolPoints[nextPoint]);

                // Idle to Running
                objectAnim.SetBool(isRunningHash, true);
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, patrolPoints[nextPoint].position) > THRESHOLD)
            {
                transform.LookAt(patrolPoints[nextPoint]);

                if (policemanRB.velocity.sqrMagnitude < maxSqrtVelocity)
                {
                    policemanRB.AddRelativeForce(Vector3.forward * speed);
                }
                else
                {
                    policemanRB.velocity /= 2;
                }
            }
            else
            {
                // Running to Idle
                objectAnim.SetBool(isRunningHash, false);

                policemanRB.velocity = Vector3.zero;

                pointReached = true;
            }
        }
    }

    public void StopPatrolling()
    {
        pointReached = true;
        waitTime = -1.0f;
        stopPatrolling = true;
    }

    public void ResumePatrolling()
    {
        stopPatrolling = false;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetMaxSqrtVelocity()
    {
        return maxSqrtVelocity;
    }
}
