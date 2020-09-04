using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateThis : MonoBehaviour
{
    public bool clockWise = false;
    public float speed = 100.0f;

    private Transform myTransform = null;
    private float spinDirection = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        spinDirection = clockWise ? 1.0f : -1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        myTransform.Rotate(new Vector3(0.0f, Time.deltaTime * speed * spinDirection, 0.0f));
    }
}
