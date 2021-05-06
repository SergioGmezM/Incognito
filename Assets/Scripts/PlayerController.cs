using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator playerAnim;
    private int isWalkingHash;
    private int isRunningHash;

    // Start is called before the first frame update
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = playerAnim.GetBool(isWalkingHash);
        bool isRunning = playerAnim.GetBool(isRunningHash);
        bool WASDPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
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
    }
}
