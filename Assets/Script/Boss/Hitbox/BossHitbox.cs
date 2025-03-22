using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    protected HitboxHandler hitboxHandler;
    protected Collider2D cold;
    protected void BeforeSetup()
    {
        hitboxHandler = GetComponentInParent<HitboxHandler>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (hitboxHandler)
        {
            hitboxHandler.OnChildCollisionEnter(gameObject, collision.gameObject);
        }
    }
}
