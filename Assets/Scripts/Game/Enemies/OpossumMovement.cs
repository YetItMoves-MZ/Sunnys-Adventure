using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

// Opossum moves left to right. When he gets to an edge or wall, he will rotate himself and move the other way.
public class OpossumMovement : EnemyMovement
{
    public SpriteRenderer sr;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // needsRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.velocity = new Vector2(monsterStat.speed * direction, rb.velocity.y);
    }

    private void HandleRotation()
    {
        Vector2 currentPosition = transform.position;
        Vector2 spriteSize = sr.sprite.bounds.size;
        Vector2 sidePosition = new Vector2(currentPosition.x + (0.05f + spriteSize.x) / 2 * direction, currentPosition.y);
        Vector2 downPosition = new Vector2(currentPosition.x + (0.01f + spriteSize.x) / 2 * direction, currentPosition.y - spriteSize.y / 2);


        RaycastHit2D side = Physics2D.Raycast(sidePosition, Vector2.right * direction, 0.05f);
        RaycastHit2D down = Physics2D.Raycast(downPosition, Vector2.down, 0.1f);

        if (side.collider != null && side.collider.gameObject.tag != "Player" && side.collider.gameObject != gameObject || down.collider == null)
        {
            transform.localScale = new Vector2(defaultXScale * direction, transform.localScale.y);
            direction *= -1;
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Vector2 currentPosition = transform.position;
    //     Vector2 spriteSize = GetComponent<SpriteRenderer>().sprite.bounds.size;
    //     Vector2 sidePosition = new Vector2(currentPosition.x + (0.05f + spriteSize.x) / 2 * direction, currentPosition.y);
    //     Vector2 downPosition = new Vector2(currentPosition.x + (0.01f + spriteSize.x) / 2 * direction, currentPosition.y - spriteSize.y / 2);

    //     Gizmos.color = Color.red;
    //     Vector2 endPoint = sidePosition + ((Vector2.right * direction).normalized * 0.05f);
    //     Gizmos.DrawLine(sidePosition, endPoint);
    //     endPoint = downPosition + ((Vector2.down).normalized * 0.1f);
    //     Gizmos.DrawLine(downPosition, endPoint);
    // }

}
