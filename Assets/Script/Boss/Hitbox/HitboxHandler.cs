using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Create and Execute Hitbox, Send reward to AI
 Status : Unfinished
 To Be Implemented : Proj, Reward (Phase 2)
 */

public class HitboxHandler : MonoBehaviour
{
    private AIDecision AIboi;
    private BossMiddleware BM;

    [Header("Hitbox Prefab")]
    [SerializeField] private TelegraphHitbox squareHitbox;
    [SerializeField] private TelegraphHitbox circleHitbox;
    [SerializeField] private AttackHitbox[] attackPrefabs; //Start with one


    private readonly Queue<BossSlash> attackHitboxesQueue = new();
    private readonly List<AttackHitbox> soulTraps = new();
    private SwordTrap swordTrap;
    private readonly List<AttackHitbox> spikeTraps = new();

    void Start()
    {
        AIboi = GetComponentInParent<AIDecision>();
        BM = GetComponentInParent<BossMiddleware>();
    }

    #region Execute Hitbox
    
    public void activateHitbox(AttackPackage ap, bool telegraph)
    {
        if (telegraph)
        {
            Telegraph(ap);
        }
        else
        {
            Attack(ap);
        }
    }

    #region SetUpTelegraph

    //TODO: Setup Slash timing is still depend on anim -> Placeholder for now

