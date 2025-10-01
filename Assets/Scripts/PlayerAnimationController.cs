using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    // References
    [SerializeField] private Transform Ball;
    [SerializeField] private Transform PosDribble;

    // Components
    private Animator animator;
    private PlayerController playerController;
    private BallController ballController;

    // Variables
    private bool isBallInHands = false; // checks if the ball is in the hand
    private float moveVelocity; // gets the move velocity from the player controller and based on that it creates the animations
    private bool isTriggerSet = false; // checks if the trigger for preparing the shooting is set

    private void Start()
    {
        animator = GetComponent<Animator>(); // getting the animator component
        playerController = GetComponent<PlayerController>(); // getting the player controller component
        ballController = Ball.GetComponent<BallController>(); // getting the ball controller component
    }

    private void Update()
    {
        isBallInHands = ballController.getIsBallInHands(gameObject.name); // getting if the ball is in the hand of the player
        moveVelocity = playerController.GetMoveVelocityMagnitude(); // getting the velocity of the player

        if (!isBallInHands) // ball not in hands and player on the ground
        {
            animator.SetBool("IsBallInHands", isBallInHands);

            SetSpeed();
        }

        else if (isBallInHands && !isTriggerSet)
        {
            animator.SetBool("IsBallInHands", isBallInHands);

            SetSpeed();

            // Ball dribble animation
            float min = -0.8f;
            float max = 1f;

            float bounce = Mathf.Abs(Mathf.Sin(Time.time * 2 * Mathf.PI)); // height between [0,1], it bounces two times in one second
            float mappedBounce = min + (max - min) * bounce;      // height remaped to [-2,1]
            Ball.position = PosDribble.position + Vector3.up * mappedBounce; // changing the position of the ball
        }
    }

    private void SetSpeed()
    {
        switch (moveVelocity)
        {
            case 0: // idle
                animator.SetFloat("Speed", 0);
                break;

            case 10: // moving
                animator.SetFloat("Speed", 10);
                break;

            case 20: // sprinting
                animator.SetFloat("Speed", 20);
                break;
        }
    }

    public void JumpAnimation()
    {
        animator.SetTrigger("Jump");
    }

    public void HandsDownAnimation()
    {
        animator.SetTrigger("HandsDown");
    }

    public void HandsPreparingForShootingAnimation()
    {
        if (!isTriggerSet)
        {
            animator.SetTrigger("HandsPreparingForShooting");
            isTriggerSet = true;
        }
    }

    public void HandsShootingAnimation()
    {
        animator.SetTrigger("HandsShooting");

        isTriggerSet = false;
    }

    public void HandsDunkingAnimation()
    {
        animator.SetTrigger("HandsDunking");

        isTriggerSet = false;
    }
}
