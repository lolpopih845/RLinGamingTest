using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTraining : MonoBehaviour
{
    private PlayerMovement PM;
    private float dirCd = 0;
    void Start()
    {
        PM = GetComponent<PlayerMovement>();
        StartCoroutine(SimulateJump());
        StartCoroutine(SimulateDash());
    }

    void Update()
    {
        if (dirCd > 0) dirCd -= Time.deltaTime;
        if (PM.disableMovement > 0) return;
        transform.Translate(5 * Time.deltaTime * new Vector3(PM.dir, 0));
        if(dirCd<=0 && transform.position.x > 8.5f || transform.position.x < -8.5)
        {
            PM.dir *= -1;
            dirCd = 1;
        }
    }
    private IEnumerator SimulateJump()
    {
        while (true)
        {
            PM.Jump();
            float timey = Random.Range(0,0.2f);
            while (timey > 0)
            {
                PM.HoldJump();
                timey -= Time.deltaTime;
                PM.look = 1;
                yield return null;
            }
            PM.look = 0;
            yield return null;
            yield return new WaitForSeconds(Random.Range(0,0.5f));
        }
    }
    private IEnumerator SimulateDash()
    {
        while (true)
        {
            PM.Dash();
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }
}
