using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Frog Jumps around to random locations and random heights.
public class FrogMovement : EnemyMovement
{
    public Animator anim;
    public float maxJumpHeight = 2f;
    public float minJumpHeight = 1f;
    public float maxJumpLenght = 1f;

    float currentTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (monsterStat.windUpTime < currentTime)
        {
            currentTime = 0f;
            Jump();
        }
        currentTime += Time.deltaTime;
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        anim.SetBool("IsFalling", rb.velocity.y < -0.01f);
        anim.SetBool("IsJumping", rb.velocity.y > 0.01f);
    }

    private void Jump()
    {
        float xForce = Random.Range(maxJumpLenght * -1, maxJumpLenght);
        float yForce = Random.Range(minJumpHeight, maxJumpHeight);

        rb.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);

        direction = xForce > 0 ? 1 : -1;
        transform.localScale = new Vector2(defaultXScale * direction, transform.localScale.y);
    }
}
