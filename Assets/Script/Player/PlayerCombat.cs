using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Manage Stat/Perk, Execute Perk, Contain Env State (Boss HP), Declare Win, Vfx
 Status : Unfinsihed (Phase 3)
 Future Implementation : Do it (Phase 3)
 */

public class PlayerCombat : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    public float atkSp;
    public float rollIFrame;
    public float damageIFrame;

    private Collider2D colly;
    private SpriteRenderer spr;
    private float timely=0;

    //Temp
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        colly = GetComponent<Collider2D>();
        spr = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        timely += Time.deltaTime;
        if(transform.position.y<-10) transform.position = new Vector2(Random.Range(startPos.x + UtilV.BORDERLEFT, startPos.x + UtilV.BORDERRIGHT), Random.Range(startPos.y + UtilV.BORDERFLOOR, startPos.y + UtilV.BORDERTOP + 1));
    }
    public void Damage()
    {
        StartCoroutine(IFrame());
        Debug.Log(timely);
        timely = 0;
        transform.position = new Vector2(Random.Range(startPos.x + UtilV.BORDERLEFT, startPos.x+UtilV.BORDERRIGHT), Random.Range(startPos.y + UtilV.BORDERFLOOR, startPos.y + UtilV.BORDERTOP+1));
    }
    private IEnumerator IFrame()
    {
        colly.enabled = false;
        float time = damageIFrame;
        while (time > 0)
        {
            time -= 0.15f;
            spr.color = new Color(1, 1, 1,spr.color.a>0?0:1);
            yield return new WaitForSeconds(0.15f);
        }
        colly.enabled = true;
    }
}
