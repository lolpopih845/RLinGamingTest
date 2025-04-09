using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public struct Rewarder
{
    
    public int mode;
    public int observe;
    public int iter;
    public int reward;
    public Rewarder Set(int mode,int observe,int iter,int reward)
    {
        this.mode = mode;
        this.observe = observe;
        this.iter = iter;
        this.reward = reward;
        return this;
    }
};
[System.Serializable]
public struct SoftMaxData
{
    public int[] moveSetSoftmaxData;
    public int[] atkTimeSoftmaxData;
    public int[] rotSoftmaxData;
}
public static class AIModel
{
    private static readonly (int, int)[,] moveSetSoftmaxData = new (int, int)[27, 13];
    private static readonly (int, int)[,] atkTimeSoftmaxData = new (int, int)[104,11];
    private static readonly (int, int)[,] rotSoftmaxData = new (int, int)[104,11];
    private static readonly (int, int)[][,] allData = new(int,int)[3][,];

    public static void LoadData()
    {
        //Load file
        

        //If no save
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                moveSetSoftmaxData[i, j] = (0, 0);
            }
        }
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                atkTimeSoftmaxData[i, j] = (0, 0);
                rotSoftmaxData[i, j] = (0, 0);
            }
        }
        allData[0] = moveSetSoftmaxData;
        allData[1] = atkTimeSoftmaxData;
        allData[2] = rotSoftmaxData;
    }
    public static void LoadData(SoftMaxData SMD)
    {
        //Load file
        allData[0] = ExpandArray(SMD.moveSetSoftmaxData,27,13);
        allData[1] = ExpandArray(SMD.atkTimeSoftmaxData,104,11);
        allData[2] = ExpandArray(SMD.rotSoftmaxData,104,11);
    }
    public static void SaveData(ref SoftMaxData sdm)
    {
        sdm.moveSetSoftmaxData = FlattenArray(allData[0]);
        sdm.atkTimeSoftmaxData = FlattenArray(allData[1]);
        sdm.rotSoftmaxData = FlattenArray(allData[2]);
        return;
    }
    public static float[] GetSoftmaxOf(int mode,int observe,float temper,int moveset=1)// 0->ms 1->time 2->rot
    {
        observe *= moveset;
        float[] possible = new float[allData[mode].GetLength(1)];
        for(int i=0;i<allData[mode].GetLength(1);i++)
        {
            if (allData[mode][observe, i].Item2 == 0) possible[i] = 0;
            else possible[i] = allData[mode][observe, i].Item1/ allData[mode][observe, i].Item2;
        }
        possible = Softmax(possible,temper);
        return possible;
    }

    public static void SendReward(Rewarder rewarder)
    {
        allData[rewarder.mode][rewarder.observe, rewarder.iter].Item1 += rewarder.reward;
        allData[rewarder.mode][rewarder.observe, rewarder.iter].Item2++;
        if(allData[rewarder.mode][rewarder.observe, rewarder.iter].Item2 > 1000)
        {
            allData[rewarder.mode][rewarder.observe, rewarder.iter].Item1 /= 10;
            allData[rewarder.mode][rewarder.observe, rewarder.iter].Item2 /= 10;
        }
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

    private static int[] FlattenArray((int, int)[,] arr2D)
    {
        int [] res = new int[arr2D.GetLength(0) * arr2D.GetLength(1) * 2];
        int index = 0;
        for(int i =0;i< arr2D.GetLength(0); i++)
        {
            for (int j = 0; j < arr2D.GetLength(1); j++)
            {
                res[index++] = arr2D[i, j].Item1;
                res[index++] = arr2D[i, j].Item2;
            }
        }
        return res;
    }
    private static (int, int)[,] ExpandArray(int[] arr,int row,int coll)
    {
        (int, int)[,] res = new (int, int)[row,coll];
        int index = 0;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < coll; j++)
            {
                res[i,j].Item1 = arr[index++];
                res[i, j].Item2 = arr[index++];
            }
        }
        return res;
    }
}
