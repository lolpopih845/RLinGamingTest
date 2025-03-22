using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Trap that stay on board and explode when activate
 Status : Done (Phase 1)
 Future Implementation : Anim (Phase 3)
 */

public class SoulTrap : AttackHitbox
{
    private Animator anim;
    public SoulTrap SetUpHitbox(Vector3 pos, HitboxHandler hh,bool real)
    {
        BeforeSetup(hh, real);
        //Play Anim
        transform.position = pos;
        if (reallly) spRen.color = new Color(1, 1, 1, 1);
        return this;
    }
    public override void Activate()
    {
        //Play anim
        StartCoroutine(Attacking());
    }
    private IEnumerator Attacking()
    {
        spRen.transform.localScale = new Vector3(0.5f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        cold.enabled = true;
        spRen.transform.localScale = new Vector3(2f, 2f);
        //Explode (Play Anim, coll get bigger)
        yield return new WaitForSeconds(0.3f);
        cold.enabled = false;
        spRen.color = new Color(1, 0.1f, 0, 0); //For Testing
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