    private void Telegraph(AttackPackage ap)
    {
        int moveset = ap.moveset;
        bool realboi = BM.AmIReal();
        switch (moveset)
        {
            case 1:
                squareHitbox.SetHitbox(2, 3, 2* transform.parent.localScale.x, 0, realboi);
                squareHitbox.StartTelegraphHitbox(ap.telegraphTime);
                attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[moveset], transform)).SetUpHitbox(2, 3, 2 * transform.parent.localScale.x, 0, 1.5f, 0.5f, 0.25f, realboi));
                break;

            case 2:
                squareHitbox.SetHitbox(6, 1.5f, 0, ap.rotation, realboi);
                squareHitbox.StartTelegraphHitbox(ap.telegraphTime);
                attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[moveset], transform)).SetUpHitbox(6, 1.5f, 0, ap.rotation, 1.5f, 0.5f, 0.25f, realboi));
                break;
            case 3:
                if(ap.atkCombo == 1)
                {
                    squareHitbox.SetHitbox(2, 3, 0, ap.rotation, realboi);
                    squareHitbox.StartTelegraphHitbox(ap.telegraphTime,true,7.5f);
                }
                else if(ap.atkCombo == 2)
                {
                    squareHitbox.SetHitbox(3, 2, 2, ap.rotation, realboi);
                    squareHitbox.StartTelegraphHitbox(ap.telegraphTime);
                    attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[moveset], transform)).SetUpHitbox(3, 2, 2, ap.rotation, 1.5f, 0.5f, 0.25f, realboi));
                }
                break;
            case 4:
                squareHitbox.SetHitbox(1, 4f, -1, 90, realboi);
                squareHitbox.StartTelegraphHitbox(ap.telegraphTime,true,7.5f);
                attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[moveset], transform)).SetUpHitbox(2, 3, 2 * transform.parent.localScale.x, 0, 1.5f,0, 0.6f, realboi));
                break;
            case 5:
                squareHitbox.SetHitbox(3, 1.5f, 3, ap.rotation, realboi);
                squareHitbox.StartTelegraphHitbox(ap.telegraphTime);
                attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[moveset], transform)).SetUpHitbox(3, 1.5f, 3 , ap.rotation, 1.5f, 0.5f, 0.25f, realboi));
                break;
            case 6:
                if (ap.atkCombo == 2) break;
                for(int i = 0; i < 4; i++)
                {
                    soulTraps.Add(((SoulTrap)Instantiate(attackPrefabs[moveset])).SetUpHitbox(ap.pos[i],this, realboi));
                }
                break;
            case 7:
                if (ap.atkCombo == 2) break;
                swordTrap =(((SwordTrap)Instantiate(attackPrefabs[moveset])).SetUpHitbox(this, realboi));
                break;
            case 8:
                squareHitbox.SetHitbox(3, 3.5f, 0, ap.rotation, realboi);
                squareHitbox.StartTelegraphHitbox(ap.telegraphTime, true, 5);
                attackHitboxesQueue.Enqueue(((WaveProj)Instantiate(attackPrefabs[moveset])).SetUpHitbox(transform.position.x, transform.position.y, 2 * Environment.BossFacePlayer(transform), ap.rotation, 1.5f, 0.5f, 0.25f,this, realboi));
                //Projs
                break;
            case 9:
                if (ap.atkCombo == 1)
                {
                    circleHitbox.SetHitbox(5, 5, 0, 0, realboi);
                    circleHitbox.StartTelegraphHitbox(ap.telegraphTime);
                    attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[moveset], transform)).SetUpHitbox(5, 5f, 0, 0, 1.5f, 0.5f, 0.25f, realboi));
                }
                else
                {
                    squareHitbox.SetHitbox(1, 4f, -1 * Environment.BossFacePlayer(transform), -90, realboi);
                    squareHitbox.StartTelegraphHitbox(ap.telegraphTime, true, 7.5f);
                    attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[1], transform)).SetUpHitbox(2, -3, 2 * Environment.BossFacePlayer(transform), 0, 1.5f, 0f, 0.5f, realboi));
                }
                break;
            case 10:
                squareHitbox.SetHitbox(2, 1f, 2 * transform.parent.localScale.x, 0, realboi);
                squareHitbox.StartTelegraphHitbox(ap.telegraphTime);
                attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[moveset], transform)).SetUpHitbox(2, 1, 2 * transform.parent.localScale.x ,0, 1.5f, 0.5f, 0.25f, realboi));
                break;
            case 11:
                StartCoroutine(A11Telegraph(ap));
                break;
            case 12:
                if (ap.atkCombo == 2) break;
                ClearTrap(spikeTraps);
                for (int i = 0; i < 4; i++)
                {
                    spikeTraps.Add(((SpikeTrap)Instantiate(attackPrefabs[moveset])).SetUpHitbox(ap.pos[i], this, realboi));
                }
                break;
                
        }
    }
    private IEnumerator A11Telegraph(AttackPackage ap)
    {
        for(int i = 0; i < 3; i++)
        {
            squareHitbox.SetHitbox(2f, 3f, (1.5f+1.5f*i) * transform.parent.localScale.x, 0,BM.AmIReal());
            squareHitbox.StartTelegraphHitbox(ap.telegraphTime);
            if(i<2) attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[11], transform)).SetUpHitbox(2, 3f, 1.5f * transform.parent.localScale.x, 0, 1.5f, 0, 0.25f, BM.AmIReal()));
            else attackHitboxesQueue.Enqueue(((BossSlash)Instantiate(attackPrefabs[3], transform)).SetUpHitbox(2, 2f, 1.5f * transform.parent.localScale.x, 0, 1.5f, 0, 0.25f, BM.AmIReal()));
            yield return new WaitForSeconds(ap.telegraphTime / 3);
        }
        
    }

    #endregion

    #region ActivateAttack
    private void Attack(AttackPackage ap)
    {
        if(ap.moveset == 6 && ap.atkCombo == 2)
        {
            ClearTrap(soulTraps);
        }
        else if(ap.moveset == 7 && ap.atkCombo == 2)
        {
            swordTrap.Activate();
        }
        else if(ap.moveset == 11)
        {
            StartCoroutine(A11Attack(ap));

        }
        else
        {
            if(attackHitboxesQueue.Count>0) attackHitboxesQueue.Dequeue().Activate();
        }
        
    }
    private void ClearTrap(List<AttackHitbox> a)
    {
        foreach (AttackHitbox st in a)
        {
            st.Activate();
        }
        a.Clear();
    }
    private IEnumerator A11Attack(AttackPackage ap)
    {
        for (int i = 0; i < 3; i++)
        {
            attackHitboxesQueue.Dequeue().Activate();
            yield return new WaitForSeconds(ap.attackTime / 3);
        }

    }
    #endregion

    #endregion

    #region Send Reward
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnChildCollisionEnter(gameObject,collision.gameObject);
    }
    public void OnChildCollisionEnter(GameObject child,GameObject hit)
    {
        if (hit.TryGetComponent(out PlayerMovement PM))
        {
            if (PM.IsRolling())
            {
                if (BM.AmIReal()) AIboi.AccumReward(5);
                else AIboi.AccumReward(2);
            }
            else if(child.TryGetComponent(out SoulTrap _) || child.TryGetComponent(out SpikeTrap _)){
                if (BM.AmIReal())
                {
                    Environment.DamagePlayer();
                    AIboi.AccumReward(2);
                }

            }
            else
            {
                
                if (BM.AmIReal())
                {
                    Environment.DamagePlayer();
                    AIboi.AccumReward(10);
                }
                else
                {
                    AIboi.AccumReward(6);
                }
            }
        }

    }
    #endregion

    #region Misc
    public void ClearTrap()
    {
        ClearTrap(soulTraps);
        ClearTrap(spikeTraps);
        if(swordTrap!=null) Destroy(swordTrap.gameObject);
    }
    private void OnDestroy()
    {
        ClearTrap(soulTraps);
        ClearTrap(spikeTraps);
    }
    #endregion



}
