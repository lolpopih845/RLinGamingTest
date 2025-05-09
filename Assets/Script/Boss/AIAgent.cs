using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AIAgent : Agent
{
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Environment.GetPlayer().transform.position.x);
        sensor.AddObservation(Environment.GetPlayer().transform.position.y);
        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.y);
        sensor.AddObservation(GetComponent<BossDecision>().atkCount);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> dec = actions.DiscreteActions;
        //Debug.Log(dec[0]);
        GetComponent<AIDecision>().RecieveDecision(dec[0].ToString()+ ","+ dec[1].ToString() + "," + dec[2].ToString());
        
    }
    public void SendReward(float reward, bool end = false)
    {
        SetReward(reward);
        EndEpisode();
    }
}
