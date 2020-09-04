using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField]
    private float speed = 2.0f;

    //use this to modify the speed.
    private float tempSpeed;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float stopRollSpeedPercentage = 0.01f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float allowVirtualJoyStickPercentage = 0.5f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float frictionCoefficient = 0.001f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float forceLossOnCollision = 0.01f;

    private float inverseFriction;
    private float inverseForceLossOnCollision;

    //To prevent robot from instantly rolling at the start of the game since the virtual joystick is not activated at the start
    private bool toggleInteractVirtualJoystick = false;
    //To control if the robot is rolling.
    private bool isRolling = false;

    private Animator animator = null;
    private VirtualJoystick vJoyStick = null;
    private Transform myTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        inverseFriction = 1.0f - frictionCoefficient;
        inverseForceLossOnCollision = 1.0f - forceLossOnCollision;

        animator = GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogError("Attach an animator component to object!!!");
        }

        vJoyStick = VirtualJoystick.Instance;
        if(vJoyStick == null)
        {
            Debug.LogError("Can't find virtual joystick!!! Something is wrong!!!");
        }

        myTransform = transform;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        if(isRolling)
        {
            vJoyStick.CanBeActivated = false;
            if(tempSpeed < speed * allowVirtualJoyStickPercentage)
            {
                vJoyStick.CanBeActivated = true;
            }

            tempSpeed *= inverseFriction;
            myTransform.position += myTransform.forward * tempSpeed * Time.deltaTime;

            animator.SetBool("Roll_Anim", true);
            animator.SetFloat("Roll_Speed_Multiplier", tempSpeed);

            if (tempSpeed < stopRollSpeedPercentage * speed)
            {
                isRolling = false;
            }
        }
        else
        {
            if(!vJoyStick.IsActivated)
            {
                animator.SetBool("Roll_Anim", false);
            }
        }
    }


    private void UpdateMovement()
    {
        if(vJoyStick.IsActivated)
        {
            animator.Play("anim_open_GoToRoll", 0, vJoyStick.NormalizedMagnitude);
            Vector3 forwardVector = new Vector3(-vJoyStick.NormalizedDir.x, 0.0f, -vJoyStick.NormalizedDir.y);

            //Preventing weird edge cases where forward vector is zero.
            if(forwardVector.magnitude > 0.0f)
            {
                myTransform.forward = forwardVector;
            }
            
            toggleInteractVirtualJoystick = true;
            isRolling = false;
        }
        else
        {
            if(toggleInteractVirtualJoystick)
            {
                tempSpeed = speed * vJoyStick.NormalizedMagnitude;
                
                toggleInteractVirtualJoystick = false;
                isRolling = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider collisionCollider = collision.collider;
        if(collisionCollider.CompareTag("Wall"))
        {
            Bounce(collision.GetContact(0).normal);
        }
        if(collisionCollider.CompareTag("Enemy"))
        {
            Bounce(collision.GetContact(0).normal);
        }
    }

    private void Bounce(Vector3 collisionNormal)
    {
        Vector3 halfDirection = myTransform.forward + collisionNormal;
        myTransform.forward = -myTransform.forward + (2 * halfDirection);
        
        //For cases when the enemy touches the player when not rolling.
        if(!isRolling)
        {
            tempSpeed = 1.0f;
            isRolling = true;
            animator.Play("closed_Roll_Loop", 0);
        }
        else
        {
            tempSpeed *= inverseForceLossOnCollision;
        }
    }
}
