using System.Collections.Generic;
using UnityEngine;

public class CameraTransparency : MonoBehaviour
{
    public Transform player;                         // Reference to the player
    public LayerMask wallMask;                       // Layer for walls
    public Material transparentMaterial;             // Assign in Inspector

    private Dictionary<Renderer, Material> originalMaterials = new();
    private List<Renderer> fadedRenderers = new();

    void Update()
    {
        ClearPreviousFades();                        // Restore materials from last frame

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        Ray ray = new Ray(transform.position, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, wallMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                if (!originalMaterials.ContainsKey(rend))
                {
                    originalMaterials[rend] = rend.material;
                    rend.material = transparentMaterial;
                }

                fadedRenderers.Add(rend);
            }
        }
    }

    private void ClearPreviousFades()
    {
        foreach (Renderer rend in fadedRenderers)
        {
            if (rend != null && originalMaterials.ContainsKey(rend))
            {
                rend.material = originalMaterials[rend];
            }
        }

        fadedRenderers.Clear();
        originalMaterials.Clear();
    }
}
