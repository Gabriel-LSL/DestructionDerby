using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.5f;

    private Transform myTransform;

    private Vector3 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        moveDir = Random.insideUnitSphere.normalized;
        moveDir = new Vector3(moveDir.x, 0.0f, moveDir.z);
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.position += moveDir * moveSpeed * Time.deltaTime;
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


    private void Bounce(Vector3 normal)
    {
        moveDir = (-moveDir + (2.0f * (moveDir + normal))).normalized;
    }

}
