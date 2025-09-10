using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [SerializeField] private float customGravity = -30f;
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
