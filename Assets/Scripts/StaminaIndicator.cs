using UnityEngine;

public class StaminaIndicator : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material staminaMaterial;  // stamina material
    [SerializeField] private Material exhaustMaterial; // exhaust material
    [SerializeField] private Material invisibleMaterial; // invisible material

    private MeshRenderer[] meshRenderer; // rendering the materials
    private MeshRenderer[] childMeshes; // only the child meshes are stored
    private BasketballController basketballController; // accessing the parent(player)

    private void Start()
    {
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        basketballController = GetComponentInParent<BasketballController>();
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            meshRenderer[i].material = staminaMaterial; // starting with full stamina bar
        }

        // storing only the meshes of the child objects
        childMeshes = new MeshRenderer[meshRenderer.Length - 1];

        int index = 0;
        foreach (MeshRenderer renderer in meshRenderer) 
        { 
            if(renderer.gameObject != this.gameObject)
            {
                childMeshes[index] = renderer;
                index++;
            }
        }

    }

    private void FixedUpdate()
    {
        if (basketballController.getIsExhausted())
            for (int i = 0; i < childMeshes.Length; i++)
            {
                childMeshes[i].material = exhaustMaterial; // changing the stamina bar to exhausted
            }

        else
            for (int i = 0; i < childMeshes.Length; i++)
            {
                if (i >= basketballController.getSprintLevel() * 2) // when using stamina, the stamina bar will decrease
                {
                    childMeshes[i].material = invisibleMaterial;
                }

                else {
                    childMeshes[i].material = staminaMaterial; // only the stamina that is left is shown
                }
            }
    }
}
