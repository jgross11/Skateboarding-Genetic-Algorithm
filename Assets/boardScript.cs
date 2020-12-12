using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private bool boardOnGround = true;
    private float airtime = 0.0f;
    private int boardID = -1;
    private float fitness = 0.0f;
    private static float bestAirtime = 0.0f;
    private List<Vector3> rotationsInAir;
    private List<Vector3> positionsInAir;
    private BoardData boardData;
    private float firstLastWheelDelta = 0.0f;
    private bool isAWheelOnGround = false;
    public Specimen specimen;
    

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialRotation = transform.rotation;
        runCounter = 0;
        rgbd = GetComponent<Rigidbody>();
        rotationsInAir = new List<Vector3>();
        positionsInAir = new List<Vector3>();
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

        // all wheels just hit ground
        if(!boardOnGround && wheelsOnGround == NUM_WHEELS){
            boardOnGround = true; 
            Debug.Log("board " + GetInstanceID() + " in air for " + airtime + " seconds");
            Debug.Log("time between first and last wheel hitting ground was " + firstLastWheelDelta + " seconds");
            
            /*
            foreach(Vector3 info in rotationsInAir){
                Debug.Log(info);
            }
            foreach(Vector3 info in positionsInAir)
            {
                Debug.Log(info);
            }
            */
            boardData = GenerateBoardData();
            Debug.Log(boardData.ToString());
            Debug.Log("fitness: " + specimen.CalculateFitness(boardData));
            Debug.Log(boardData.minXAngle);
            Debug.Log(boardData.minYAngle);
            Debug.Log(boardData.minZAngle);
            airtime = 0;
            rotationsInAir = new List<Vector3>();
            positionsInAir = new List<Vector3>();

        }
        else if(isAWheelOnGround){
            firstLastWheelDelta += Time.deltaTime;
        }
        // all wheels just left ground
        else if(boardOnGround && wheelsOnGround == 0){
            Debug.Log("board " + GetInstanceID() + " in air");
            boardOnGround = false;
            firstLastWheelDelta = 0;
        // all wheels have been off ground for some time
        } else if(wheelsOnGround == 0){
            airtime += Time.deltaTime;
            rotationsInAir.Add(transform.rotation.eulerAngles);
            positionsInAir.Add(transform.position);
        }
    }

    /*
    Updates the number of wheels currently on the ground
    Called when a wheel leaves the ground (value = -1)
    or when a wheel hits the ground (value = 1)
    by the ground's groundScript
    */
    public void UpdateWheelCount(int value){
        wheelsOnGround += value;
        isAWheelOnGround = wheelsOnGround > 0;
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

    private float calculateFitness(){
        return 0.0f;
    }

    private BoardData GenerateBoardData(){
        BoardData data = new BoardData();

        // calculate maximum rotation variance 
        // TODO this probably isn't correct.
        float currentMaxX = 0;
        float currentMaxY = 0;
        float currentMaxZ = 0;
        foreach(Vector3 rotData in rotationsInAir)
        {
            float currentX = (float)Math.Sqrt(Math.Abs(rotData.x));
            float currentY = (float)Math.Sqrt(Math.Abs(rotData.y));
            float currentZ = (float)Math.Sqrt(Math.Abs(rotData.z));

            if(currentX - initialRotation.eulerAngles.x > currentMaxX){
                currentMaxX = (float) Math.Abs(currentX - initialRotation.eulerAngles.x);
            }
            if(currentY - initialRotation.eulerAngles.y > currentMaxY){
                currentMaxY = (float) Math.Abs(currentY - initialRotation.eulerAngles.y);
            }
            if(currentZ - initialRotation.eulerAngles.z > currentMaxZ){
                currentMaxZ = (float) Math.Abs(currentZ - initialRotation.eulerAngles.z);
            }
        }
        data.minXAngle = currentMaxX;
        data.minYAngle = currentMaxY;
        data.minZAngle = currentMaxZ;

        // calculate highest point during airtime
        float highestHeight = 0;
        foreach(Vector3 pos in positionsInAir)
        {
            float currentHeight = (float)Math.Abs(pos.y - initialPos.y);
            if(currentHeight > highestHeight){
                highestHeight = currentHeight;
            }
        }
        data.height = highestHeight;

        // assign first/last wheel time
        data.firstLastWheelDelta = firstLastWheelDelta;
        return data;
    }
}
