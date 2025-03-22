using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Setup and Execute Player Slashes
 Status : Unfinished (Phase 3)
 Future Implementation : Do it (Phase 3)
 */

public class PlayerSlash : MonoBehaviour
{
    protected PlayerCombat PC;
    protected Collider2D cold;
    protected SpriteRenderer spRen;
    private Animator anim;
    private void Start()
    {
        PC = GetComponentInParent<PlayerCombat>();
        cold = GetComponent<Collider2D>();
        cold.enabled = false;
        spRen = GetComponent<SpriteRenderer>();
        spRen.color = new Color(1, 1, 1, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Send hit
    }
}
