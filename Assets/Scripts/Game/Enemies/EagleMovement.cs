using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Eagle flies around searching for prey (Player). If the player is nearby,
// he will follow the player with his eyes (changing scale), 
// until he will get a target lock (position will be targeted then by the player position at that time),
// and after some time, he will then go to the locked position
public class EagleMovement : EnemyMovement
{

    public float preyRange;
    public float searchRange;
    GameObject Player;
    float currentTime;
    bool isSearching;
    bool reachedPosition;
    Vector2 nextDestination;

    float minSearchingXPosition;
    float maxSearchingXPosition;
    float minSearchingYPosition;
    float maxSearchingYPosition;

    AudioSource audioSource;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        currentTime = 0f;
        isSearching = true;
        Player = GameObject.Find("Player");

        minSearchingXPosition = transform.position.x - (searchRange / 2);
        maxSearchingXPosition = transform.position.x + (searchRange / 2);
        minSearchingYPosition = transform.position.y - (searchRange / 2);
        maxSearchingYPosition = transform.position.y + (searchRange / 2);

        nextDestination = GetRandomDestination();
    }

    // Update is called once per frame
    void Update()
    {
        TryToFindPrey();
        GoToTarget(isSearching);

    }

    private void TryToFindPrey()
    {
        if (!isSearching)
            return;
        if (Vector2.Distance(Player.transform.position, transform.position) < preyRange)
        {
            // prey found
            isSearching = false;
            currentTime = 0f;
        }
    }

    private void GoToTarget(bool isSearching)
    {
        if (reachedPosition)
        {
            currentTime = 0f;
            nextDestination = isSearching ? GetRandomDestination() : GetPlayerPosition();
        }
        if (currentTime > monsterStat.windUpTime)
        {
            Vector2 currentPosition = transform.position;
            Vector2 speeding = (nextDestination - currentPosition).normalized;
            transform.Translate(speeding.x * monsterStat.speed * Time.deltaTime, speeding.y * monsterStat.speed * Time.deltaTime, 0f);
            if (Vector2.Distance(currentPosition, nextDestination) < 0.05f)
            {
                reachedPosition = true;
            }
        }
        currentTime += Time.deltaTime;
    }

    private Vector2 GetPlayerPosition()
    {
        audioSource.Play();
        reachedPosition = false;
        CheckRotation(Player.transform.position);
        return Player.transform.position;
    }

    private Vector2 GetRandomDestination()
    {
        reachedPosition = false;
        float x = Random.Range(minSearchingXPosition, maxSearchingXPosition);
        float y = Random.Range(minSearchingYPosition, maxSearchingYPosition);
        Vector2 destination = new Vector2(x, y);
        CheckRotation(destination);
        return new Vector2(x, y);
    }

    private void CheckRotation(Vector2 destination)
    {
        direction = destination.x > transform.position.x ? -1 : 1;
        transform.localScale = new Vector3(defaultXScale * direction, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmos()
    {
        // Prey searching range
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawCube(transform.position, new Vector2(searchRange, searchRange));
    }


}
