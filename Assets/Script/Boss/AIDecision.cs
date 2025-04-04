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
    private int totalReward = 0;
    private bool isReward = false;
    void Start()
    {
        BD = GetComponent<BossDecision>();
    }

    public void SendAI(int moveset)
    {
        if(isReward) SendReward();
        isReward = false;
        if (moveset == 0) {
            BD.PackAndSend("0,0,0,0,0");
            return;
        }
        totalReward = 0;

        //BD.PackAndSend(package + "," + moveset.ToString() + "," + atk.ToString() + "," + rot.ToString());
        GetComponent<AIAgent>().RequestDecision();
    }
    public void RecieveDecision(string decision)
    {
        BD.PackAndSend("0,0," + decision);
        isReward = true;
    }
    public void SendReward()
    {
        
    }
    public void AccumReward(int reward)
    {
        totalReward += reward;
    }
}


