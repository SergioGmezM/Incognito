using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    private Transform playerTransform;
    private PlayerController playerController;
    [SerializeField] private float detectionDistance;
    [SerializeField] private float detectionAngle;
    private bool detected = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = playerTransform.position - transform.position;
        float angle = Vector3.Angle(targetDirection, transform.forward);
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        bool isPlayerIncognito = playerController.incognito;

        if (distance < detectionDistance && angle < detectionAngle && !isPlayerIncognito)
        {
            detected = true;
        }
    }
}
