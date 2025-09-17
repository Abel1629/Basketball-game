using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Constants
    private const float runSpeed = 2f; // running multiplyer
    private const float dunkDuration = 0.2f; // duration of the dunk

    [Header("Variables")]
    [SerializeField] private float moveSpeed = 10f; // speed of movement (DO NOT CHANGE)
    [SerializeField] private float jumpHeight = 10f; // height of jump (DO NOT CHANGE)
                                                     
    private Vector3 direction; // direction where the player is moving
    private float sprintLevel = 4f; // the remaining sprint amount
    private float exhaustLevel = 2f;
    private float TDunk = 0f; // dunk position interpolation variable
    
    Vector3 DunkPos; // the position from where the player will dunk
    int shotType; // 1- dunk, 2- two pointer, 3- three pointer

    [Header("Referrences")]

    // References
    [SerializeField] private Transform Ball;
    [SerializeField] private Transform Arms;
    [SerializeField] private Transform RightHand;
    [SerializeField] private Transform PosOverHead;
    [SerializeField] private Transform PosDribble;
    [SerializeField] private Transform Target;
    [SerializeField] private GameObject ShotClockTimer;
    [SerializeField] private Transform[] DunkTarget = new Transform[3];
    [SerializeField] private GameObject PosessionManager;

    // Components
    private Rigidbody playerRigidbody;
    private BallController ballController;
    private TimerController timerController;
    private TeamStats teamStats;
    private PosessionChanger posessionChanger;

    // Booleans
    private bool isInAir = false; // when the player is in the air
    private bool isSpaceReleasedAfterJump = false; // when the space is released after jumping with the ball
    private bool isSprinting = false; // sprinting boolean
    private bool isJumping = false; // jumping boolean
    private bool isDunkPositionSet = false; // setting the position from where the player will dunk
    private bool isPreparedToDunk = false; // when I press the space the second time in the dunking zone
    private bool isActive = false; // when the player is active(is controlled by the user)
    private bool isExhausted = false; // when using all the stamina it will take some time for it to recharge
    
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        ballController = Ball.GetComponent<BallController>(); // referring to the basketball
        timerController = ShotClockTimer.GetComponent<TimerController>();
        isActive = true;
        teamStats = GetComponentInParent<TeamStats>();
        posessionChanger = PosessionManager.GetComponent<PosessionChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        // Changing direction of the player based on input
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        Sprint(); // Sprinting

        // Ball in hands
        if (ballController.getIsBallInHands(gameObject.name))
        {
            ShootingMechanism(); // implementing the shooting mechanism
        }

        // If the ball is not in the hand
        else
        {
            // Putting the hands in normal position
            HandsDown();

            // Jumping without the ball
            if (Input.GetKey(KeyCode.Space) && !isInAir)
            {
                HandsUp();
                isInAir = true;
                isJumping = true;
            }
        }

        // Ball flying towards basket
        if (ballController.getIsBallFlying())
        {
            ballController.BallTrajectory(PosOverHead.position, playerRigidbody, shotType); // shooting the ball towards the basket
        }

        if (isPreparedToDunk)
        {
            Dunking();
        }

        if (timerController.GetShotclockTimer() <= 0 && ballController.getIsBallInHands(gameObject.name)) // if the shot clock reaches 0 and the player still has the ball in his hands++
        {
            DropBall();
            // AFTER IMPLEMENTING THE TWO TEAMS
            //posessionChanger.ChangePosession(); // giving the posession for the other team
        }
    }

    // All the input movements are controlled here
    void FixedUpdate()
    {
        // Compute movement speed
        float currentSpeed = isSprinting ? moveSpeed * runSpeed : moveSpeed;

        // Set velocity based on input
        Vector3 moveVelocity = direction * currentSpeed;

        // Keep existing Y velocity (important for gravity & jumping)
        moveVelocity.y = playerRigidbody.linearVelocity.y;

        // Apply the velocity
        playerRigidbody.linearVelocity = moveVelocity;

        // Jumping
        if (isJumping) 
        {
            Jump();
        }

        // Looking at the rim
        if (ballController.getIsBallInHands(gameObject.name))
        {
            LookAtTarget(Target);
        }

        // looking at the ball
        else
        {
            LookAtTarget(Ball);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // When the player is on the ground
        if (collision.gameObject.name == "Ground")
        {
            isInAir = false; // the player is on the ground
        }

        if (collision.gameObject.CompareTag("TwoPointer"))
            SetShootingForm("TwoPointer");

        if (collision.gameObject.CompareTag("ThreePointer"))
            SetShootingForm("ThreePointer");

        if (collision.gameObject.CompareTag("Dunk"))
            SetShootingForm("Dunk");

        // When catching the ball
        if (!ballController.getIsBallFlying() && !ballController.getIsBallInHands(gameObject.name) && collision.gameObject.name == "Ball")
        {
            if (!teamStats.GetIsTeamPosessionBlocked()) // the player cacthes the ball only if it's posession is not blocked
            {
                ballController.setIsballInHands(true, gameObject.name);
                ballController.setIsBallInAir(false);
            }
        }
    }

    private void SetShootingForm(string type)
    {
        if (type.Equals("TwoPointer"))
        {
            shotType = 2;
        }

        if (type.Equals("ThreePointer"))
        {
            shotType = 3;
        }

        if (type.Equals("Dunk"))
        {
            shotType = 1;
        }
    }

    // When second space is hit with the ball in the hand
    private void HandsShooting()
    {
        // Raise hands and move ball over head
        RightHand.localEulerAngles = Vector3.left * 0;
        Ball.position = PosOverHead.position;
        Arms.localEulerAngles = Vector3.left * 130;
    }

    // When first space is hit with the ball in the hand
    private void HandsPreparingForShooting()
    {
        // Raise hands and move ball over head
        RightHand.localEulerAngles = Vector3.left * 0;
        Ball.position = PosOverHead.position;
        Arms.localEulerAngles = Vector3.left * 160;
    }

    // Normal hand position
    private void HandsDown()
    {
        // Putting hands in normal position
        if (!ballController.getIsBallInHands(gameObject.name) && !isInAir && !ballController.getIsBallFlying())
        {
            RightHand.localEulerAngles = Vector3.left * 0;
            Arms.localEulerAngles = Vector3.left * 0;
        }
    }

    // Dribbling hand position
    private void HandsDribble()
    {
        // Dribble animation
        Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
        Arms.localEulerAngles = Vector3.left * 0;

        // Rotate the right hand in front of the player
        RightHand.localEulerAngles = Vector3.left * 20;
    }

    // Jumping without the ball
    private void HandsUp()
    {
        // Raise hands up vertically
        RightHand.localEulerAngles = Vector3.left * 0;
        Arms.localEulerAngles = Vector3.left * 180;
    }

    private void HandsDunking()
    {
        // Move the hands to a horizontal position
        RightHand.localEulerAngles = Vector3.left * 0;
        Arms.localEulerAngles = Vector3.left * 90;
    }

    private void Jump()
    {
        isInAir = true;
        playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);

        // Preventing double jumping
        isJumping = false;
    } // Jumping mechanism

    private void Shoot()
    {
        if (shotType != 1) // not a dunk
        {
            HandsShooting();
            ThrowingBall();
        }

        else // dunk
        {
            TDunk = 0; // To start the dunking
            isPreparedToDunk = true;
        }
    }

    private void Sprint() // Sprinting mechanism
    {
        // Changing direction of the player based on input
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (!Input.GetKey(KeyCode.Space))
        {
            // Sprinting
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && sprintLevel > 0)
            {
                isSprinting = true;
                sprintLevel -= Time.deltaTime; // decreasing the sprint level
                sprintLevel = Mathf.Max(sprintLevel, 0);
            }
            // Walking
            else
            {
                isSprinting = false;

                if (!isExhausted)
                {
                    sprintLevel += Time.deltaTime; // increasing the sprint level
                    sprintLevel = Mathf.Min(sprintLevel, 4);
                }
            }
        }

        if (sprintLevel <= 0)
        {
            isExhausted = true;
        }

        if (isExhausted)
        {
            exhaustLevel -= Time.deltaTime;

            if (exhaustLevel <= 0)
            {
                isExhausted = false;
                exhaustLevel = 2f;
            }
        }
    }
    
    // Setting the different variables to throw the ball
    private void ThrowingBall()
    {
        isSpaceReleasedAfterJump = false;
        ballController.setIsballInHands(false, gameObject.name);
        ballController.setIsballFlying(true);
    }

    // Moving the player to the dunking position
    private void Dunking()
    {
        // Initializing the dunk point
        if (!isDunkPositionSet)
        {
            DunkPos = ClosestDunkPosition();
        }

        TDunk += Time.deltaTime;

        float t01 = TDunk / dunkDuration;

        // Move to dunk target
        Vector3 A = playerRigidbody.position;
        Vector3 B = DunkPos;
        Vector3 pos = Vector3.Lerp(A, B, t01);

        // Move linearly the player to the dunk point
        playerRigidbody.position = pos;

        if (t01 >= 1)
        {
            isPreparedToDunk = false;
            isDunkPositionSet = false;

            HandsDunking();

            ThrowingBall();
        }
    }

    // Calculating the closest Dunking target out of the three target points
    private Vector3 ClosestDunkPosition()
    {
        float minDistance = Vector3.Distance(DunkTarget[0].position, playerRigidbody.position);
        Vector3 closestTarget = DunkTarget[0].position;
        
        for (int i = 1; i <=2; i++)
        {
            float currentDistance = Vector3.Distance(DunkTarget[i].position, playerRigidbody.position);

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestTarget = DunkTarget[i].position;
            }
        }

        isDunkPositionSet = true;

        return closestTarget;
    }

    private void LookAtTarget(Transform ObjectToLookAt)
    {
        // Look at the given object
        Vector3 lookingDirection = ObjectToLookAt.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(lookingDirection);
        playerRigidbody.MoveRotation(Quaternion.Euler(0, targetRotation.eulerAngles.y, 0));
    }

    // The player is shooting the ball based on this mechanism
    private void ShootingMechanism()
    {
        // First Space Press -> Jump (only if grounded)
        if (Input.GetKeyDown(KeyCode.Space) && !isInAir)
        {
            isInAir = true;
            isJumping = true;
        }

        // It sticks the ball in the hands of the player while jumping and looks at the rim
        if (isInAir)
        {
            // Raise the hands to prepare for shooting the ball
            HandsPreparingForShooting();
            LookAtTarget(Target);
        }

        // Detect that space was released after jumping
        if (isInAir && Input.GetKeyUp(KeyCode.Space))
        {
            isSpaceReleasedAfterJump = true;
        }

        // Second Space Press -> Throw
        if (isInAir && isSpaceReleasedAfterJump && Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        // If ball not thrown before landing(it will be automatically dropped)
        if (!isInAir && isSpaceReleasedAfterJump)
        {
            DropBall();
        }

        // Dribling when ball in hands and not shooting
        if (!isInAir && ballController.getIsBallInHands(gameObject.name))
        {
            isJumping = false;

            HandsDribble();
        }
    }

    private void DropBall()
    {
        ballController.setIsballInHands(false, gameObject.name);
        teamStats.SetTeamPosessionBlocked();
        ballController.setIsBallInAir(true);
        isSpaceReleasedAfterJump = false;
    }

    public bool getPlayerIsActive()
    {
        return isActive;
    }

    public bool getIsExhausted()
    {
        return isExhausted;
    }

    public float getSprintLevel()
    {
        return sprintLevel;
    }
}
