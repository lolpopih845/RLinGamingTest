using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Keep track of environment to be use for AI or others
 Status : Unfinished
 Future Implementation : Player pos,rot,velocity
 */

public static class Environment
{
    public static GameObject Player;
    public static GameObject RealBoss;
    public static int level = 10;
    public static void PlayerAnnounce(GameObject game)
    {
        Player = game;
    }
    public static int BossFacePlayer(Transform boss)
    {
        Transform player = GameObject.Find("Player").transform; //For testing
        return player.position.x > boss.position.x ? 1 : -1;
    }
    public static GameObject GetPlayer()
    {
        return Player;
    }
    public static void DamagePlayer()
    {
        Player.GetComponent<PlayerCombat>().Damage();
    }
    public static int GetLevel()
    {
        return level;
    }
}
