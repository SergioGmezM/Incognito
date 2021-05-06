using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float x = playerTransform.position.x + offset.x;
        float y = playerTransform.position.y + offset.y;
        float z = playerTransform.position.z + offset.z;
        transform.position = new Vector3(x, y, z);
    }
}
