using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Specimen : MonoBehaviour
{
    // the fitness function to use when evaluating the specimen's performance
    private FitnessFunction fitnessFunction;
    
    // the board this specimen acts through (setting up foot positioning, pressures, etc.)
    private GameObject board;

    // the fitness value of this board
    private double fitnessValue;

    // the identifier for this specimen in its generation
    private int specimenID;

    // initializes the appropriate parameters of this specimen's board
    public abstract void initModel();

    // calculates this specimen's fitness
    public double calculateFitness(){
        fitnessValue = fitnessFunction.getFitness();
        return fitnessValue;
    }

    // returns the fitness of this specimen
    public double getFitness(){
        return fitnessValue;
    }
}
