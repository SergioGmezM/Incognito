using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeToSafety : MonoBehaviour
{
    private const float THRESHOLD = 3.5f;

    public Transform[] clusterPoints;

    private GameManager gameManager;
    private Rigidbody objectRB;
    private Animator objectAnim;
    private int isWalkingHash;
    private int isRunningHash;
    private int isDeadHash;
    [SerializeField] private float speed = 300.0f;
    [SerializeField] private float maxSqrtVelocity = 700.0f;
    private bool escaping;
    private int currentCluster;
    private int nextCluster;
    private int previousMurderCluster;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();
        objectRB = GetComponent<Rigidbody>();
        objectAnim = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isDeadHash = Animator.StringToHash("isDead");

        objectAnim.SetBool(isWalkingHash, false);
        objectAnim.SetBool(isRunningHash, false);

        escaping = false;
        nextCluster = -1;

        currentCluster = 0;
        float minDistance = Vector3.Distance(transform.position, clusterPoints[0].position);
        for (int i = 1; i < clusterPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, clusterPoints[i].position);
            if (distance < minDistance)
            {
                currentCluster = i;
                minDistance = distance;
            }
        }
    }

    private void Update()
    {
        if (objectAnim.GetBool(isDeadHash))
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FindNewCluster();
        EscapeToCluster();
    }

    public int GetCurrentCluster()
    {
        return this.currentCluster;
    }

    private void FindNewCluster()
    {
        if (!escaping)
        {
            int murderAtCluster = gameManager.GetMurderCommitted();

            if (murderAtCluster != -1 && murderAtCluster != previousMurderCluster)
            {
                escaping = true;
                previousMurderCluster = murderAtCluster;

                nextCluster = Random.Range(0, clusterPoints.Length);
                objectAnim.SetBool(isRunningHash, true);

                while (nextCluster == murderAtCluster)
                {
                    nextCluster = Random.Range(0, clusterPoints.Length);
                }
            }
        }
    }

    private void EscapeToCluster()
    {
        if (nextCluster != -1)
        {
            if (Vector3.Distance(transform.position, clusterPoints[nextCluster].position) > THRESHOLD)
            {
                transform.LookAt(clusterPoints[nextCluster]);

                if (objectRB.velocity.sqrMagnitude < maxSqrtVelocity)
                {
                    objectRB.AddRelativeForce(Vector3.forward * speed);
                }
                else
                {
                    objectRB.velocity /= 2;
                }
            }
            else
            {
                // Running to Idle
                objectAnim.SetBool(isRunningHash, false);

                objectRB.velocity = Vector3.zero;

                currentCluster = nextCluster;
                nextCluster = -1;
                escaping = false;
            }
        }
    }
}
