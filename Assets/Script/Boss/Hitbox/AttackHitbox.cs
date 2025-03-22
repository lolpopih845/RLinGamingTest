using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Key : Abstract for all attack hitbox
 Description : Setup and Execute Attack Hitbox
 Status : Done
 */

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class AttackHitbox : MonoBehaviour
{
    protected HitboxHandler hitboxHandler;
    protected Collider2D cold;
    protected SpriteRenderer spRen;
    protected bool reallly = false;
    protected void BeforeSetup(bool real)
    {
        hitboxHandler = GetComponentInParent<HitboxHandler>();
        cold = GetComponent<Collider2D>();
        cold.enabled = false;
        spRen = GetComponent<SpriteRenderer>();
        spRen.color = new Color(1, 1, 1, 0);
        reallly = real;
    }
    protected void BeforeSetup(HitboxHandler hh, bool real)
    {
        hitboxHandler = hh;
        cold = GetComponent<Collider2D>();
        cold.enabled = false;
        spRen = GetComponent<SpriteRenderer>();
        spRen.color = new Color(1, 1, 1, 0);
        reallly = real;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitboxHandler)
        {
            hitboxHandler.OnChildCollisionEnter(gameObject,collision.gameObject);
        }
    }
    public abstract void Activate();
}
