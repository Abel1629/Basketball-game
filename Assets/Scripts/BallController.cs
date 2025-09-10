using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BallController : MonoBehaviour
{
    // Constants
    private const float OFFSETY = 3.0f;
    private const float MINHEIGHT = 9.0f;

    // References
    [SerializeField] private Transform Target;

    // Booleans
    private bool isBallFlying = false; // when the ball is flying towards the basket
    private List<bool> isBallInHands = new List<bool>() { false, false, false, false, false, false }; // when the ball is in the hands of the player
    private bool isTargetSet = false; // setting the target where the ball will fly

    // Variables
    private float T = 0f; // interpolation variable
    private float duration = 0.5f; // duration of the shot
    Vector3 OffsetTarget; // offsetting the original target
    private Vector3 previousBallPosition; // for calculating the velocity of the ball

    // Setting the ball to a player
    public void setIsballInHands(bool value, string playerName)
    {
        // When setting the ball to a player, we make sure the ball is not set to another player also
        for (int i = 0; i < isBallInHands.Count; i++)
            isBallInHands[i] = false;

        switch (playerName)
        {
            case "Player1":
                isBallInHands[0] = value;
                break;

            case "Player2":
                isBallInHands[1] = value;
                break;

            case "Player3":
                isBallInHands[2] = value;
                break;

            case "Player4":
                isBallInHands[3] = value;
                break;

            case "Player5":
                isBallInHands[4] = value;
                break;

            case "Player6":
                isBallInHands[5] = value;
                break;
        }
    }

    // Checking if the ball is in the hands of the player with the given number
    public bool getIsBallInHands(string playerName)
    {
        switch (playerName)
        {
            case "Player1":
                return isBallInHands[0];

            case "Player2":
                return isBallInHands[1];

            case "Player3":
                return isBallInHands[2];

            case "Player4":
                return isBallInHands[3];

            case "Player5":
                return isBallInHands[4];

            case "Player6":
                return isBallInHands[5];

            default:
                return false;
        }
    }

    public void setIsballFlying(bool value)
    {
        isBallFlying = value;
    }
    
    public bool getIsBallFlying()
    {
        return isBallFlying;
    }


    // Calculating the offset to where the shot ball will land
    private void TargetOffsetCalulation(Rigidbody playerRigidbody, int shotType)
    {
        Vector3 offset = new Vector3(0f, 0f, 0f); // Offsetting the original target
        float distance = Vector3.Distance(playerRigidbody.position, Target.position); // Distance between player and rim
        float height = transform.position.y; // Height of launch (9 - 15)


        // Y coordinate offset
        offset.y += OFFSETY - ((height - MINHEIGHT) / 2f); // y offset between 0 and 3

        if (shotType == 2) // two pointer
        {
            // X coordinate offset
            offset.x += (playerRigidbody.linearVelocity.x / 10f); // x offset between -2 and 2

            // Z coordinate offset
            if (playerRigidbody.linearVelocity.z > 0f) // z offset between -3 and 2
                offset.z = playerRigidbody.linearVelocity.z / 10f;

            else if (playerRigidbody.linearVelocity.z == 0f)
                offset.z = 0f;

            else if (playerRigidbody.linearVelocity.z < 0f)
                offset.z = playerRigidbody.linearVelocity.z / 7f;

            offset.z -= distance / 32f; // if the ball is thrown from further, the accuracy decreases

            duration = 0.5f;
        }

        else if (shotType == 3) // three pointer
        {
            // X coordinate offset
            offset.x += (playerRigidbody.linearVelocity.x / 7f); // x offset between -3 and 3

            // Z coordinate offset
            if (playerRigidbody.linearVelocity.z > 0f) // z offset between -4 and 3
                offset.z = playerRigidbody.linearVelocity.z / 7f;

            else if (playerRigidbody.linearVelocity.z == 0f)
                offset.z = 0f;

            else if (playerRigidbody.linearVelocity.z < 0f)
                offset.z = playerRigidbody.linearVelocity.z / 5f;

            // Decreasing accuracy by distance
            if (distance < 40f) // between 26 and 40 the offset is between 0 and 1
            {
                offset.z -= (distance - 26f) / 14f;
            }

            else if (distance < 45f) // between 40 and 45 the offset is between 1 and 2
            {
                offset.z -= (distance - 35f) / 5f;
            }

            else if (distance < 50f) // between 45 and 50 the offset is between 2 and 3
            {
                offset.z -= (distance - 35f) / 5f;
            }

            else // above 50 the offset is 4;
            {
                offset.z -= 4f;
            }

            duration = 0.5f;
        }

        else if (shotType == 1) // dunk
        {
            duration = 0.5f;
            offset.x = 0f;
            offset.y = 0f;
            offset.z = 0f;

            //if (height < 13.5f)
            //{
            //    offset.y = 0.5f;
            //    offset.z = -2f;
            //}

            //Debug.Log(height);
        }

        OffsetTarget += offset;
        isTargetSet = true;
    }

    // Shooting the ball to the rim
    public void BallTrajectory(Vector3 startingPosition, Rigidbody playerRigidbody, int shotType)
    {
        if (!isTargetSet)
        {
            T = 0;
            OffsetTarget = Target.position;
            TargetOffsetCalulation(playerRigidbody, shotType);
            previousBallPosition = transform.position; // initializing the velocity calculation
        }

        T += Time.deltaTime;

        float t01 = T / duration;

        // Move to target
        Vector3 A = startingPosition; // starting position
        Vector3 B = OffsetTarget;
        Vector3 pos = Vector3.Lerp(A, B, t01);

        // move linearly
        Vector3 finalBallPosition = pos;

        if (shotType != 1) // not dunk
        {
            // move in arc
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            finalBallPosition += arc;
        }

        transform.position = finalBallPosition;

        // Calculate velocity
        Vector3 velocity = (finalBallPosition - previousBallPosition) / Time.deltaTime;
        previousBallPosition = finalBallPosition;

        // Moment when the ball arrives at the target
        if (t01 >= 1)
        {
            setIsballFlying(false);
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;

            if (shotType != 1) // not dunk
                rb.linearVelocity = velocity;

            isTargetSet = false;
        }
    }
}
