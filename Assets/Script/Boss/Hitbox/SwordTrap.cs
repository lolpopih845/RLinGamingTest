using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Trap that hid on ground and raise up when activate
 Status : Done (Phase 1)
 Future Implementation : Anim (Phase 3)
 */

public class SwordTrap : AttackHitbox
{
    private Animator anim;
    public SwordTrap SetUpHitbox(HitboxHandler hh, bool real)
    {
        BeforeSetup(hh, real);
        //Play anim
        //set Pos (Later)
        if (reallly) spRen.color = new Color(1, 1, 1, 1); //For Testing
        
        return this;
    }
    public override void Activate()
    {
        //Play anim
        StartCoroutine(Attacking());
    }
    private IEnumerator Attacking()
    {
        //Anim
        yield return new WaitForSeconds(0.25f);
        cold.enabled = true;
        transform.position = new Vector3(0, -3.5f);
        yield return new WaitForSeconds(1.5f);
        cold.enabled = false;
        //Anim
        spRen.color = new Color(1, 0.1f, 0, 0); //For Testing
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
