using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundScript : MonoBehaviour
{

    // map storing all created boards' ID as key and script as value
    private Dictionary<int, boardScript> boardMap;

    // board GameObject to populate field
    public GameObject board;

    // number of generations to run experiment
    public int numberOfGenerations;

    // number of specimen per generation
    public int numberOfSpecimens;

    // mutation percentage
    public float mutationPercentage;

    // Start is called before the first frame update
    void Start()
    {
        boardMap = new Dictionary<int, boardScript>();
        RunExperiment();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider col = collision.collider;
        if(col.tag == "wheel"){

            // get board instance ID
            int boardID = col.transform.parent.GetInstanceID();

            // if no reference to this board exists in map, create it 
            if(!boardMap.ContainsKey(boardID)){
                boardScript bs = col.gameObject.GetComponentInParent(typeof(boardScript)) as boardScript;
                boardMap.Add(boardID, bs);
                Debug.Log("new board in map");
            }

            // increment number of wheels on ground for this board
            boardMap[boardID].UpdateWheelCount(1);
        }
        
    }

    void OnCollisionExit(Collision collision)
    {
        Collider col = collision.collider;
        if(col.tag == "wheel"){

            // get board instance ID
            int boardID = col.transform.parent.GetInstanceID();

            // if no reference to this board exists in map, create it 
            if(!boardMap.ContainsKey(boardID)){
                boardScript bs = col.gameObject.GetComponentInParent(typeof(boardScript)) as boardScript;
                boardMap.Add(boardID, bs);
                Debug.Log("new board in map");
            }

            // decrement number of wheels on ground for this board
            boardMap[boardID].UpdateWheelCount(-1);
        }
    }


    // initializes the experiment
    void RunExperiment(){
        
    }
}
