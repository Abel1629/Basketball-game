using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Constants
    private const float runSpeed = 2f; // running multiplyer
    private const float dunkDuration = 0.2f; // duration of the dunk

    // Variables
    [Header("Variables")]
    [SerializeField] private float moveSpeed = 10f; // speed of movement (DO NOT CHANGE)
    [SerializeField] private float jumpHeight = 10f; // height of jump (DO NOT CHANGE)
                                                    
    private float sprintLevel = 4f; // the remaining sprint amount
    private float exhaustLevel = 2f;
    private float TDunk = 0f; // dunk position interpolation variable

    private Vector3 direction; // direction where the player is moving
    private Vector3 dunkPos; // the position from where the player will dunk
    private Vector3 moveVelocity; // Set velocity based on input
    private int shotType; // 1- dunk, 2- two pointer, 3- three pointer

    [Header("Referrences")]

    // References
    [SerializeField] private Transform Ball;
    [SerializeField] private Transform PosOverHead;
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
    private PlayerAnimationController playerAnimationController;

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
        playerAnimationController = GetComponent<PlayerAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer(); // Sprinting/ walking

        // Ball in hands
        if (ballController.getIsBallInHands(gameObject.name))
        {
            ShootingMechanism(); // implementing the shooting mechanism
        }

        // If the ball is not in the hand
        else
        {
            // Jumping without the ball
            if (Input.GetKey(KeyCode.Space) && !isInAir)
            {
                playerAnimationController.JumpAnimation();

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
            // posessionChanger.ChangePosession(); // giving the posession for the other team
        }
    }

    // All the input movements are controlled here
    void FixedUpdate()
    {
        // Compute movement speed
        float currentSpeed = isSprinting ? moveSpeed * runSpeed : moveSpeed;

        // Set velocity based on input
        moveVelocity = direction * currentSpeed;

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
            playerAnimationController.HandsDownAnimation();
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

    // Jumping mechanism
    private void Jump()
    {
        isInAir = true;
        playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);

        // Preventing double jumping
        isJumping = false;
    }

    private void Shoot()
    {
        if (shotType != 1) // not a dunk
        {
            // moving the hand to the shooting position
            Ball.position = PosOverHead.position;
            playerAnimationController.HandsShootingAnimation();

            ThrowingBall();
        }

        else // dunk
        {
            TDunk = 0; // To start the dunking
            isPreparedToDunk = true;
        }
    }

    private void MovePlayer() // Moving mechanism
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
            dunkPos = ClosestDunkPosition();
        }

        TDunk += Time.deltaTime;

        float t01 = TDunk / dunkDuration;

        // Move to dunk target
        Vector3 A = playerRigidbody.position;
        Vector3 B = dunkPos;
        Vector3 pos = Vector3.Lerp(A, B, t01);

        // Move linearly the player to the dunk point
        playerRigidbody.position = pos;

        if (t01 >= 1)
        {
            isPreparedToDunk = false;
            isDunkPositionSet = false;

            // moving the hand in the dunking position
            playerAnimationController.HandsDunkingAnimation();

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
            Ball.position = PosOverHead.position;
            playerAnimationController.HandsPreparingForShootingAnimation();
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
            // the dribling is implemented in the player animation controller
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

    public float GetMoveVelocityMagnitude()
    {
        return Mathf.Round(moveVelocity.magnitude);
    }
}
