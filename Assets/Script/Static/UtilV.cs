using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This contain util var

public static class UtilV
{
    public readonly static float BORDERLEFT = -8.5f;
    public readonly static float BORDERRIGHT = 8.5f;
    public readonly static float BORDERTOP = 5f;
    public readonly static float BORDERBOTTOM = -5f;
    public readonly static float BORDERFLOOR = -4.2f;
    public readonly static float bossHeight = 3;
    public readonly static float bossWidth = 2;
    public static void PreventOotOfBound(Transform t,Vector3 scale)
    {
        float x =t.position.x,y=t.position.y;
        if(t.position.x < BORDERLEFT + scale.x/2)
        {
            x = BORDERLEFT + scale.x / 2;
        }
        if(t.position.x > BORDERRIGHT - scale.x / 2)
        {
            x = BORDERRIGHT - scale.x / 2;
        }
        if (t.position.y < BORDERFLOOR + scale.y / 2)
        {
            y = BORDERFLOOR + scale.y / 2;
        }
        if (t.position.y > BORDERTOP - scale.y / 2)
        {
            y = BORDERTOP - scale.y / 2;
        }
        t.position = new Vector3(x, y);
    }

}
