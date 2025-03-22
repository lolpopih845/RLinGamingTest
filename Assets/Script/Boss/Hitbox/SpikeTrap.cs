using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Trap with hp and damage contacted playet
 Status : Done (Phase 1)
 Future Implementation : Player Dam (Phase 3), Anim (Phase 3)
 */

public class SpikeTrap : AttackHitbox
{
    private int hp;
    private Animator anim;
    public SpikeTrap SetUpHitbox(Vector3 pos, HitboxHandler hh,bool real)
    {
        BeforeSetup(hh,real);
        //Play Anim
        hp = 3;
        if(reallly) spRen.color = new Color(1, 1f, 1, 1); //For Testing
        transform.position = pos;
        //Play anim
        StartCoroutine(Attacking());
        StartCoroutine(Die());
        return this;
    }
    public override void Activate()
    {
        Destroy(gameObject);
    }
    private IEnumerator Attacking()
    {
        yield return new WaitForSeconds(2f);
        cold.enabled = true;
        if (reallly) spRen.color = new Color(1, 0.1f, 0, 1); //For Testing
    }
    public void Damage()
    {
        hp--;
    }
    private IEnumerator Die()
    {
        yield return new WaitUntil(() => hp <= 0);
        cold.enabled = false;
        //play death anim
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
