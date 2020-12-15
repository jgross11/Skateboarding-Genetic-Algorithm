using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// helps avoid ambiguity / verbosity between System.Random and UnityEngine.Random
using r = UnityEngine.Random;

public class GARunner : MonoBehaviour
{

    // number of generations to run experiment
    public int numberOfGenerations;

    // number of specimen per generation
    public int numberOfSpecimens;

    // top x percent of current population specimen to use when breeding new generation
    public float truncationPercentage;

    // x percent chance that a new 
    public float mutationPercentage;

    // type of specimen to run in this experiment
    public GameObject specimenType;

    // number of actions that a specimen executes
    public int numberOfActions;

    // fittest specimen in population
    private GameObject fittestSpecimen;

    // max time an action can occur
    public float maxTime;

    // time multiplier used to speed up simulations
    public float timeMultiplier;

    // length of time each trick attempt can take
    public float durationPerAttempt;

    // time between 0 -> durationPerAttempt of the current attempt
    public float durationOfCurrentAttempt = 0;

    // minimum component value for a component vector in movement
    public float minMovementComponentValue;

    // maximum component value for a component vector in movement
    public float maxMovementComponentValue;

    // population array
    public GameObject[] population;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(200 * numberOfSpecimens, 1, 200);
        Time.timeScale *= timeMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        durationOfCurrentAttempt += Time.deltaTime;
        // if it is time to end current attempt
        if(durationOfCurrentAttempt > durationPerAttempt){
            SortPopulationByFitness();
            GenerateNextGeneration();
            durationOfCurrentAttempt = 0;
        }

    }

    // initializes the experiment
    public void RunExperiment(){
        
        // create initial population
        population = new GameObject[numberOfSpecimens];
        int xOffset = 0;
        for(int popIndex = 0; popIndex < numberOfSpecimens; popIndex++){
            // create gameobjects that will contain and perform movements
            GameObject go = Instantiate(specimenType, new Vector3(xOffset++ * 100, -3, 0), new Quaternion(0, 0, 0, 1));
            
            go.name = "Generation 1 Board " + (popIndex + 1);

            // generate random movement tuple and assign to game object's specimen
            Tuple<float, Vector3, bool>[] actions = new Tuple<float, Vector3, bool>[numberOfActions];

            float currentTimeMin = 0;
            for(int i = 0; i < numberOfActions; i++)
            {
                float newTimeMin = r.Range(currentTimeMin, maxTime);
                actions[i] = CreateNewTrait(newTimeMin);
                currentTimeMin = newTimeMin;
            }
            go.GetComponent<Specimen>().setActionTuple(actions);
            population[popIndex] = go;
        }
        
    }

    private Tuple<float, Vector3, bool> CreateNewTrait(float minTime){
        return Tuple.Create(r.Range(minTime, maxTime),                             // time to execute action at
        new Vector3(r.Range(minMovementComponentValue, maxMovementComponentValue),  // x component direction
                    r.Range(minMovementComponentValue, maxMovementComponentValue),  // y component direction
                    r.Range(minMovementComponentValue, maxMovementComponentValue)), // z component direction
                    (int) Math.Round(r.value) == 0);                                // foot that executes action
    }

    public void SortPopulationByFitness(){
        System.Array.Sort(population, compareByFitness);
        Debug.Log("current fittest after sorting: " + population[0].GetComponent<Specimen>().GetFitness());
        Debug.Log("fittest name: " + population[0].name);
    }

    public int compareByFitness(GameObject a, GameObject b){
        return b.GetComponent<Specimen>().GetFitness().CompareTo(a.GetComponent<Specimen>().GetFitness());
    }

    // calculate the fitness of each specimen
    void calculateFitnesses(){
        foreach (GameObject spec in population){
            // TODO fix this
            // spec.CalculateFitness();
        }
    }

    // determine the fittest specimen in the current population
    Specimen calculateFittestSpecimenInPopulation(){
        if(population.Length < 1){
            throw new System.ArgumentException("Invalid population size");
        }
        Specimen fittest = population[0].GetComponent<Specimen>();
        double fittestFitness = fittest.GetFitness();
        for(int i = 1; i < population.Length; i++){
            Specimen current = population[i].GetComponent<Specimen>();
            double currentFitness = current.GetFitness();
            if(currentFitness > fittestFitness){
                fittest = current;
                fittestFitness = currentFitness;
            }
        }
        return fittest;
    }

    // TODO ensure next generation's specimen aren't overwriting current generation while breeding
    void GenerateNextGeneration(){
        for(int i = 0; i < (int)(truncationPercentage * numberOfSpecimens); i++){
            population[i].GetComponent<Specimen>().Reset();
        }
        for(int i = (int)(truncationPercentage * numberOfSpecimens); i < numberOfSpecimens; i++){
            // Debug.Log(i + "th specimen before: " + population[i].GetComponent<Specimen>().GetActionTuple()[0].Item1);
            Specimen newPop = population[i].GetComponent<Specimen>();
            newPop.setActionTuple(breed(population[(int) r.Range(0, (truncationPercentage * numberOfSpecimens))].GetComponent<Specimen>(), population[(int) r.Range(0, (truncationPercentage * numberOfSpecimens))].GetComponent<Specimen>()));
            newPop.Reset();
            // Debug.Log(i + "th specimen after: " + population[i].GetComponent<Specimen>().GetActionTuple()[0].Item1);
            // Debug.Log("Current fitnes: " + next[i].GetFitness());
        }

/*
        // TODO fix this 
        for(int i = (int)(truncationPercentage * numberOfSpecimens); i < numberOfSpecimens; i++){
            population[i].GetComponent<Specimen>().setActionTuple(next[i].GetActionTuple());
        }
  */      
    }

    // TODO make this truly breed
    private Tuple<float, Vector3, bool>[] breed(Specimen a, Specimen b){
        Tuple<float, Vector3, bool>[] aActions = a.GetActionTuple();
        Tuple<float, Vector3, bool>[] bActions = b.GetActionTuple();
        Tuple<float, Vector3, bool>[] result = new Tuple<float, Vector3, bool>[numberOfActions];
        for(int i = 0; i < numberOfActions; i++)
        {
            float mutationValue = r.value;
            result[i] = (mutationValue < mutationPercentage) ? CreateNewTrait(i == 0 ? 0 : result[i-1].Item1) : ((int) Math.Round(r.value) == 0) ? aActions[i] : bActions[i];
            // Debug.Log("a"+i+": " + aActions[i].Item1 + ", " + aActions[i].Item2 + ", " + aActions[i].Item3);
            // Debug.Log("b"+i+": " + bActions[i].Item1 + ", " + bActions[i].Item2 + ", " + bActions[i].Item3);
            // Debug.Log("result "+i+": " + result[i].Item1 + ", " + result[i].Item2 + ", " + result[i].Item3);
        }

        // sort actions by action times
        SortActionsByExecutionTime(result);
        return result;
    }
    public void SortActionsByExecutionTime(Tuple<float, Vector3, bool>[] spec){
        System.Array.Sort(spec, compareByExecutionTime);
    }

    public int compareByExecutionTime(Tuple<float, Vector3, bool> a, Tuple<float, Vector3, bool> b){
        return a.Item1.CompareTo(b.Item1);
    }
}
