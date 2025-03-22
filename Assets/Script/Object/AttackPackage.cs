using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 AttackPackage : Like struct, contain info about boss moveset
 Status : Done
 Future Implementation : reward field
 */

public class AttackPackage
{
    
    public int moveset;
    public float telegraphTime, attackTime, recoverTime, recoverEnd;
    public float rotation;
    public Vector3[] pos = new Vector3[4];
    public int atkCombo;
    public AttackPackage(int moveset)
    {
        this.moveset = moveset;
        telegraphTime = 0.4f;
        attackTime = 1;
        recoverTime = 0.4f;

        atkCombo = 0;
    }
    public AttackPackage(int moveset,float telegraphTime,float recoverTime)
    {
        this.moveset = moveset;
        this.telegraphTime = telegraphTime;
        this.attackTime = 1;
        this.recoverTime = recoverTime;
    }
    public AttackPackage(int moveset, float telegraphTime, float recoverTime, Vector3[] pos)
    {
        this.moveset = moveset;
        this.telegraphTime = telegraphTime;
        this.recoverTime = recoverTime;
        this.pos = pos;
    }
    public AttackPackage(int moveset, float telegraphTime, float recoverTime,float rotation)
    {
        this.moveset = moveset;
        this.telegraphTime = telegraphTime;
        this.recoverTime = recoverTime;
        this.rotation = rotation;
    }
    public void PushPos(Vector3 pos)
    {
        for (int i = 0; i < 4; i++)
        {
            if (this.pos[i] == null)
            {
                this.pos[i] = pos;
                return;
            }
        }
    }
}
