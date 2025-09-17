using UnityEngine;

public class LockYPosition : MonoBehaviour
{
    [SerializeField] private float fixedY = 0.5f; // Locking the object to a fixed Y position globally

    void LateUpdate() // It is called after Update
    {
        Vector3 pos = transform.position;
        pos.y = fixedY; // force Y to always stay at fixedY
        transform.position = pos;
    }
}
