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

    // mutation percentage
    public float mutationPercentage;

    // type of specimen to run in this experiment
    public GameObject specimenType;

    // number of actions that a specimen executes
    public int numberOfActions;

    // fittest specimen in population
    private GameObject fittestSpecimen;

    // max time an action can occur
    public float maxTime;

    // length of time each trick attempt can take
    public float durationPerAttempt;

    // time between 0 -> durationPerAttempt of the current attempt
    public float durationOfCurrentAttempt = 0;

    // minimum component value for a component vector in movement
    public float minMovementComponentValue;

    // maximum component value for a component vector in movement
    public float maxMovementComponentValue;

    // population array
    private GameObject[] population;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(20 * numberOfSpecimens, 1, 100);
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
            GameObject go = Instantiate(specimenType, new Vector3(xOffset++ * 10, -3, 0), new Quaternion(0, 0, 0, 1));
            
            // generate random movement tuple and assign to game object's specimen
            Tuple<float, Vector3, bool>[] actions = new Tuple<float, Vector3, bool>[numberOfActions];

            float currentTimeMin = 0;
            for(int i = 0; i < numberOfActions; i++)
            {
                float newTimeMin = r.Range(currentTimeMin, maxTime);
                actions[i] = Tuple.Create(newTimeMin,                                       // time to execute action at
                new Vector3(r.Range(minMovementComponentValue, maxMovementComponentValue),  // x component direction
                            r.Range(minMovementComponentValue, maxMovementComponentValue),  // y component direction
                            r.Range(minMovementComponentValue, maxMovementComponentValue)), // z component direction
                            (int) Math.Round(r.value) == 0);                                // foot that executes action
                currentTimeMin = newTimeMin;
            }
            go.GetComponent<Specimen>().setActionTuple(actions);
            population[popIndex] = go;
        }
        
    }

    public void SortPopulationByFitness(){
        
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

    void GenerateNextGeneration(){
        Specimen[] next = new Specimen[numberOfSpecimens];
        foreach(Specimen spec in next){

        }
    }

    Specimen breed(Specimen a, Specimen B){
        
        return null;
    }
}
