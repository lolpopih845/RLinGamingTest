using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Setup and Execute Telegraph Hitbox
 Status : Done (Phase 1)
 Future Implementation : Decision on visible telegraph?, Anim?
 */

public class TelegraphHitbox : MonoBehaviour
{
    private Collider2D collidery;
    private SpriteRenderer sprRen;
    private HitboxHandler hitboxHandler;
    private bool really = false;

    private void Start()
    {
        sprRen = GetComponent<SpriteRenderer>();
        sprRen.color = new Color(1, 0.1f, 0, 0);
        transform.parent.TryGetComponent(out HitboxHandler hitbox);
        hitboxHandler = hitbox;
    }

    #region Execute Hitbox
    public void SetHitbox(float length,float width,float dist,float angle,bool real)
    {
        transform.localScale = new Vector3(length, width);
        transform.localPosition = new Vector3(dist * Mathf.Cos(angle * Mathf.PI / 180) * transform.parent.parent.localScale.x, dist * Mathf.Sin(angle * Mathf.PI / 180));
        transform.rotation = Quaternion.Euler(0, 0, angle);
        really = real;
    }

    public void StartTelegraphHitbox(float time)
    {
        StartCoroutine(DoingTelegraph(time, false,0));
    }

    //for testing
    public void StartTelegraphHitbox(float time, bool extended, float increasewidth)
    {
        StartCoroutine(DoingTelegraph(time, extended, increasewidth));
    }

    private IEnumerator DoingTelegraph(float timer, bool extended, float increasewidth)
    {
        float maxtimer = timer;
        if (!really) yield break;
        //collidery.enabled = true;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            //flash
            if (timer > maxtimer * 3 / 4)
            {
                sprRen.color = new Color(1, 0.1f, 0, 1 - (timer - maxtimer * 3 / 4) / timer);
            }
            else
            {
                sprRen.color = new Color(1, 0.1f, 0, timer / (maxtimer * 3 / 4));
                
            }
            if (extended)
            {
                CalculateExtension(maxtimer, increasewidth);
            }
            yield return null;
        }
        sprRen.color = new Color(1, 0.1f, 0, 0);
    }


    #endregion

    #region Misc
    private void CalculateExtension(float maxtimer,float increasewidth)
    {

        transform.localScale += new Vector3(increasewidth / maxtimer * Time.deltaTime, 0, 0);
        transform.position += new Vector3(increasewidth / maxtimer / 2 * Time.deltaTime * Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.PI / 180), increasewidth / maxtimer / 2 * Time.deltaTime * Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.PI / 180));
    }
    #endregion

}
