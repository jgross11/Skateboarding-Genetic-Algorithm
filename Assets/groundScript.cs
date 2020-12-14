using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundScript : MonoBehaviour
{

    // map storing all created boards' ID as key and script as value
    private Dictionary<int, boardScript> boardMap;

    // genetic algorithm controller
    public GARunner runner;

    // Start is called before the first frame update
    void Start()
    {
        boardMap = new Dictionary<int, boardScript>();
        runner.RunExperiment();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider col = collision.collider;

        // collided with wheel
        if(col.tag == "wheel"){

            // get board instance ID
            int boardID = col.transform.parent.GetInstanceID();

            // if no reference to this board exists in map, create it 
            if(!boardMap.ContainsKey(boardID)){
                boardScript bs = col.gameObject.GetComponentInParent(typeof(boardScript)) as boardScript;
                boardMap.Add(boardID, bs);
                // Debug.Log("new board in map");
            }

            // increment number of wheels on ground for this board
            boardMap[boardID].UpdateWheelCount(1);
        }
        
    }

    void OnCollisionExit(Collision collision)
    {
        Collider col = collision.collider;

        // wheel left ground
        if(col.tag == "wheel"){

            // get board instance ID
            int boardID = col.transform.parent.GetInstanceID();

            // if no reference to this board exists in map, create it 
            if(!boardMap.ContainsKey(boardID)){

                // get board's boardscript reference
                boardScript bs = col.gameObject.GetComponentInParent(typeof(boardScript)) as boardScript;
                
                // add board and script to map
                boardMap.Add(boardID, bs);
                // Debug.Log("new board in map");
            }

            // decrement number of wheels on ground for this board
            boardMap[boardID].UpdateWheelCount(-1);
        }
    }
}
