using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardData
{
    // highest point center of board got in run
    public float height;

    // TODO end and start position of both feet

    // time between first and last wheel hitting ground (not / sketchy)
    public float firstLastWheelDelta;

    // minimum angle of rotation experienced on X axis
    public float minXAngle;

    // minimum angle of rotation experienced on Y axis
    public float minYAngle;

    // minimum angle of rotation experienced on Z axis
    public float minZAngle;

    // TODO max distance between two feet and board

    public override string ToString(){
        string result = "";
        result += "max height: " + height;
        result += "\nfirst / last wheel time difference: " + firstLastWheelDelta;
        result += "\nmax X angle: " + minXAngle;
        result += "\nmax Y angle: " + minYAngle;
        result += "\nmax Z angle: " + minZAngle;
        return result;
    }
}
