using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 Description : Call AI to decide moveset, compact to AttackPackage and send to middleware, Sanitize/Stop Attack
 Status : Unfinished
 To Be Implemented : Call AI (Phase 2), Sanitization
 */

public class BossDecision : MonoBehaviour
{
    private BossMiddleware BM;
    private AIDecision AD;
    private readonly int[] combo = { 0,0,0,0,0,0,0,0,0,0,0,0,0};
    private AttackPackage previousMove;
    private int atkCount = 0;

    private (float, float)[][] rules;

    [Header("Stat")]
    [SerializeField] private float[] recovEnd = { 1.5f, 1.5f, 1, 1f, 1, 2, 1, 1, 1.5f, 2, 0.4f, 2.5f, 1};
    

    //IF BOSS has 1 timing for each moveset per round -> store here
    //if boss is not real ignore it
    private void Start()
    {
        BM = GetComponent<BossMiddleware>();
        AD = GetComponent<AIDecision>();
        rules = BossMovesetRule.rules;
        previousMove = new AttackPackage(0);
    }

    #region RequestAI

    public void AttackNOW()
    {
        //Send AI
        Vector3 playerPos = Environment.GetPlayer().transform.position;
        Vector3 playerVelos = Environment.GetPlayer().GetComponent<Rigidbody2D>().velocity;
        string note = $"{playerPos.x},{playerPos.y},{transform.position.x},{transform.position.y},{playerVelos.x},{playerVelos.y},{atkCount},-1";
        AD.SendAI(note,atkCount);
        //SendAttack();
    }
    public void AttackNOW(int moveset)
    {
        if ((moveset == 6 || moveset == 7) && combo[moveset] == 1) PackAndSend($"Action,{AD.bossNo},{moveset},0,0,0,0");
        else
        {
            //Send AI
            Vector3 playerPos = Environment.GetPlayer().transform.position;
            Vector3 playerVelos = Environment.GetPlayer().GetComponent<Rigidbody2D>().velocity;
            string note = $"{playerPos.x},{playerPos.y},{transform.position.x},{transform.position.y},{playerVelos.x},{playerVelos.y},{atkCount},{moveset}";
            AD.SendAI(note, atkCount);
            //SendAttack();
        }

    }
    public void SendReward()
    {
        AD.SendReward(previousMove.moveset,atkCount);
    }
    #endregion


    #region SanitizeMoveSet
    private AttackPackage SanitizeMove(AttackPackage ap)
    {
        //AI say fin
        if (ap.moveset == 0)
        {
            return CanEndCombo(ap);
        }

        //combo > 4
        if (atkCount >= 4 && combo[ap.moveset] == 0)
        {
            return CanEndCombo(ap);
        }        

        //Percent to Time
        ap.telegraphTime = ap.telegraphTime * (rules[ap.moveset][0].Item2 - rules[ap.moveset][0].Item1) + rules[ap.moveset][0].Item1;
        ap.attackTime    = rules[ap.moveset][1].Item1;
        ap.recoverTime   = ap.recoverTime   * (rules[ap.moveset][2].Item2 - rules[ap.moveset][2].Item1) + rules[ap.moveset][2].Item1;
        ap.rotation      = ap.rotation      * (rules[ap.moveset][3].Item2 - rules[ap.moveset][3].Item1) + rules[ap.moveset][3].Item1;
        if (new List<int> { 6, 12 }.Contains(ap.moveset)){
            for(int i=0;i<4;i++)
            {
                ap.pos[i].x = ap.pos[i].x * (rules[ap.moveset][4].Item2 - rules[ap.moveset][4].Item1) + rules[ap.moveset][4].Item1;
                ap.pos[i].y = ap.pos[i].y * (rules[ap.moveset][5].Item2 - rules[ap.moveset][5].Item1) + rules[ap.moveset][5].Item1;
            }
        }
        //Left of Right
        if (new List<int> { 1,4, 10, 11 }.Contains(ap.moveset)) ap.rotation = ap.rotation > 90 ? 180 : 0;

        //Rotate downward
        if (new List<int> { 3, 5,8 }.Contains(ap.moveset) && transform.position.y > Environment.GetPlayer().transform.position.y)
        {
            ap.rotation *= -1;
        }

        if (new List<int> { 1,3,5 }.Contains(ap.moveset) && previousMove!=null  && previousMove.moveset!=ap.moveset) combo[ap.moveset] = 0;
        if (combo[ap.moveset] == 0) atkCount++;
        
        previousMove = ap;
        if(!new List<int> { 4,8,10,11,12 }.Contains(ap.moveset)) ap.atkCombo = ++combo[ap.moveset];
        if (combo[ap.moveset] >= 2) combo[ap.moveset] = 0;
        ap.recoverEnd = recovEnd[ap.moveset];
        return ap;
    }
    #endregion


    #region PackAndSend
    public void PackAndSend(string action)
    {

        //Compact To Package
        string[] actArr = action.Split(',');
        AttackPackage ap;
        try
        {
            ap = new(int.Parse(actArr[2]), float.Parse(actArr[3]) / 10, (10f-(float)Environment.GetLevel()) / 10, float.Parse(actArr[4]) / 10);

            //Pseudo Vecto3 for trap
            if (int.Parse(actArr[2]) == 6 || int.Parse(actArr[2]) == 12)
            {
                ap.pos[0] = new Vector3(float.Parse(actArr[3]) / 10, float.Parse(actArr[4]) / 10);
                ap.pos[1] = new Vector3(float.Parse(actArr[5]) / 20, float.Parse(actArr[6]) / 20);
                ap.pos[2] = new Vector3(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
                ap.pos[3] = new Vector3(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
            }
        }
        catch
        {
            ap = new(0);
        }
        
        ap = SanitizeMove(ap);

        if (ap != null)
        {
            AD.SetPreqMS(ap.moveset);
            BM.PackageCome(ap);
        }
    }

    #endregion


    #region Misc
    //Check If the combo can be end => 0 = can end, other = attack must be cast to end
    public AttackPackage CanEndCombo(AttackPackage ap)
    {
        if (combo[6] == 1 || combo[7] == 1)
        {
            ForceTrap();
            return null;
        }
        else
        {
            
            EndCombo();
            ap.moveset = 0;
            return ap;
        }
    }
    public void EndCombo()
    {
        for (int i = 1; i <= 12; i++)
        {
            combo[i] = 0;
        }
        atkCount = 0;
        previousMove = new AttackPackage(0);
    }

    private void ForceTrap()
    {
        
        for(int i = 6; i <= 7; i++)
        {
            if(combo[i] == 1) {
                PackAndSend($"Action,1,{i},{UnityEngine.Random.Range(0.4f, 1f)},{UnityEngine.Random.Range(0.4f, 1f)},{UnityEngine.Random.Range(0.4f, 1f)},0,0,0,0"); //Fixed Timing?
                return;
            }
        }
    }

    public void ResetAI(bool isMs,bool isPr,bool isTp)
    {
        string message = $"Reset,{AD.bossNo},{isMs},{isPr},{isTp}";
        AD.SendCommmand(message);
    }
    #endregion


    #region Debug
    //Just for debug
    public void SendAttack()
    {
        AttackPackage ap;
        int x = UnityEngine.Random.Range(0, 13);
        ap = new(x, UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
        if (x == 6 || x == 12)
        {
            for (int i = 0; i < 4; i++)
            {
                ap.pos[i] = new Vector3(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));
            }
        }
        ap = SanitizeMove(ap);
        //if (ap != null) Debug.Log(ap.moveset);
        if (ap != null) BM.PackageCome(ap);
    }
    #endregion



}
