using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private static readonly Vector3 DEF_DEST_POSITION = Vector3.zero;
    private static readonly float WALK_DISTANCE_CAP = 0.07f;
    private static readonly float RUNNING_DISTANCE_CAP = 0.4f;


    private Vector3 destPos = DEF_DEST_POSITION;
    public Vector3 Destination
    {
        private get { return destPos; }
        set { destPos = value; }
    }

    private Animator animator;
    public float walkingSpeed = 1f;
    public float runningSpeed = 2f;
    public float rotatingSpeed = 15f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (destPos != DEF_DEST_POSITION && !MoveCharacter(destPos))
        {
            destPos = DEF_DEST_POSITION;
        }
    }

    private bool MoveCharacter(Vector3 target)
    {
        // rotate character
        Vector3 targetDirection = target - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotatingSpeed * Time.deltaTime, .0f);
        newDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(newDirection);

        // move character if done rotating
        targetDirection.y = 0;
        float angle = Vector3.Angle(targetDirection, newDirection);
        if (angle < 10)
        {
            // determine moving speed
            float distance = Vector3.Distance(transform.position, target);
            float movingSpeed = 0;
            if (distance > WALK_DISTANCE_CAP)
            {
                movingSpeed = distance > RUNNING_DISTANCE_CAP ? runningSpeed : walkingSpeed;
            }
            SetMovingSpeed(movingSpeed);

            // move to desired position
            newDirection = Vector3.MoveTowards(transform.position, target, movingSpeed / 4 * Time.deltaTime);
            transform.position = newDirection;

            return movingSpeed > 0;
        }

        // still rotating
        return true;
    }

    private void SetMovingSpeed(float speed = 0)
    {
        string speedName = "MoveSpeed";
        if (animator.GetFloat(speedName) != speed)
        {
            // print(speed);
            animator.SetFloat(speedName, speed);
        }
    }
}
