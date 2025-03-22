using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Lanch Proj. when Activate
 Status : Done (Phase 1)
 Future Implementation : Anim (Phase 3)
 */

public class WaveProj : BossSlash
{
    private float angle;
    [SerializeField] private float speed;
    //length = bossx , width = bossY
    public BossSlash SetUpHitbox(float length, float width, float dist, float angle, float animTime, float colDelay, float colDuration, HitboxHandler hh,bool real)
    {
        BeforeSetup(hh,real);
        transform.SetPositionAndRotation(new Vector3(length,width), Quaternion.Euler(0, 0, angle));
        this.animTime = animTime;
        this.colDelay = colDelay;
        this.colDuration = colDuration;
        this.angle = angle;
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
        float time = animTime;
        cold.enabled = true;
        if(reallly) spRen.color = new Color(1, 0.1f, 0, 1); //For Testing
        while (time >= 0)
        {
            time -= Time.deltaTime;
            transform.localPosition += speed * Time.deltaTime * new Vector3(Mathf.Cos(angle * Mathf.PI / 180), Mathf.Sin(angle * Mathf.PI / 180));
            yield return null;
        }
        Destroy(gameObject);
    }
}
