using UnityEngine;
using System.Collections;

public class NPCBootstrapper : MonoBehaviour
{
    public GameObject[] npcObjects;
    public float delay = 0.1f; // small delay so physics settles

    IEnumerator Start()
    {
        // Disable all NPCs initially
        foreach (var npc in npcObjects)
            npc.SetActive(false);

        // Wait for 1 physics frame to avoid initial overlaps
        yield return new WaitForSeconds(delay);

        // Enable NPCs cleanly
        foreach (var npc in npcObjects)
            npc.SetActive(true);

        Debug.Log("NPCs activated after delay");
    }
}
