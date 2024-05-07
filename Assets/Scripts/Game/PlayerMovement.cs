using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Properties")]
    public float speed;

    [Header("Jumping Properties")]
    public float jumpHeight;
    public float maxJumpTime;

    [Header("Sound Effects")]
    public AudioSource SoundEffectAudioSource;
    public AudioClip jumpSound;
    public AudioClip deathSound;
    public AudioClip finishLevelSound;
    public AudioSource GameMusicAudioSource;

    float defaultXScale;
    float currentJumpTime;

    bool canJump;
    bool isFullJumpFinished;
    bool CanClimb;
    bool recentlyKilledEnemy;

    [HideInInspector]
    public bool playerDied;

    Rigidbody2D rb;
    Animator anim;
    BoxCollider2D cldr;

    // Start is called before the first frame update
    void Start()
    {
        if (!TryGetComponent(out Rigidbody2D rbComponent))
            Debug.LogError("missing Rigidbody2D");
        if (!TryGetComponent(out Animator animComponent))
            Debug.LogError("missing Animator");
        if (!TryGetComponent(out BoxCollider2D colliderComponent))
            Debug.LogError("missing BoxCollider2D");

        rb = rbComponent;
        anim = animComponent;
        cldr = colliderComponent;

        defaultXScale = transform.localScale.x;
        currentJumpTime = 0;
        canJump = true;
        isFullJumpFinished = false;
        CanClimb = false;
        playerDied = false;
        recentlyKilledEnemy = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleRageQuit();
        if (playerDied)
            return;
        HandleWhatIsBelowMe();
        HandleBeingCrushed();
        HandleJump();
        HandleClimb();
        HandleMovement();
    }

    private void HandleRageQuit()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void HandleWhatIsBelowMe()
    {
        Vector2 currentPosition = new Vector2(transform.position.x + cldr.offset.x, transform.position.y);
        float yPosition = currentPosition.y - cldr.size.y;
        Vector2[] positions = new Vector2[3];
        RaycastHit2D[] hits = new RaycastHit2D[3];
        Collider2D[] colliders = new Collider2D[3];
        float rbFactor = rb.velocity.y >= -0.1 ? 1 : rb.velocity.y * -1;

        for (int i = -1; i <= 1; i++)
        {
            positions[i + 1] = new Vector2(currentPosition.x + (cldr.size.x / 2 * i), yPosition);
            hits[i + 1] = Physics2D.Raycast(positions[i + 1], Vector2.down, 0.05f * rbFactor);
            colliders[i + 1] = hits[i + 1].collider;
        }

        HandleLowerCollision(colliders);

    }

    private void HandleLowerCollision(Collider2D[] others)
    {
        foreach (Collider2D other in others)
        {
            if (other != null && other.gameObject.tag == "Enemy")
            {
                if (other != null && other.TryGetComponent(out EnemyMovement enemyMovement))
                    HandleEnemyKill(enemyMovement);
                return;
            }
            else if (other != null && other.gameObject.tag == "Ground")
            {
                currentJumpTime = 0f;
                canJump = true;
                if (other != null && other.gameObject.name.Contains("MovingPlatform"))
                {
                    if (other.gameObject.TryGetComponent(out MovingPlatform movingPlatform))
                    {
                        MakePlayerMoveWithPlatform(movingPlatform);
                    }
                }
                return;
            }
        }
    }

    private void HandleEnemyKill(EnemyMovement enemyMovement)
    {
        recentlyKilledEnemy = true;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
        enemyMovement.OnDeath();
    }

    private void OnPlayerDeath()
    {
        if (playerDied)
            return;
        playerDied = true;
        SoundEffectAudioSource.clip = deathSound;
        GameMusicAudioSource.Stop();
        SoundEffectAudioSource.Play();
        anim.SetBool("IsDamaged", true);
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
        cldr.isTrigger = true;
        StartCoroutine("FinishGameOnDeath");
    }
    IEnumerator FinishGameOnDeath()
    {

        yield return new WaitForSeconds(1f);
        InGameUIEngine.needToCheckHighScore = true;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void HandleClimb()
    {
        if (!CanClimb)
            return;
        float direction = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(rb.velocity.x, direction * speed / 2);
        HandleClimbAnimation();
    }
    private void HandleClimbAnimation()
    {
        float direction = Input.GetAxisRaw("Vertical");
        if (anim.GetBool("StartedClimbAnimation"))
        {
            anim.speed = direction != 0 ? 1f : 0f;
        }
    }

#if UNITY_ANDROID
    private void HandleMovement()
    {

    }

    public void OnLeftEnter()
    {
        
    }
    public void OnLeftExit()
    {

    }
    public void OnRightEnter()
    {

    }
    public void OnRightExit()
    {

    }
#else
    private void HandleMovement()
    {
        float direction;
        bool isBothKeyPressed = IsLeftPressed() && IsRightPressed();
        direction = !isBothKeyPressed ? Input.GetAxis("Horizontal") : 0f;
        HandleMovementAnimations();
        if (!recentlyKilledEnemy)
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
        else
            recentlyKilledEnemy = false;
    }
#endif
    bool IsLeftPressed()
    {
        return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    }
    bool IsRightPressed()
    {
        return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    }

    private void HandleMovementAnimations()
    {
        float direction = Input.GetAxisRaw("Horizontal");
        anim.SetBool("IsRunning", direction != 0);
        if (direction == 0)
            return;
        float currentXScale = defaultXScale * direction;
        Vector3 currentScale = new Vector3(currentXScale, transform.localScale.y, transform.localScale.z);
        transform.localScale = currentScale;
    }

    private void HandleJump()
    {
        HandleJumpAnimations();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isFullJumpFinished = false;
            if (rb.velocity.y != 0f)
                canJump = false;
        }

        if (!canJump || !Input.GetKey(KeyCode.Space) || isFullJumpFinished)
            return;

        if (currentJumpTime > maxJumpTime)
        {
            isFullJumpFinished = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundEffectAudioSource.clip = jumpSound;
            SoundEffectAudioSource.Play();
        }
        currentJumpTime += Time.deltaTime;
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
    }
    private void HandleJumpAnimations()
    {
        anim.SetBool("IsJumping", rb.velocity.y > 0.1f);
        anim.SetBool("IsFalling", rb.velocity.y < -0.1f);
    }

    private void HandleBeingCrushed()
    {
        Vector2 circleStart = new Vector2(transform.position.x, transform.position.y - 0.05f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(circleStart, 0.01f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Ground"))
            {
                OnPlayerDeath();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (playerDied)
            return;

        if (other.gameObject.tag == "Enemy")
        {
            OnPlayerDeath();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerDied)
            return;

        if (other.gameObject.tag == "Ladder")
        {
            CanClimb = true;
            rb.gravityScale = 0f;
            anim.SetBool("IsClimbing", true);
            anim.SetTrigger("IsClimbMoving");
        }
        else if (other.gameObject.tag == "Trap")
        {
            OnPlayerDeath();
        }
        else if (other.gameObject.tag == "Sign")
        {
            if (other.TryGetComponent(out SignEngine signEngine))
            {
                InGameUIEngine.ReadSign(signEngine.signSO);
            }
        }
        else if (other.gameObject.tag == "Cherry")
        {
            InGameUIEngine.cherryAmount--;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Gem")
        {
            InGameUIEngine.gemAmount++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "BigGem")
        {
            InGameUIEngine.gemAmount += 10;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Finish")
        {
            if (InGameUIEngine.cherryAmount <= 0)
            {
                SoundEffectAudioSource.clip = finishLevelSound;
                GameMusicAudioSource.Stop();
                SoundEffectAudioSource.Play();
                InGameScoreManagment.levelsFinished++;
                InGameUIEngine.needToCheckHighScore = true;
                LevelManager.levelEnded = true;
                playerDied = true;
                rb.velocity = Vector2.zero;
                anim.SetBool("IsGameFinished", true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            CanClimb = false;
            rb.gravityScale = 1f;
            anim.SetBool("IsClimbing", false);
            anim.speed = 1f;
        }
        else if (other.gameObject.tag == "Sign")
        {
            InGameUIEngine.StopReadSign();
        }
    }

    private void MakePlayerMoveWithPlatform(MovingPlatform movingPlatform)
    {

        float newVelocity = movingPlatform.speed * movingPlatform.isInverted * Time.deltaTime;
        if (movingPlatform.isHorizontal)
            transform.Translate(newVelocity, 0f, 0f);
        else
            transform.Translate(0f, newVelocity, 0f);
    }

    private void OnDrawGizmos()
    {
        rb = GetComponent<Rigidbody2D>();
        cldr = GetComponent<BoxCollider2D>();
        Vector2 currentPosition = new Vector2(transform.position.x + cldr.offset.x, transform.position.y);
        float yPosition = currentPosition.y - cldr.size.y;

        Vector2 leftPosition = new Vector2(currentPosition.x - cldr.size.x / 2, yPosition);
        Vector2 rightPosition = new Vector2(currentPosition.x + cldr.size.x / 2, yPosition);
        Vector2 middlePosition = new Vector2(currentPosition.x, yPosition);

        RaycastHit2D hit1 = Physics2D.Raycast(leftPosition, Vector2.down, 0.05f);
        RaycastHit2D hit2 = Physics2D.Raycast(rightPosition, Vector2.down, 0.05f);
        RaycastHit2D hit3 = Physics2D.Raycast(middlePosition, Vector2.down, 0.07f);


        float rbFactor = rb.velocity.y >= -0.1 ? 1 : rb.velocity.y * -1;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(leftPosition, new Vector3(leftPosition.x, leftPosition.y + Vector2.down.y * 0.05f * rbFactor, 0f));
        Gizmos.DrawLine(rightPosition, new Vector3(rightPosition.x, rightPosition.y + Vector2.down.y * 0.05f * rbFactor, 0f));
        Gizmos.DrawLine(middlePosition, new Vector3(middlePosition.x, middlePosition.y + Vector2.down.y * 0.07f * rbFactor, 0f));
    }
}
