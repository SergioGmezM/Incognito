using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    private Animator objectAnim;
    private int isDeadHash;
    private bool isDead;

    private void Start()
    {
        objectAnim = GetComponent<Animator>();
        isDeadHash = Animator.StringToHash("isDead");
        isDead = false;
    }

    private void Update()
    {
        isDead = objectAnim.GetBool(isDeadHash);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead &&
            (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("Policeman") ||
            collision.gameObject.CompareTag("Civilian")))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
}
