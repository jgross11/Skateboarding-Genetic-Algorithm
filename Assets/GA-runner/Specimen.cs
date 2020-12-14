﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specimen : MonoBehaviour
{

    // constant bool for left, right foot
    private const bool LEFT = true;
    private const bool RIGHT = false;

    // the fitness function to use when evaluating the specimen's performance
    public FitnessFunction fitnessFunction;

    // the fitness value of this board
    public float fitnessValue;

    // reference to boardScript for obtaining information
    public boardScript boardScript;

    // reference to back foot's rigidbody
    public Rigidbody backFootRGBD;

    // initial back foot position
    public Vector3 backFootInitialPos;

    // reference to front foot's rigidbody
    public Rigidbody frontFootRGBD;

    // initial front foot position
    public Vector3 frontFootInitialPos;

    // the identifier for this specimen in its generation
    private int specimenID;

    // array of foot-movement vectors, timestamps,
    // and the foot to execute movement on
    private Tuple<float, Vector3, bool>[] actionsPairing;

    // time variable that determines when a foot movement occurs
    // start at -1 to give time for board to settle on ground before
    // starting a trick attempt
    private float simulationTime = -1;

    // index of next foot movement to execute
    private int nextActionIndex = 0;

    // time to execute next action
    private float nextActionTime;

    // movement to execute at next time
    private Vector3 nextActionVector;

    // raw power behind movement
    private float thrust = 50000;

    void Start(){

        // obtain appropriate references for prefab children
        foreach (Transform child in gameObject.transform)
        {
            string childName = child.name;
            // boardScript contains information on trick attempt
            if(childName == "board"){
                boardScript = child.GetComponent<boardScript>();
            
            // back foot rigidbody used to execute movements on back foot
            } else if(childName == "back_foot"){
                backFootRGBD = child.GetComponent<Rigidbody>();
                backFootInitialPos = child.transform.position;

            // front foot rigidbody used to execute movements on front foot
            } else if(childName == "front_foot"){
                frontFootRGBD = child.GetComponent<Rigidbody>();
                frontFootInitialPos = child.transform.position;

            // fitness function designates how to interpret information
            } else if(childName == "fitnessFunctionObject"){
                fitnessFunction = child.GetComponent<FitnessFunction>();
            } else{
                Debug.Log("Unknown child component " + child.name);
            }
        }
        


        // test ollie action pairs
        /*
        actionsPairing = new Tuple<float, Vector3, bool>[]{
            Tuple.Create(0.9f, new Vector3(0.0f, 1.0f, 0.0f), RIGHT),
            Tuple.Create(1f, new Vector3(0.0f, -2.0f, 0), LEFT),
            Tuple.Create(1.05f, new Vector3(0.0f, 2.0f, 0.3f), LEFT),
            Tuple.Create(1.05f, new Vector3(0.0f, 0.0f, 0.5f), RIGHT),
            Tuple.Create(1.4f, new Vector3(0.0f, -2.0f, 0), LEFT),
            Tuple.Create(1.4f, new Vector3(0.0f, -2.0f, -1.0f), RIGHT)
        };
        */
    }

    void Update(){
        simulationTime += Time.deltaTime;

        // if it is time to do the next action
        if(nextActionTime < simulationTime && actionsPairing != null && nextActionIndex < actionsPairing.Length){
            // Debug.Log("executing movement #" + nextActionIndex);
            // apply the next action's movement
            // to the appropriate foot
            // true = apply movement to back foot
            // false = apply movement to front foot
            // I feel the need to do a ternary, although it is not allowed, so this is the compromise
            if(actionsPairing[nextActionIndex].Item3) backFootRGBD.AddForce(nextActionVector * thrust); else frontFootRGBD.AddForce(nextActionVector * thrust);

            // move to would-be next index
            nextActionIndex++;

            // set next action time and movement if applicable
            if(nextActionIndex < actionsPairing.Length){
                nextActionTime = actionsPairing[nextActionIndex].Item1;
                nextActionVector = actionsPairing[nextActionIndex].Item2;
            }
        }
    }

    // calculates this specimen's fitness
    public float CalculateFitness(BoardData data){
        fitnessValue = fitnessFunction.GetFitness(data);
        return fitnessValue;
    }

    // returns the fitness of this specimen
    public double GetFitness(){
        return fitnessValue;
    }

    // sets this action tuple to the given
    public void setActionTuple(Tuple<float, Vector3, bool>[] actions){
        actionsPairing = actions;
        nextActionTime = actionsPairing[0].Item1;
        nextActionVector = actionsPairing[0].Item2;
    }

    public void Reset(){
        boardScript.reset();
        backFootRGBD.isKinematic = true;
        backFootRGBD.transform.position = backFootInitialPos;
        backFootRGBD.isKinematic = false;
        frontFootRGBD.isKinematic = true;
        frontFootRGBD.transform.position = frontFootInitialPos;
        frontFootRGBD.isKinematic = false;
        simulationTime = -1;
        nextActionIndex = 0;
        fitnessValue = 0;
    }

    public Tuple<float, Vector3, bool>[] GetActionTuple(){
        return actionsPairing;
    }
}
