using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public MonsterStatsSO monsterStat;
    public Rigidbody2D rb;
    public GameObject puffOfSmokePrefab;

    protected bool isDead = false;
    protected int direction;
    protected float defaultXScale;

    protected virtual void Start()
    {
        defaultXScale = transform.localScale.x;
        direction = -1;
    }

    public void OnDeath()
    {
        if (isDead)
            return;
        isDead = true;

        Instantiate(puffOfSmokePrefab, transform.position, puffOfSmokePrefab.transform.rotation);
        Destroy(gameObject);
    }
}
