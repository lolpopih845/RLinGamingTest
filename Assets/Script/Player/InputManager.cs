using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Buffer Input, Validate Input, Store Cursor Pos?, Input Cooldown Management
 Status : Unfinsihed (Phase 3)
 Future Implementation : Do it (Phase 3) + maxCd on attack change depend on atkSp buff (1 placeholder) + maxCd on special depend on which special (5 placeholder)
 */

//Action -> 0=Jump 1=Dash 2=Attack 3=Special
public class InputManager : MonoBehaviour
{
    private PlayerMovement PM;

    //[Action][ActPreq] 
    private float[][] maxCd;
    private float[] actionCd;
    private delegate void action();
    private action[] actions = new action[] { };
    private List<int> HoldButtonBuffer = new(); //Jump,Attack,Dash,Special
    private List<(int, float)> PressButtonBuffer = new(); //Jump,Attack,Dash,Special

    void Start()
    {
        PM = Environment.GetPlayer().GetComponent<PlayerMovement>();
        maxCd = new float[][]{ new float[] { 0.1f,0.3f,0.3f,5f}, new float[] { 0.1f,0.75f,0.3f,5f}, new float[] { 0.1f,0.3f,1f,5f}, new float[] { 0.1f,0.5f,1f,5f}};
        actionCd = new float[] { 0, 0, 0, 0 };
        actions = new action[] { PM.Jump,PM.Dash,PM.Attack,PM.Special};
    }

    // Update is called once per frame
    void Update()
    {
        DepleteCd();
        RecieveInput();
        ValidateAndSend();
    }

    #region BufferInput
    private void RecieveInput()
    {
        if (Input.GetButton("Jump")) HoldButtonBuffer.Add(0);
        if (Input.GetButton("Attack")) HoldButtonBuffer.Add(2);
        if (Input.GetButtonDown("Jump")) PressButtonBuffer.Add((0, 0.25f));
        if (Input.GetButtonDown("Attack")) PressButtonBuffer.Add((2, 0.25f));
        if (Input.GetButtonDown("Dodge")) PressButtonBuffer.Add((1, 0.25f));
        if (Input.GetButtonDown("Special")) PressButtonBuffer.Add((3, 0.25f));
    }
    #endregion

    #region ValidateInput
    private void ValidateAndSend()
    {
        if (PM.disableAction > 0) return;
        foreach (int act in HoldButtonBuffer)
        {
            if (act == 0) PM.HoldJump();
            else PM.HoldAttack();
        }
        HoldButtonBuffer.Clear();
        List<(int, float)> temp = new();
        foreach((int, float) act in PressButtonBuffer) {
            (int, float) tempy = act;
            if (actionCd[tempy.Item1] <= 0)
            {
                actions[tempy.Item1]();
                ApplyCd(tempy.Item1);
                temp.Clear();
                continue;
            }
            tempy.Item2 -= Time.deltaTime;
            if (tempy.Item2 > 0) temp.Add(tempy);
        }
        PressButtonBuffer = temp;
    }
    #endregion


    #region Input Cooldown
    private void ApplyCd(int preqact)
    {
        for (int act = 0; act < 4; act++)
        {
            actionCd[act] = Mathf.Max(actionCd[act], maxCd[act][preqact]);
            if (act == 0) actionCd[1] = 0;
        }
        
    }
    private void DepleteCd()
    {
        for (int i = 0; i < 4; i++)
        {
            if(actionCd[i] > 0) actionCd[i] -= Time.deltaTime;
        }
    }
    #endregion

}
