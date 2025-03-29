using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Rewarder
{
    public int mode;
    public int observe;
    public int iter;
    public int reward;
};
public static class AIModel
{
    private static readonly (int, int)[,] moveSetSoftmaxData = new (int, int)[27, 13];
    private static readonly (int, int)[,] atkTimeSoftmaxData = new (int, int)[13,13];
    private static readonly (int, int)[,] rotSoftmaxData = new (int, int)[13,13];
    private static readonly (int, int)[][,] allData;

    public static void LoadData()
    {
        //Load file


        //If no save
        for (int i = 0; i < 27; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                moveSetSoftmaxData[i, j] = (0, 0);
            }
        }
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                atkTimeSoftmaxData[i, j] = (0, 0);
                rotSoftmaxData[i, j] = (0, 0);
            }
        }
        allData[0] = moveSetSoftmaxData;
        allData[1] = atkTimeSoftmaxData;
        allData[2] = rotSoftmaxData;
    }
    public static void SaveData()
    {

    }
    public static float[] GetSoftmaxOf(int mode,int observe,float temper)// 0->ms 1->time 2->rot
    {
        float[] possible = new float[13];
        for(int i=0;i<allData[mode].Length;i++)
        {
            possible[i] = allData[mode][observe, i].Item1/ allData[mode][observe, i].Item2;
        }
        possible = Softmax(possible,temper);
        return possible;
    }

    public static void SendReward(Rewarder rewarder)
    {
        allData[rewarder.mode][rewarder.observe, rewarder.iter].Item1 += rewarder.reward;
        allData[rewarder.mode][rewarder.observe, rewarder.iter].Item2++;
    }

    public static float[] Softmax(float[] logits,float temper)
    {
        float[] expScores = new float[logits.Length];
        float sumExpScores = 0.0f;

        for (int i = 0; i < logits.Length; i++)
        {
            expScores[i] = Mathf.Exp(logits[i] / temper);
            sumExpScores += expScores[i];
        }

        float[] probabilities = new float[logits.Length];
        for (int i = 0; i < logits.Length; i++)
        {
            probabilities[i] = expScores[i] / sumExpScores;
        }

        return probabilities;
    }
}
