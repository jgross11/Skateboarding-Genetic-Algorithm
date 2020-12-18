using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public Vector3 direction;
    public bool isLeftFoot;
    public float executionTime;

    public Action(float time, Vector3 dir, bool foot){
        direction = dir;
        isLeftFoot = foot;
        executionTime = time;
    }

    public override string ToString()
    {
        return "dir="+direction.ToString()+"|isLeftFoot="+isLeftFoot+"|time="+executionTime+"\n";
    }
}
