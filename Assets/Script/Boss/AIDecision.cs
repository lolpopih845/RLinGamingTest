using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
 Description : Handle AI API
 Status : Unfinsihed
 Future Implementation : Do it (Phase 2)
 */

public class AIDecision : MonoBehaviour
{
    private BossDecision BD;
    public int bossNo;
    private enum Order
    {
        Waiting,
        Critic
    }
    private Order order;
    private int totalReward = 0;
    private int trapReward = 0;
    private int preqAtkCount = 0;
    private int preqMS = 0;
    void Start()
    {
        BD = GetComponent<BossDecision>();
    }

    public async void SendAI(string note,int atkCount)
    {
        string observe = $"Observe,{bossNo},";
        observe += note;
        //Debug.Log($"Observe: {observe}");
        string action = "";
        SendReward(preqMS, atkCount);
        if (order == Order.Waiting)
        {
            action = await PipeHolder.SendObservationAsync(observe);
            //Debug.Log($"AI{bossNo} Response: {action}");
            order = Order.Critic;
        }

        BD.PackAndSend(action);
        
    }

    public void SendReward(int moveset,int atkCount)
    {
        if (order == Order.Critic)
        {
            int additionalMovesetReward = 0;
            if (atkCount == 0)
            {
                if (preqAtkCount == 1 && totalReward > 0) additionalMovesetReward += 3;
                if (preqAtkCount == 4) additionalMovesetReward -= 2;
            }
            string text = $"Reward,{bossNo},{moveset},{totalReward + additionalMovesetReward},{totalReward},{trapReward}," + (atkCount == 0 ? "True" : "False");
            PipeHolder.SendCommmand(text);
            order = Order.Waiting;
            totalReward = 0;
            trapReward = 0;
            preqAtkCount = atkCount;
        }
    }
    public void SendCommmand(string message)
    {
        PipeHolder.SendCommmand(message);
    }
    public void AccumReward(int reward)
    {
        totalReward += reward;
    }
    public void AccumTrapReward(int reward)
    {
        trapReward += reward;
    }
    public void SetPreqMS(int ms)
    {
        preqMS = ms;
    }

}


