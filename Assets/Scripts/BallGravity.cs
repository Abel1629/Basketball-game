using UnityEngine;

public class BallGravity : MonoBehaviour
{
    [SerializeField] private float customGravity = -0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector3(0, customGravity, 0), ForceMode.Acceleration); // adding custom gravity pull to the ball
    }
}
