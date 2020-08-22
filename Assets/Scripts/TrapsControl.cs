using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapsControl : MonoBehaviour
{

    Rigidbody2D enemyRir;

    public enum TrapsType
    {
        Non, FallingPlatforms, Fire, Trampoline, SpikeHead
    }
    public TrapsType trapsType;
    bool isTrapStop = false;

   public bool isAttack = false;
    public int attackNum = 0;
    public float attackSpeed = 100f;
    public Vector2 disValue = Vector2.zero;
    private void Awake()
    {
        enemyRir = GetComponent<Rigidbody2D>();
        if (trapsType == TrapsType.SpikeHead)
        {
            StartCoroutine(EnemyAI());
        }
    }

    IEnumerator EnemyAI()
    {
        Transform playerTr = GameObject.Find("Player").transform;
        float checkDisValue = 20;
        Vector2 startPos = transform.position;

        while (playerTr)
        {
            if(isAttack)
            {
                if (disValue == Vector2.zero)
                {
                    switch (attackNum)
                    {
                        case 1:
                            disValue = Vector2.up;
                            break;
                        case 2:
                            disValue = Vector2.left;
                            break;
                        case 3:
                            disValue = Vector2.right;
                            break;
                        case 4:
                            disValue = Vector2.down;
                            break;
                    }
                }
                else
                {
                    Debug.Log(attackNum);
                    enemyRir.velocity = disValue * attackSpeed;
                }
            }
            else
            {
                int isLayerMask = 1 << LayerMask.NameToLayer("Player");

                if (Vector2.Distance(transform.position, startPos) == 0)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.up, checkDisValue, isLayerMask))
                    {
                        attackNum = 1;
                    }
                    else if (Physics2D.Raycast(transform.position, Vector2.left, checkDisValue, isLayerMask))
                    {
                        attackNum = 2;
                    }
                    else if (Physics2D.Raycast(transform.position, Vector2.right, checkDisValue, isLayerMask))
                    {
                        attackNum = 3;
                    }
                    else if (Physics2D.Raycast(transform.position, Vector2.down, checkDisValue, isLayerMask))
                    {
                        attackNum = 4;
                    }
                    if (attackNum != 0)
                    {
                        Debug.Log(attackNum);
                        gameObject.GetComponent<Animator>().SetInteger("attackNum", 0);
                        gameObject.GetComponent<Animator>().SetTrigger("IsAttack");
                        disValue = Vector2.zero;
                        isAttack = true;
                        yield return new WaitForSeconds(1f);
                    }
                }
                else
                {
                    enemyRir.velocity = Vector2.zero;
                    attackNum = 0;
                    transform.position = Vector2.MoveTowards(transform.position, startPos, Time.deltaTime * 20);
                }
            }
            yield return null;
        }
    }

    void OnBecameInvisible()
    {
        if (isTrapStop)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.CompareTag("Player"))
        {
            ContactPoint2D contact = col.contacts[0];
            Vector3 pos = (Vector2)transform.position - contact.point;

            switch (trapsType)
            {
                case TrapsType.FallingPlatforms:
                    if (col.transform.position.y > transform.position.y + 0.1f)
                    { 
                        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                    }
                    isTrapStop = true;
                    break;
                case TrapsType.Fire:
                    Debug.Log(gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("off"));
                    if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Off") && pos.normalized.y < -0.7f)
                    {
                        gameObject.GetComponent<Animator>().SetTrigger("Work");
                    }
                  
                    break;
                case TrapsType.Trampoline:
                    if (col.transform.position.y > transform.position.y + 0.1f)
                    {
                        col.transform.GetComponent<CharacterControl>().Jump(25,true);
                        gameObject.GetComponent<Animator>().SetTrigger("Working");
                    }
                    isTrapStop = true;
                    break;
                case TrapsType.SpikeHead:
                    //col.transform.GetComponent<CharacterControl>().PlayerDamage(0);
                    gameObject.GetComponent<Animator>().SetInteger("attackNum", attackNum);
                    attackNum = 0;
                    isAttack = false;
                    break;
            }
        }
        if (col.transform.CompareTag("Ground"))
        {
            switch (trapsType)
            {
                case TrapsType.SpikeHead:
                    gameObject.GetComponent<Animator>().SetInteger("attackNum", attackNum);
                    attackNum = 0;
                    isAttack = false;
                    break;
            }
        }
    }
}
