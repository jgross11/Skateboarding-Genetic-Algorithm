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

    // number of feet in contact with board after trick ends
    public int feetOnBoard;

    // left foot variance over duration of trick
    public float leftFootVariance;

    // right foot variance over duration of trick
    public float rightFootVariance;

    // difference between board end pos and board start pos
    public float boardPositionVariance;

    // TODO max distance between two feet and board


    public override string ToString(){
        string result = "";
        result += "max height: " + height;
        result += "\nfirst / last wheel time difference: " + firstLastWheelDelta;
        result += "\nmax X angle: " + minXAngle;
        result += "\nmax Y angle: " + minYAngle;
        result += "\nmax Z angle: " + minZAngle;
        result += "\nfeet on board: " + feetOnBoard;
        result += "\nleft foot variance: " + leftFootVariance;
        result += "\nright foot variance: " + rightFootVariance;
        return result;
    }
}
