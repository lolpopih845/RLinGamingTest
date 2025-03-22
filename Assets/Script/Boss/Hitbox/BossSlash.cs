using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Setup and Execute Boss Slashes
 Status : Done (Phase 1)
 Future Implementation : Anim (Phase 3)
 */
public class BossSlash : AttackHitbox
{
    protected Animator anim;
    protected float animTime, colDelay, colDuration;
    public virtual BossSlash SetUpHitbox(float length, float width, float dist, float angle, float animTime, float colDelay, float colDuration,bool real)
    {
        BeforeSetup(real);
        transform.localScale = new Vector3(length, width);
        transform.localPosition = new Vector3(dist * Mathf.Cos(angle * Mathf.PI / 180) * transform.parent.parent.localScale.x, dist * Mathf.Sin(angle * Mathf.PI / 180));
        transform.rotation = Quaternion.Euler(0, 0, angle);
        this.animTime = animTime;
        this.colDelay = colDelay;
        this.colDuration = colDuration;
        return this;
    }
    public override void Activate()
    {
        //Play anim
        StartCoroutine(Attacking());
    }
    private IEnumerator Attacking()
    {
        yield return new WaitForSeconds(colDelay);
        cold.enabled = true;
        if (reallly) spRen.color = new Color(1, 0.1f, 0, 1); //For Testing
        yield return new WaitForSeconds(colDuration);
        cold.enabled = false;
        spRen.color = new Color(1, 0.1f, 0, 0); //For Testing
        yield return new WaitForSeconds(animTime-colDelay-colDuration);
        Destroy(gameObject);
    }
}
