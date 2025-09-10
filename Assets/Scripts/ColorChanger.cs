using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private Material playerActiveMaterial;  // Active player color
    [SerializeField] private Material playerInactiveMaterial; // Inactive player color

    private MeshRenderer meshRenderer; // rendering the material
    private BasketballController basketballController; // accessing the parent(player)

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        basketballController = GetComponentInParent<BasketballController>();
        meshRenderer.material = playerInactiveMaterial;
    }

    // Chechking if the player is active(controlled by the user)
    private void Update()
    {
        if (basketballController.getPlayerIsActive()) 
            meshRenderer.material = playerActiveMaterial;
        else
            meshRenderer.material = playerInactiveMaterial;
    }

}
