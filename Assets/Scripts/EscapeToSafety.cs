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
    [SerializeField] private float speed = 300.0f;
    [SerializeField] private float maxSqrtVelocity = 700.0f;
    private bool escaping;
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

        objectAnim.SetBool(isWalkingHash, false);
        objectAnim.SetBool(isRunningHash, false);

        escaping = false;
        nextCluster = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FindNewCluster();
        EscapeToCluster();
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

                nextCluster = -1;
                escaping = false;
            }
        }
    }
}
