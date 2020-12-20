using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OllieFitnessFunction : FitnessFunction
{
    public float highOllieHeight = 5.0f;
    // ollie fitness function, what a doozy
    public override float GetFitness(BoardData data){
        // height = 0.25 weight - maximize
        // firstLastWheelDelta = 0.25 weight - minimize
        // minY = 0.25 weight - minimize
        // minZ = 0.25 weight - minimize
        // must have both feet on board to consider the trick completed
        // so return 0 if both feet aren't on board after trick is executed
        // TODO determine max height
        /*
        return (float)((data.height / highOllieHeight * 0.03125) + 
        ((1.0 / (data.firstLastWheelDelta + 1)) * 0.445) + 
        ((1.0 / (data.minYAngle + 1)) * 0.245) +
        ((1.0 / (data.minZAngle + 1)) * 0.245)) + 
        (data.feetOnBoard * 1000.45f0) +
        ((1.0f / (data.leftFootVariance+1))) +
        ((1.0f / (data.rightFootVariance+1)));
        */
        return (
            
            data.height * 0.3f - 3.0f + // maximize height of ollie
            (-data.boardPositionVariance*2 + 1) * 0.15f + // minimize non-vertical distance of board and punish when excessively far away
            (-data.firstLastWheelDelta + 1) * 0.05f + // minimize time between first/last wheel on ground and punish when excessively large period of time
            (-data.leftFootVariance + 1) * 0.15f + // minimize distance left foot travels and punish when excessively far away
            (-data.rightFootVariance + 1) * 0.15f + // minimize distance right foot travels and punish when excessively far away
            (data.feetOnBoard == 2 ? 5.0f : data.feetOnBoard == 1 ? 0.0f : -2.0f) // give appropriate weight depending on number of feet on board 
        );
    }
}
