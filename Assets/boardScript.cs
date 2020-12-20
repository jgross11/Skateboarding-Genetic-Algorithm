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
    private Transform leftFoot, rightFoot;
    private List<Vector3> leftFootPositions, rightFootPositions;
    public Specimen specimen;
    public int numFeetOnBoard;
    public const float FOOT_BOARD_HEIGHT_CONSTANT = 0.3672593f;
    

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialRotation = transform.rotation;
        runCounter = 0;
        rgbd = GetComponent<Rigidbody>();
        rotationsInAir = new List<Vector3>();
        positionsInAir = new List<Vector3>();
        leftFootPositions = new List<Vector3>();
        rightFootPositions = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        leftFootPositions.Add(leftFoot.position);
        rightFootPositions.Add(rightFoot.position);
        /*
        if(runCounter < runTime){
            runCounter += Time.deltaTime;
        }
        else{
            reset();
            runCounter = 0;
        }*/

        // all wheels just hit ground
        if(!boardOnGround && wheelsOnGround == NUM_WHEELS){
            boardOnGround = true; 
            // Debug.Log("board " + GetInstanceID() + " in air for " + airtime + " seconds");
            // Debug.Log("time between first and last wheel hitting ground was " + firstLastWheelDelta + " seconds");
            
            /*
            foreach(Vector3 info in rotationsInAir){
                Debug.Log(info);
            }
            */
            
            /*
            foreach(Vector3 info in positionsInAir)
            {
                Debug.Log(info);
            }
            */
            airtime = 0;
            rotationsInAir = new List<Vector3>();
            positionsInAir = new List<Vector3>();

        }
        // at least one wheel is on ground
        else if(isAWheelOnGround){
            firstLastWheelDelta += Time.deltaTime;
        }
        // all wheels just left ground
        else if(boardOnGround && wheelsOnGround == 0){
            // Debug.Log("board " + GetInstanceID() + " in air");
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

    public void SetFoot(bool isLeftFoot, Transform transform){
        if(isLeftFoot) leftFoot = transform; else rightFoot = transform;
    }

    public void reset()
    {
        boardData = GenerateBoardData();
        // Debug.Log(boardData.ToString());
        specimen.CalculateFitness(boardData);

        // disable physics to wipe velocity, torque, etc.
        rgbd.isKinematic = true;

        // reset position to start
        transform.position = initialPos;

        // reset rotation to start
        transform.rotation = initialRotation;

        // enable physics for next run
        rgbd.isKinematic = false;

        rotationsInAir = new List<Vector3>();
        positionsInAir = new List<Vector3>();
        leftFootPositions = new List<Vector3>();
        rightFootPositions = new List<Vector3>();

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
            float currentX = rotData.x >= 0 ? rotData.x : rotData.x + 360;
            float currentY = rotData.y >= 0 ? rotData.y : rotData.y + 360;
            float currentZ = rotData.z >= 0 ? rotData.z : rotData.z + 360;

            if(currentX - initialRotation.eulerAngles.x > currentMaxX){
                currentMaxX = currentX - initialRotation.eulerAngles.x;
            }
            if(currentY - initialRotation.eulerAngles.y > currentMaxY){
                currentMaxY = currentY - initialRotation.eulerAngles.y;
            }
            if(currentZ - initialRotation.eulerAngles.z > currentMaxZ){
                currentMaxZ = currentZ - initialRotation.eulerAngles.z;
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

        // calculate board position variance
        if(positionsInAir.Count > 0){
            Vector3 lastBoardPos = positionsInAir[positionsInAir.Count-1];
            Vector3 firstBoardPos = positionsInAir[0];
            data.boardPositionVariance = (float) Math.Sqrt(
                (lastBoardPos.x - firstBoardPos.x) * (lastBoardPos.x - firstBoardPos.x) +
                (lastBoardPos.z - firstBoardPos.z) * (lastBoardPos.z - firstBoardPos.z)
            );
        } else{
            data.boardPositionVariance = 1.0f;
        }

        // assign first/last wheel time
        data.firstLastWheelDelta = firstLastWheelDelta;

        // assign num feet on board after trick end
        data.feetOnBoard = numFeetOnBoard;

        // assign left / right foot variance from beginning to end of trick
        Vector3 leftStart, leftEnd, rightStart, rightEnd;
        leftStart = leftFootPositions[0];
        leftEnd = leftFootPositions[leftFootPositions.Count-1];
        rightStart = rightFootPositions[0];
        rightEnd = rightFootPositions[rightFootPositions.Count-1];
        data.leftFootVariance = (float) Math.Sqrt(
            (leftEnd.x - leftStart.x)*(leftEnd.x - leftStart.x)+
            (leftEnd.y - leftStart.y)*(leftEnd.y - leftStart.y)+
            (leftEnd.z - leftStart.z)*(leftEnd.z - leftStart.z)
        );
        data.rightFootVariance = (float) Math.Sqrt(
            (rightEnd.x - rightStart.x)*(rightEnd.x - rightStart.x)+
            (rightEnd.y - rightStart.y)*(rightEnd.y - rightStart.y)+
            (rightEnd.z - rightStart.z)*(rightEnd.z - rightStart.z)
        );

        return data;
    }

    void OnCollisionEnter(Collision collision){
        if(collision.collider.tag == "foot" && collision.collider.transform.position.y > transform.position.y + FOOT_BOARD_HEIGHT_CONSTANT){
            numFeetOnBoard++;
        }
    }

    void OnCollisionExit(Collision collision){
        if(collision.collider.tag == "foot" && numFeetOnBoard > 0){
            numFeetOnBoard--;
        }
    }
}
