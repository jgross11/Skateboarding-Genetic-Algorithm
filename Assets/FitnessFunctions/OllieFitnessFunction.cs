using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OllieFitnessFunction : FitnessFunction
{
    public float highOllieHeight = 1.0f;
    // ollie fitness function, what a doozy
    public override float GetFitness(BoardData data){
        // height = 0.25 weight - maximize
        // firstLastWheelDelta = 0.25 weight - minimize
        // minY = 0.25 weight - minimize
        // minZ = 0.25 weight - minimize
        // must have both feet on board to consider the trick completed
        // so return 0 if both feet aren't on board after trick is executed
        // TODO determine max height
        return data.bothFeetOnBoard ? (float)((data.height / highOllieHeight * 0.25) + 
        ((1.0 / (data.firstLastWheelDelta + 1)) * 0.25) + 
        ((1.0 / (data.minYAngle + 1)) * 0.25) +
        ((1.0 / (data.minZAngle + 1)) * 0.25))
        : 0;
    }
}
