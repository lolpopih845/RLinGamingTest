using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Run Boss Intro/Outtro Seq, Middleware to Execute-Decision, Hold important State (Real/Fake)
 Status : Unfinished
 To Be Implemented : GroundCheck, Middleware, Real-Fake State (Phase 2), Intro/Outtro (Phase 3)
 */

public class BossMiddleware : MonoBehaviour
{
    private BossDecision BD;
    private BossExecute BE;
    private AttackPackage ap;

    [SerializeField] private bool amIReal = true;
    [SerializeField] private SpriteRenderer spriteR; 
    private float attackCD = 1;
    private bool finishedAttack=true;
    private bool attacking = false;
    private float recovEnd = 1;

    private GameObject[] Fakemen;
    private GameObject RealBoss;
    //Temp for multi Env
    public GameObject Player;

    //Temp
    public Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
        BD = GetComponent<BossDecision>();
        BE = GetComponent<BossExecute>();
        if (amIReal)
        {
            
            Fakemen = new GameObject[] { null,null,null,null,null,null,null, null, null};
            RealBoss = gameObject;
            StartCoroutine(StartBossIntro());
        }
        else
        {
            spriteR.color = new Color(1, 1, 1, 0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GroundCheck()||attacking&&amIReal) attackCD -= Time.deltaTime;
    }

    #region BossIntro
    private IEnumerator StartBossIntro()
    {
        //Wait for enter room

        //runintro
        yield return new WaitForSeconds(1);
        SummonFaker();
        StartCoroutine(AttackStart());
        yield return null;
    }
    #endregion


    #region AttackMiddleBox
    private IEnumerator AttackStart()
    {
        yield return new WaitUntil(() => GroundCheck() || attacking);
        //Debug.Log(GroundCheck());
        
        yield return new WaitWhile(() => attackCD > 0);
        BD.AttackNOW();
        attackCD = 999;
        yield return new WaitUntil(() => ap!=null);
        foreach (GameObject g in Fakemen)
        {
            StartCoroutine(g.GetComponent<BossMiddleware>().FakerAttackStart(!attacking, ap.moveset));
        }
        attacking = true;
        yield return new WaitForSeconds(0.05f);
        if (ap.moveset == 0)
        {
            attackCD = recovEnd + Random.Range(1f,3f);
            //Start Recov Anim?

            ap = null;
            attacking = false;

            StartCoroutine(AttackStart());
        }
        else
        {
            finishedAttack = false;
            BE.RecievePackage(ap);
            yield return new WaitUntil(() => finishedAttack);
            attackCD = ap.recoverTime;
            recovEnd = ap.recoverEnd;
            ap = null;
            StartCoroutine(AttackStart());
        }
        
    }



    #endregion


    #region Faker
    private void SummonFaker()
    {
        for(int i = 0; i < 9; i++)
        {
            try
            {
                if (Fakemen[i] == null)
                {
                    GameObject fakeman = Instantiate(gameObject, transform.position, Quaternion.identity);
                    fakeman.GetComponent<BossMiddleware>().YouAreFake(gameObject);
                    fakeman.GetComponent<AIDecision>().bossNo = i+1;
                    Fakemen[i] = fakeman;
                }
            }
            catch
            {
                GameObject fakeman = Instantiate(gameObject, transform.position, Quaternion.identity);
                fakeman.GetComponent<BossMiddleware>().YouAreFake(gameObject);
                fakeman.GetComponent<AIDecision>().bossNo = i+1;
                Fakemen[i] = fakeman;
            }
            
        }
        
    }
    public void YouAreFake(GameObject realOne)
    {
        amIReal = false;
        RealBoss = realOne;
    }
    public IEnumerator FakerAttackStart(bool start, int moveset)
    {
        
        transform.position = RealBoss.transform.position;
        if (attacking && moveset==0)
        {
            //ForceTrapProblem : Need to be fixed
            BE.ClearTrap();
            BD.EndCombo();
            BD.ResetAI(false, false, true);
            BD.SendReward();
            yield break;
        }
        if (!finishedAttack) yield break;
        else
        {
            if (start) attacking = true;
            if (attacking)
            {
                BD.AttackNOW(moveset);
                yield return new WaitUntil(() => ap != null);
                if (ap.moveset == 0)
                {
                    BD.SendReward();
                    ap = null;
                    attacking = false;
                }
                else
                {
                    finishedAttack = false;
                    BE.RecievePackage(ap);
                    yield return new WaitUntil(() => finishedAttack);
                    ap = null;
                }
            }
            
        }
    }
    #endregion


    #region Messenger

    public void PackageCome(AttackPackage ap)
    {
        this.ap = ap;
    }

    public void FinishAttacking()
    {
        finishedAttack = true;
    }

    #endregion


    #region Util (GroundCheck, Attacking, amIReal)
    public bool GroundCheck()
    {
        return Physics2D.Raycast(transform.position, Vector3.down, UtilV.bossHeight / 2 + 0.15f, LayerMask.GetMask("Floor"));
    }
    public bool IsAttacking()
    {
        return attacking;
    }
    public bool AmIReal()
    {
        return amIReal;
    }
    public int BossRot() //1 = right -1 = left
    {
        return (int) (transform.localScale.x / Mathf.Abs(transform.localScale.x));
    }
    #endregion 


    #region BossOutro
    #endregion



}
