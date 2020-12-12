using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OllieFitnessFunction : FitnessFunction
{
    // ollie fitness function, what a doozy
    public override float GetFitness(BoardData data){
        // height = 0.25 weight
        // firstLastWheelDelta = 0.25 weight
        // minY = 0.25 weight
        // minZ = 0.25 weight
        // TODO determine max height
        return (float)(data.height / 1.0 * 0.25 + 
        (1.0 / (data.firstLastWheelDelta + 1)) * 0.25 + 
        (1.0 / (data.minYAngle + 1)) * 0.25 +
        (1.0 / (data.minZAngle + 1)) * 0.25);
    }
}
