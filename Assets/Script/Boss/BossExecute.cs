using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Recieve Attack Package, Execute Attack (Send Hitbox to work, move boss around), Hold Combat Info, Doing Idle work
 Status : Unfinished
 To Be Implemented : Combat info?, Idle, Out-of-bound
 */



public class BossExecute : MonoBehaviour
{
    [Range(1,3)]
    [SerializeField] private int combo; //For testing
    private BossMiddleware BM;
    
    private Rigidbody2D RB;

    private HitboxHandler HH;

    private bool onGroundState = false;

    private float truIdleTimer = 2;

    
    void Start()
    {
        BM = GetComponent<BossMiddleware>();
        RB = GetComponent<Rigidbody2D>();
        HH = GetComponentInChildren<HitboxHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        PreventOotOfBound(transform,new Vector3(UtilV.bossWidth,UtilV.bossHeight));
        if(!BM.IsAttacking())
        {
            Idling();
        }
    }

    #region Idle
    private void Idling()
    {
        if(!BM.GroundCheck())
        {
            FakeGrav();
            onGroundState = false;
            return;
        }
        else if (!onGroundState)
        {
            onGroundState = true;
            RB.velocity = new Vector2(0, 0);
            //run Drop anim
            truIdleTimer = Random.Range(0.4f,2);
        }
        else if(truIdleTimer > 0)
        {
            truIdleTimer -= Time.deltaTime;
        }
        else
        {
            //if (Mathf.Abs(Environment.GetPlayer().transform.position.x - transform.position.x) > UtilV.bossWidth/2 + 0.5f)
            //{
            //    transform.localScale = new Vector3(Environment.BossFacePlayer(transform), 1);
            //    transform.position += new Vector3(Environment.BossFacePlayer(transform) * Time.deltaTime, 0);
            //}
            if (Mathf.Abs(BM.Player.transform.position.x - transform.position.x) > UtilV.bossWidth / 2 + 0.5f)
            {
                transform.localScale = new Vector3(BM.Player.transform.position.x > transform.position.x ? 1 : -1, 1);
                transform.position += new Vector3(BM.Player.transform.position.x > transform.position.x ? 1 : -1 * Time.deltaTime, 0);
            }
        }
    }
    #endregion

    #region Execute Attack
    public void RecievePackage(AttackPackage ap)
    {
        int moveset = ap.moveset;
        //Execute Anim

        //ExecuteHitbox with exception
        StartCoroutine(ExecuteHitbox(ap));

        //Unique
        switch (moveset)
        {
            case 1:
                if (!BM.GroundCheck()) StartCoroutine(MoveBoss(ap.telegraphTime/2, -90 -45*transform.localScale.x, 8f, 0.5f));
                else if (ap.atkCombo == 2) StartCoroutine(MoveBoss(ap.telegraphTime/2,0, transform.localScale.x * 2f, 0.5f));
                break;
            case 3:
                if(ap.atkCombo == 1) StartCoroutine(MoveBoss(ap.telegraphTime, ap.rotation, 7.5f, ap.attackTime));
                break;
            case 4:
                StartCoroutine(MoveBoss(ap.telegraphTime, 90,5, 0.4f));
                break;
            case 7:
                if (ap.atkCombo == 2) StartCoroutine(MoveBoss(ap.telegraphTime, -90, 10, 0.2f));
                break;
            case 9:
                if (ap.atkCombo == 1) StartCoroutine(MoveBoss(ap.telegraphTime, ap.rotation, 3, 0.75f));
                if (ap.atkCombo == 2) StartCoroutine(MoveBoss(ap.telegraphTime + 0.2f, -90, 10, 0.2f));
                break;
            case 11:
                StartCoroutine(MoveBoss(ap.telegraphTime, 0, transform.localScale.x * 1.5f, ap.attackTime / 3));
                StartCoroutine(MoveBoss(ap.telegraphTime + ap.attackTime/3, 0, transform.localScale.x * 1.5f, ap.attackTime / 3));
                StartCoroutine(MoveBoss(ap.telegraphTime + ap.attackTime*2/3, 0, transform.localScale.x * 1.5f, ap.attackTime / 3));
                break;
        }
    }
    private IEnumerator ExecuteHitbox(AttackPackage ap)
    {
        if (ap.rotation > 90 && ap.rotation <= 180 || (ap.rotation < -90 && ap.rotation >= -180)) transform.localScale = new Vector3(transform.localScale.x * -1, 1);
        HH.activateHitbox(ap, true);
        yield return new WaitForSeconds(ap.telegraphTime);
        HH.activateHitbox(ap, false);
        yield return new WaitForSeconds(ap.attackTime);
        BM.FinishAttacking();
    }
    #endregion

    #region MoveSet Function
    private IEnumerator MoveBoss(float telegraphTime, float rot,float dist, float time)
    {
        yield return new WaitForSeconds(telegraphTime);
        float maxtime = time;
        while (time >= 0)
        {
            time -= Time.deltaTime;
            transform.position += dist/((maxtime-time)*8) * Time.deltaTime * new Vector3(Mathf.Cos(rot * Mathf.PI / 180), Mathf.Sin(rot * Mathf.PI / 180));
            PreventOotOfBound(transform, new Vector3(UtilV.bossWidth, UtilV.bossHeight));
            yield return null;
        }
        
    }

    //Temp
    private void PreventOotOfBound(Transform t, Vector3 scale)
    {
        float x = t.position.x, y = t.position.y;
        if (t.position.x < BM.startPos.x + UtilV.BORDERLEFT + scale.x / 2)
        {
            x = BM.startPos.x + UtilV.BORDERLEFT + scale.x / 2;
        }
        if (t.position.x > BM.startPos.x + UtilV.BORDERRIGHT - scale.x / 2)
        {
            x = BM.startPos.x + UtilV.BORDERRIGHT - scale.x / 2;
        }
        if (t.position.y < BM.startPos.y + UtilV.BORDERFLOOR + scale.y / 2)
        {
            y = BM.startPos.y + UtilV.BORDERFLOOR + scale.y / 2;
        }
        if (t.position.y > BM.startPos.y + UtilV.BORDERTOP - scale.y / 2)
        {
            y = BM.startPos.y + UtilV.BORDERTOP - scale.y / 2;
        }
        t.position = new Vector3(x, y);
    }
    #endregion

    #region Misc
    private void FakeGrav()
    {
        var gravityVector = Vector3.down * 0.961f;
        if (BM.GroundCheck()) RB.velocity = new Vector2(RB.velocity.x, 0);
        else RB.AddForce(gravityVector, ForceMode2D.Force);
    }
    public void ClearTrap()
    {
        HH.ClearTrap();
    }
    #endregion

}
