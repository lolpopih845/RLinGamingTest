using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Description : Handle Player control, Player Movement, Anim, Sfx, Floorcheck, Hit check, Send Hit, Hitbox
 Status : Unfinsihed (Phase 3)
 Future Implementation : Do it (Phase 3)
 */

//TODO : Wall collision conflict with enemy collision


public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D RB2;
    private PlayerCombat PC;
    [SerializeField] private Hitbox grapHB;

    private string[] actMapping = new string[] { "Jump","Dash","Attack","Special"};

    //ogowroedohnrihrptmormmehoenoegmdpnmepnmrpnmrpmprnmpenmpemnoenomgpbmeoboetbmenmoem
    public int look;
    public int dir = 1;
    private float dropDelay;
    
    //Flag
    private float iFrame = 0;
    private float holdJumpWindow;
    private bool attacking;
    public float disableMovement = 0;
    public float disableAction = 0;
    private float disableTurn = 0;
    private float disableGrav = 0;
    private float disableHold = 0;

    //Count
    private int dJumpCount = 1;
    private int dashCount = 1;
    private int atkCount = 1;
    private int divingCount = 1;


    void Start()
    {
        Environment.PlayerAnnounce(gameObject);
        RB2 = GetComponent<Rigidbody2D>();
        PC = GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        DepleteCd();
        FakeGrav();
        MoveAndLook();

        if (GroundCheck() || StepCheck())
        {
            ReCount();
        }
        grapHB.GetComponent<Collider2D>().enabled = look == 1;
    }

    #region BasicMovement
    private void MoveAndLook()
    {
        //sbrhonieoneothneorhmeotnhtehnoethnoetnhetnoenhooenthonetohnretohneothnoetnhoethoenhoemromeon
        look = (int)Input.GetAxisRaw("Vertical");
        if (disableTurn <= 0)
        {
            if (Input.GetAxisRaw("Horizontal") != 0) dir = (int)Input.GetAxisRaw("Horizontal");
        }
        if (disableMovement > 0) return;
        transform.Translate(PC.moveSpeed * Time.deltaTime * new Vector3(Input.GetAxis("Horizontal"), 0));
        
        if (look == -1) dropDelay = 0.2f;
    }
    public void Jump()
    {
        if(!(GroundCheck()||StepCheck()))
        {
            if (dJumpCount > 0) dJumpCount--;
            else return;
        }
        ReCount(ref divingCount);
        disableMovement = 0;
        //dropDelay = 0.1f;
        holdJumpWindow = 0.2f;
        RB2.velocity = new Vector3(RB2.velocity.x,0);
        RB2.AddForce(PC.jumpHeight/1.1f * Vector3.up, ForceMode2D.Impulse);
    }
    public void HoldJump()
    {
        if(holdJumpWindow > 0)
        {
            //dropDelay = 0.15f;
            RB2.AddForce(Vector3.up*1.25f, ForceMode2D.Force);
            holdJumpWindow -= Time.deltaTime;
        }
        
    }
    public void Dash()
    {
        if(!(GroundCheck()||StepCheck()) && divingCount>0)
        {
            divingCount = 0;
            StartCoroutine(Diving());
            
        }
        else
        {
            StartCoroutine(Dashing());
        }
    }
    private IEnumerator Dashing()
    {
        disableMovement = 0.5f;
        disableTurn = 0.5f;
        float timer = 0.5f;
        iFrame = PC.rollIFrame;
        while(timer> 0)
        {
            transform.Translate(10 * dir * Time.deltaTime * (timer+0.25f) * Vector3.right);
            timer -= Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator Diving()
    {
        while (!(GroundCheck() || StepCheck()) && divingCount <= 0)
        {
            transform.Translate(PC.moveSpeed * 2 * dir * Time.deltaTime * Vector3.right + 2 * Time.deltaTime * Vector3.down);
            disableMovement = 0.01f;
            disableTurn = 0.01f;
            disableHold = 0.01f;
            yield return null;
        }
        if((GroundCheck() || StepCheck()) && divingCount <= 0) StartCoroutine(Dashing());
    }
    public void Attack()
    {

    }
    public void HoldAttack()
    {

    }
    public void Special()
    {

    }
    #endregion

    #region Furniture Interaction
    public void OnChildCollisionEnter(Hitbox hitbox, GameObject gameObject)
    {
        if(hitbox == grapHB)
        {
            if (gameObject.layer == 7 || gameObject.layer == 8)
            {
                dJumpCount = 1;
                if(disableHold <= 0) StartCoroutine(HoldingState(gameObject));
            }
        }
    }

    private IEnumerator HoldingState(GameObject gameObject)
    {
        dJumpCount = 1;
        while (true)
        {
            RB2.velocity = new Vector2(0, 0);
            disableGrav = 0.1f;
            disableMovement = 0.1f;
            if (look == -1) { disableHold = 0.5f; disableGrav = 0f; disableMovement = 0; dashCount = 1; yield break; }
            if (dJumpCount <= 0) { disableHold = 0.5f; disableGrav = 0f; disableMovement = 0; dashCount = 1; RB2.AddForce(PC.jumpHeight / 1.1f * Vector3.up, ForceMode2D.Impulse); yield break;  }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) { Debug.Log("YO");  break; }
            yield return null;
        }
        float timey = gameObject.layer == 8?0.5f:0.35f;
        disableAction = timey;
        RB2.velocity = new Vector3(0, 0);
        disableHold = 0.5f;
        while (timey > 0)
        {
            disableGrav = 0.1f;
            disableMovement = 0.1f;
            transform.Translate(5 * Time.deltaTime * Vector3.up);
            timey -= Time.deltaTime;
            yield return null;
        }
        disableGrav = 0f; disableMovement = 0; dashCount = 1; yield break;
    }
    #endregion




    #region Public Space
    public bool IsRolling()
    {
        return iFrame > 0;
    }
    public void DoneDamage()
    {
        return;
    }
    #endregion


    #region Cooldown and Count
    private void ReCount()
    {
        dJumpCount = 1; atkCount = 1; dashCount = 1;
        if (atkCount > 1 && dashCount > 1) return;
    }
    private void ReCount(ref int h)
    {
        h = 1;
    }
    private void DepleteCd()
    {
        if(disableTurn > 0)disableTurn -= Time.deltaTime;
        if (disableMovement > 0) disableMovement -= Time.deltaTime;
        if (disableAction > 0) disableAction -= Time.deltaTime;
        if (iFrame > 0) iFrame -= Time.deltaTime;
        if (disableGrav > 0) disableGrav -= Time.deltaTime;
        if (disableHold > 0) disableHold -= Time.deltaTime;
    }
    #endregion


    #region Grav and coll
    public bool GroundCheck()
    {
        return Physics2D.Raycast(transform.position - new Vector3(0.5f, transform.localScale.y/2 - 0.25f), Vector3.down, 0.08f, LayerMask.GetMask("Floor")) || Physics2D.Raycast(transform.position - new Vector3(0, transform.localScale.y / 2 - 0.25f), Vector3.down, 0.08f, LayerMask.GetMask("Floor"));
    }
    private bool StepCheck()
    { 
         return dropDelay<=0 && (Physics2D.Raycast(transform.position - new Vector3(-0.5f, transform.localScale.y/2 - 0.25f), Vector3.down, 0.08f, LayerMask.GetMask("Step"))|| Physics2D.Raycast(transform.position - new Vector3(0.5f, transform.localScale.y / 2 - 0.25f), Vector3.down, 0.08f, LayerMask.GetMask("Step")));
    }
    private void FakeGrav()
    {
        if (disableGrav > 0) return;
        var gravityVector = Vector3.down * 0.961f;
        if(dropDelay>0) dropDelay -= Time.deltaTime;
        if (GroundCheck() || StepCheck()) RB2.velocity = new Vector2(RB2.velocity.x, Mathf.Max(0,RB2.velocity.y));
        else RB2.AddForce(gravityVector, ForceMode2D.Force);
    }
    #endregion

}
