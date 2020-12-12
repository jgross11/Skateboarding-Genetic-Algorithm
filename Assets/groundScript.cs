using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundScript : MonoBehaviour
{

    // map storing all created boards' ID as key and script as value
    private Dictionary<int, boardScript> boardMap;

    // board GameObject whose parameters will be set by the specimen in this experiment
    public GameObject board;

    // number of generations to run experiment
    public int numberOfGenerations;

    // number of specimen per generation
    public int numberOfSpecimens;

    // mutation percentage
    public float mutationPercentage;

    // type of specimen to run in this experiment
    public Specimen specimenType;

    // fittest specimen
    private Specimen fittestSpecimen;

    // population array
    private Specimen[] population;

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

        // collided with wheel
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
                Debug.Log("new board in map");
            }

            // decrement number of wheels on ground for this board
            boardMap[boardID].UpdateWheelCount(-1);
        }
    }


    // initializes the experiment
    void RunExperiment(){
        
        // create initial population
        population = new Specimen[numberOfSpecimens];
        foreach (var spec in population){
            
        }
        
    }

    // calculate the fitness of each specimen
    void calculateFitnesses(){
        foreach (Specimen spec in population){
            // TODO fix this
            // spec.CalculateFitness();
        }
    }

    // determine the fittest specimen in the current population
    Specimen calculateFittestSpecimenInPopulation(){
        if(population.Length < 1){
            throw new System.ArgumentException("Invalid population size");
        }
        Specimen fittest = population[0];
        double fittestFitness = fittest.GetFitness();
        for(int i = 1; i < population.Length; i++){
            Specimen current = population[i];
            double currentFitness = current.GetFitness();
            if(currentFitness > fittestFitness){
                fittest = current;
                fittestFitness = currentFitness;
            }
        }
        return fittest;
    }

    void nextGeneration(){
        Specimen[] next = new Specimen[numberOfSpecimens];
        foreach(Specimen spec in next){

        }
    }
}
