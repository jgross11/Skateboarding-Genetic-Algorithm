using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardScript : MonoBehaviour
{
    // fields
    private Vector3 initialPos;
    private Quaternion initialRotation;
    private Rigidbody rgbd;
    private float runCounter;
    public float runTime;
    private int wheelsOnGround;
    private const int NUM_WHEELS = 4;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialRotation = transform.rotation;
        runCounter = 0;
        rgbd = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(runCounter < runTime){
            runCounter += Time.deltaTime;
        }
        else{
            reset();
            runCounter = 0;
        }

    }

    public void UpdateWheelCount(int value){
        wheelsOnGround += value;
        Debug.Log("board " + GetInstanceID() + " has " + wheelsOnGround + " wheel(s) on ground");
    }

    void reset()
    {
        // disable physics to wipe velocity, torque, etc.
        rgbd.isKinematic = true;

        // reset position to start
        transform.position = initialPos;

        // reset rotation to start
        transform.rotation = initialRotation;

        // enable physics for next run
        rgbd.isKinematic = false;
    }
}
