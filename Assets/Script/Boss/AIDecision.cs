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
    private int preqAtkCount = 0;
    private int preqMS = 0;
    void Start()
    {
        BD = GetComponent<BossDecision>();
    }

    public async void SendAI(int moveset)
    {
        Vector3 playerPos = Environment.GetPlayer().transform.position;
        Vector3 playerVelos = Environment.GetPlayer().GetComponent<Rigidbody2D>().velocity;
        if (moveset == -1)
        {
            float [] poss = AIModel.GetSoftmaxOf(0, ClassyClassifier(playerPos), 1 + bossNo*0.1f*Mathf.Pow(-1,bossNo));
        }
        //string note = $"{playerPos.x},{playerPos.y},{transform.position.x},{transform.position.y},{playerVelos.x},{playerVelos.y},{atkCount},-1";
    }

    public void SendReward(int moveset,int atkCount)
    {

       
    }
    public void AccumReward(int reward)
    {
        totalReward += reward;
    }
    private int ClassyClassifier(Vector3 PlPos)//Horizontal * 9 + Vertical * 3 + Dist
    {
        int classy = 0;
        classy += ((PlPos.x<transform.position.x)?2:0 + ((PlPos.x > transform.position.x) ? 1 : 0))*9 + (PlPos.y+1 < transform.position.y?2:0 + PlPos.y - 1 > transform.position.y ? 1 : 0) *3 ;
        classy += (Vector3.Distance(PlPos, transform.position) < 1.5f ? 2 : 0);
        classy += (Vector3.Distance(PlPos, transform.position) < 3f ? 1 : 0);
        return classy;
    }
}


