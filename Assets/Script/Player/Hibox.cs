using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    protected PlayerMovement PM;
    private void Start()
    {
        PM = GetComponentInParent<PlayerMovement>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PM)
        {
            PM.OnChildCollisionEnter(this,collision.gameObject);
        }
    }
}
