using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class BossMovesetRule
{
    public static (float, float)[][] rules = new (float, float)[][] //Usage -> rules[moveset][variable].Item1/2
    {
      //new (float, float)[] {   (Tele)    ,    (Atk)     ,    (Recov)   ,     (Rot)     , (Posx) , (PosY) }
        new (float, float)[] {  },
        
        new (float, float)[] { (0.4f, 2f)  , (1.0f, 1.0f) ,  (0.2f, 1.00f) ,  (0f, 180f) }, //1
        new (float, float)[] { (0.4f, 1f)  , (0.6f, 0.6f) ,  (0.2f, 1.00f) ,  (0f, 180f) }, //2
        new (float, float)[] { (0.4f, 1.5f), (0.6f, 0.6f) ,  (0.2f, 1.00f) ,  (0f, 180f) }, //3
        new (float, float)[] { (0.6f, 1.5f), (0.7f, 0.7f) ,  (0.2f, 1.0f)  ,  (0f, 180f) }, //4
        new (float, float)[] { (0.4f, 1f)  , (0.5f, 0.5f) ,  (0.2f, 1.0f)  ,  (0f, 180f) }, //5
        new (float, float)[] { (0.4f, 1f)  , (0.6f, 0.6f) ,  (0f, 1.0f)    ,  (0f, 0f)    , (UtilV.BORDERLEFT+1,UtilV.BORDERRIGHT-1) , (UtilV.BORDERBOTTOM+1,UtilV.BORDERTOP-1)}, //6
        new (float, float)[] { (0.4f, 1f)  , (0.6f, 0.6f) ,  (0f, 1.0f)    ,  (0f, 0f)   }, //7
        new (float, float)[] { (0.4f, 1.5f), (1f, 1f)     ,  (0.2f, 1.00f) ,  (0f, 180f) }, //8
        new (float, float)[] { (0.4f, 1f)  , (2f, 2f)     ,  (0.2f, 1f)    ,  (30f, 150f)}, //9
        new (float, float)[] { (0.25f,0.6f), (0.4f, 0.4f) ,  (0.1f, 0.5f)  ,  (0f, 180f) }, //10
        new (float, float)[] { (1f, 1.5f)  , (1.2f, 1.2f) ,  (1.50f, 1.50f),  (0f, 180f) }, //11
        new (float, float)[] { (0.4f, 1f)  , (0.6f, 0.6f) ,  (0.40f, 0.40f),  (0f, 0f)    , (UtilV.BORDERLEFT+1,UtilV.BORDERRIGHT-1) , (UtilV.BORDERBOTTOM+1,UtilV.BORDERTOP-1) }, //12

    };
    //3,5,8 check player Y 
}
