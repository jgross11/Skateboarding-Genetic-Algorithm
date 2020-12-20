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
    public GameObject[] populationGOArr;

    // population specimen information
    public Specimen[] population;



    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(200 * numberOfSpecimens, 1, 200);
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeMultiplier;
        durationOfCurrentAttempt += Time.deltaTime;
        // if it is time to end current attempt
        if(durationOfCurrentAttempt > durationPerAttempt){
            ResetPopulation();
            SortPopulationByFitness();
            GenerateNextGeneration();
            durationOfCurrentAttempt = 0;
        }

    }

    public void ResetPopulation(){
        foreach(Specimen spec in population)
        {
            spec.Reset();
        }
    }

    // initializes the experiment
    public void RunExperiment(){
        
        // create initial population
        populationGOArr = new GameObject[numberOfSpecimens];
        population = new Specimen[numberOfSpecimens];
        int xOffset = 0;
        for(int popIndex = 0; popIndex < numberOfSpecimens; popIndex++){
            // create gameobjects that will contain and perform movements
            GameObject go = Instantiate(specimenType, new Vector3(xOffset++ * 100, -3, 0), new Quaternion(0, 0, 0, 1));
            
            go.name = "Board " + (popIndex + 1);

            // generate random movement tuple and assign to game object's specimen
            Action[] actions = new Action[numberOfActions];

            float currentTimeMin = 0;
            for(int i = 0; i < numberOfActions; i++)
            {
                actions[i] = CreateNewTrait(currentTimeMin);
                currentTimeMin = actions[i].executionTime;
            }

            population[popIndex] = go.GetComponent<Specimen>();
            population[popIndex].SetActions(actions);
            population[popIndex].specimenID = popIndex+1;
            populationGOArr[popIndex] = go;
        }
        
    }

    private Action CreateNewTrait(float minTime){
        return new Action(r.Range(minTime, maxTime),                             // time to execute action at
        new Vector3(r.Range(minMovementComponentValue, maxMovementComponentValue),  // x component direction
                    r.Range(minMovementComponentValue, maxMovementComponentValue),  // y component direction
                    r.Range(minMovementComponentValue, maxMovementComponentValue)), // z component direction
                    (int) Math.Round(r.value) == 0);                                // foot that executes action
    }

    public void SortPopulationByFitness(){
        System.Array.Sort(population, compareByFitness);
        /*
            Debug.Log("current fittest after sorting: " + population[0].ToString());
            Debug.Log("least fittest after sorting: " + population[population.Length-1].ToString());
            Debug.Log("most-least fit diff: " + (population[0].GetFitness() - population[population.Length-1].GetFitness()));
        */
        float averageFitness = 0.0f;
        foreach(Specimen spec in population)
        {
            averageFitness += (float)spec.GetFitness();
        }
        averageFitness /= (float)population.Length;
        Debug.Log("Average fitness: " + averageFitness);
        for(int i = 0; i < numberOfSpecimens; i++)
        {
            populationGOArr[i].GetComponent<Specimen>().SetActions(population[i].GetActions());
            populationGOArr[i].GetComponent<Specimen>().specimenID = population[i].specimenID;
        }
        
        // debug sorting
        
        /*
        foreach(Specimen spec in population)
        {
            Debug.Log("Specimen name: " + spec.name + " | fitness: " + spec.GetFitness());
        }
        */

    }

    public int compareByFitness(Specimen a, Specimen b){
        return b.GetFitness().CompareTo(a.GetFitness());
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
        for(int i = (int)(truncationPercentage * numberOfSpecimens); i < numberOfSpecimens; i++){
            // Debug.Log(i + "th specimen before: " + population[i].GetComponent<Specimen>().GetActionTuple()[0].Item1);
            population[i].SetActions(breed(population[(int) r.Range(0, (truncationPercentage * numberOfSpecimens))], population[(int) r.Range(0, (truncationPercentage * numberOfSpecimens))]));
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
    private Action[] breed(Specimen a, Specimen b){
        Action[] aActions = a.GetActions();
        Action[] bActions = b.GetActions();
        Action[] result = new Action[numberOfActions];
        for(int i = 0; i < numberOfActions; i++)
        {
            float mutationValue = r.value;
            result[i] = (mutationValue < mutationPercentage) ? CreateNewTrait(i == 0 ? 0 : result[i-1].executionTime) : ((int) Math.Round(r.value) == 0) ? aActions[i] : bActions[i];
            // Debug.Log("a"+i+": " + aActions[i].Item1 + ", " + aActions[i].Item2 + ", " + aActions[i].Item3);
            // Debug.Log("b"+i+": " + bActions[i].Item1 + ", " + bActions[i].Item2 + ", " + bActions[i].Item3);
            // Debug.Log("result "+i+": " + result[i].Item1 + ", " + result[i].Item2 + ", " + result[i].Item3);
        }

        // sort actions by action times
        SortActionsByExecutionTime(result);
        return result;
    }
    public void SortActionsByExecutionTime(Action[] spec){
        System.Array.Sort(spec, compareByExecutionTime);
    }

    public int compareByExecutionTime(Action a, Action b){
        return a.executionTime.CompareTo(b.executionTime);
    }
}
