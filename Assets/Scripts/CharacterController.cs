using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private static readonly Vector3 DEF_DEST_POSITION = Vector3.zero;
    private static readonly float WALK_DISTANCE_CAP = 0.07f;
    private static readonly float RUNNING_DISTANCE_CAP = 0.4f;

    private enum CLIMB_STATE { GROUND, CLIMBING, CLIMBING_UP };
    CLIMB_STATE climbState = CLIMB_STATE.GROUND;

    private Vector3 destPos = DEF_DEST_POSITION;
    public Vector3 Destination
    {
        private get { return destPos; }
        set { destPos = value; }
    }

    private Animator animator;
    private float climbUpAnimDuration = -1;

    public float speed = .25f;
    private float walkSpeed = 1f;
    private float runSpeed = 2f;
    private float rotateSpeed = 60f;
    private float climbSpeed = .3f;

    private bool hittingObstacle = false;
    private bool isClimbing = false;

    public float RayHitDistance = -1;
    

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private float debugTime = 0;
    void Update()
    {
        if (destPos != DEF_DEST_POSITION && !MoveCharacter(destPos))
        {
            destPos = DEF_DEST_POSITION;
        }

        // debugging
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position + new Vector3(1, 9, 0), forward, Color.green);
        if ((debugTime += Time.deltaTime) > 5) {
            debugTime = 0;
            print(transform.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        print("Collision: " + collision.transform.name);
        if (destPos != DEF_DEST_POSITION && climbState == CLIMB_STATE.GROUND)
        {
            hittingObstacle = true;
        }
    }

    public void OnCollisionStay(Collision c)
    {
        // print("Collision stay: " + c.contactCount + " " + c.transform.gameObject.name);
    }

    private bool MoveCharacter(Vector3 target)
    {
        if (climbState != CLIMB_STATE.GROUND)
        {
            if (!ClimbCharacter(target))
            {
                SetClimbingState(CLIMB_STATE.GROUND);
            }
            return true;
        }
        else
        {
            float angle = RotateCharacter(target);
            if (angle < 1)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
                {
                    RayHitDistance = hit.distance;
                }
                else 
                {
                    RayHitDistance = -1;
                }

                if (hittingObstacle)
                {
                    hittingObstacle = false;
                    if (target.y - transform.position.y > 1.5f * transform.localScale.y)
                    {
                        print(target.y - transform.position.y);
                        SetClimbingState(CLIMB_STATE.CLIMBING);
                        return true;
                    }
                }

                return TranslateCharacter(target);
            }

            // still rotating
            return true;
        }
    }

    private bool ClimbCharacter(Vector3 target)
    {
        float hDiff = target.y - transform.position.y;
        if (hDiff <= transform.localScale.y)
        {
            transform.position = new Vector3(transform.position.x, target.y, transform.position.z);
            return false;
        }

        print(transform.localScale.y + " " + hDiff);
        if (hDiff < transform.localScale.y * 3)
        {
            SetClimbingState(CLIMB_STATE.CLIMBING_UP);
        }

        float step = climbSpeed * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y + step, transform.position.z);

        return true;
    }

    private bool SetClimbingState(CLIMB_STATE state)
    {
        climbState = state;
        if (animator.GetInteger("ClimbState") != (int)climbState)
        {
            print((int)climbState);
            animator.SetInteger("ClimbState", (int)climbState);
            return true;
        }
        return false;
    }

    private float RotateCharacter(Vector3 target)
    {
        Vector3 targetDirection = target - transform.position;
        targetDirection.y = 0;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * speed * Time.deltaTime, .0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        targetDirection.y = 0;
        return Vector3.Angle(targetDirection, newDirection);
    }

    private bool TranslateCharacter(Vector3 target)
    {
        // only move on ground
        target.y = transform.position.y;

        // determine current speed
        float moveSpeed = GetMoveSpeed(target);
        SetMoveSpeed(moveSpeed);

        // move to desired position
        Vector3 newDirection = Vector3.MoveTowards(transform.position, target, moveSpeed * speed * Time.deltaTime);
        transform.position = newDirection;

        return moveSpeed > 0;
    }

    private float GetMoveSpeed(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);
        float moveSpeed = 0;
        if (distance > WALK_DISTANCE_CAP)
        {
            moveSpeed = distance > RUNNING_DISTANCE_CAP ? runSpeed : walkSpeed;
        }
        return moveSpeed;
    }

    private void SetMoveSpeed(float speed = 0)
    {
        string speedName = "MoveSpeed";
        if (animator.GetFloat(speedName) != speed)
        {
            // print(speed);
            animator.SetFloat(speedName, speed);
        }
    }
}
