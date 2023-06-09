using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAttack : MonoBehaviour
{
    private GameObject pigeon;
    private GuardDetect guardDetect;
    private GuardPatrol guardPatrol;
    private Rigidbody2D guardBody;
    public bool isReadyToAttack;
    public bool isAttacking;
    public bool isAttackingPaused;
    public bool canBeNotified;
    public float attackStartDelayTimer;
    private float attackTimer;
    private float attackStopDelayTimer;
    public float attackStartDelayMin = 1f;
    public float attackStartDelayMax = 3f;
    private float attackStartDelayDuration;
    public float attackDuration = 3f;
    public float attackStopDelayDuration = 2f;
    public float notifyRadius = 2f;
    public static bool killPigeon;

    private GameObject indicatorFound;

    public event EventHandler<EventArgs> StopAttacking;

    // Start is called before the first frame update
    void Start()
    {
        pigeon = FindObjectOfType<PigeonMovement>().gameObject;
        guardDetect = transform.parent.GetComponentInChildren<GuardDetect>();
        guardPatrol = GetComponent<GuardPatrol>();
        guardBody = GetComponent<Rigidbody2D>();
        isAttacking = false;
        canBeNotified = true;
        killPigeon = false;
        ResetAttack();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer >= attackDuration)
        {
            if (attackStopDelayTimer < attackStopDelayDuration)
            {
                isAttackingPaused = true;
                attackStopDelayTimer += Time.deltaTime;
            }
            else ResetAttack();
        }
        if (guardDetect.isPigeonDetected) // yes see pigeon
        {
            isReadyToAttack = true;
            isAttackingPaused = false;
            attackStopDelayTimer = 0f;
            attackTimer = 0f;
        }
        else if (isAttacking) // no see pigeon, yes attacking--> increment attackTimer
        {
            attackTimer += Time.deltaTime;
        }
        if (isReadyToAttack) // no attacking, yes ready
        {
            if (attackStartDelayTimer >= attackStartDelayDuration)
            {
                if (indicatorFound != null) Destroy(indicatorFound);
                isAttacking = true;
            }
            else
            {
                if (indicatorFound == null)
                {
                    indicatorFound = (GameObject)Instantiate(Resources.Load("Prefabs/Indicator - Found"), transform.position + new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
                    indicatorFound.GetComponent<SpriteRenderer>().sortingOrder = 20;
                }
                attackStartDelayTimer += Time.deltaTime;
            }
            // Notify other guards if they are not attacking, can be notified, and are close enough
            foreach (GuardAttack guard in FindObjectsOfType<GuardAttack>())
            {
                if (guard.isReadyToAttack) continue;
                if (!guard.canBeNotified) continue;
                if (guard == GetComponent<GuardAttack>()) continue;
                if (Vector2.Distance(guard.transform.position, transform.position) <= notifyRadius)
                {
                    guard.isReadyToAttack = true;
                    guard.isAttackingPaused = false;
                    guard.attackStopDelayTimer = 0f;
                    guard.attackTimer = 0f;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.collider != pigeon.GetComponent<Collider2D>()) return;
        if (isAttacking) killPigeon = true;
        else if (!isReadyToAttack)
        {
            isReadyToAttack = true;
            isAttackingPaused = false;
            attackStopDelayTimer = 0f;
            attackTimer = 0f;
        }
    }

    void ResetAttack()
    {
        if (isAttacking) StopAttacking?.Invoke(this, EventArgs.Empty);
        attackStartDelayTimer = 0f;
        attackStopDelayTimer = 0f;
        attackStartDelayDuration = UnityEngine.Random.Range(attackStartDelayMin, attackStartDelayMax);
        attackTimer = 0f;
        isReadyToAttack = false;
        isAttacking = false;
        isAttackingPaused = false;
    }
}
