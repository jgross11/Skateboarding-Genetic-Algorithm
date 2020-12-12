using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FitnessFunction : MonoBehaviour
{

    // indicates how to calculate a fitness function's value
    public abstract float GetFitness(BoardData data);
}
