using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 3f;
    public bool isHorizontal = true;
    public float movementRange;
    public bool startInverted = false;

    [HideInInspector]
    public int isInverted;
    float movedAmount;
    // Start is called before the first frame update
    void Start()
    {
        isInverted = startInverted ? -1 : 1;
        movedAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float amountToMove = speed * Time.deltaTime * isInverted;
        if (isHorizontal)
            HorizontalMovement(amountToMove);
        else
            VerticalMovement(amountToMove);
        amountToMove = Math.Abs(amountToMove);
        movedAmount += amountToMove;
        if (movedAmount > movementRange)
        {
            isInverted *= -1;
            movedAmount *= -1;
        }
    }

    private void VerticalMovement(float amountToMove)
    {
        transform.Translate(0f, amountToMove, 0f);
    }

    private void HorizontalMovement(float amountToMove)
    {
        transform.Translate(amountToMove, 0f, 0f);
    }
}
