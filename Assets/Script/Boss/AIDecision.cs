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
    private readonly Rewarder[] buffer = new Rewarder[3];
    void Start()
    {
        BD = GetComponent<BossDecision>();
    }

    public void SendAI(int moveset)
    {
        string package = "0,0";
        Vector3 playerPos = Environment.GetPlayer().transform.position;
        if(isReward) SendReward();
        isReward = false;
        int classy = ClassyClassifier(playerPos);
        float temper = 1 + bossNo * 0.1f * Mathf.Pow(-1, bossNo);
        if (moveset == -1 || bossNo>=5)
        {
            float[] poss = AIModel.GetSoftmaxOf(0, classy, temper);
            moveset = Randomizer(poss);
        }
        buffer[0] = new Rewarder().Set(0, classy, moveset, totalReward);
        if (moveset == 0) {
            BD.PackAndSend(package + ",0,0,0");
            return;
        }
        int atk = Randomizer(AIModel.GetSoftmaxOf(1, classy, temper, moveset));
        int rot = Randomizer(AIModel.GetSoftmaxOf(2, classy, temper, moveset));
        buffer[0] = new Rewarder().Set(0,classy, moveset, totalReward);
        buffer[1] = new Rewarder().Set(1, classy* moveset, atk , totalReward);
        buffer[2] = new Rewarder().Set(2, classy* moveset, rot, totalReward);
        totalReward = 0;
        BD.PackAndSend(package + "," + moveset.ToString() + "," + atk.ToString() + "," + rot.ToString());
        isReward = true;
    }

    public void SendReward()
    {
        foreach(Rewarder r in buffer)
        {
            AIModel.SendReward(r);
        }
    }
    public void AccumReward(int reward)
    {
        totalReward += reward;
    }
    private int ClassyClassifier(Vector3 PlPos)//Horizontal * 9 + Vertical * 3 + Dist
    {
        int classy = 0;
        if (PlPos.x < transform.position.x) classy += 18;
        else if (PlPos.x > transform.position.x) classy += 9;
        if (PlPos.y + 1 < transform.position.y) classy += 6;
        else if (PlPos.y - 1 > transform.position.y) classy += 3;
        if (Vector3.Distance(PlPos, transform.position) < 1.5f) classy += 2;
        else if (Vector3.Distance(PlPos, transform.position) < 3f) classy++;
        return classy;
    }
    private int Randomizer(float[] poss)
    {
        float randy = Random.Range(0f, 1f);
        for (int i = 0; i < poss.Length; i++)
        {
            randy -= poss[i];
            if (randy <= 0)
            {
                return i;
            }
        }
        return 0;
    }
}


