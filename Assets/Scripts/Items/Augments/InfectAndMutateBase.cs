using System.Collections.Generic;
using UnityEngine;

// Replaces itself with random augments of TargetRarity taken from the augment panel's pool
public abstract class InfectAndMutateBase : MonoBehaviour
{
    private const string TitlePrefix = "Infect and Mutate: ";

    protected abstract AugmentRarity TargetRarity { get; }
    protected virtual int AugmentCount => 1;

    private void Start()
    {
        Transform player = this.transform.root;
        Transform playerAugments = player.Find("PlayerAugments");
        // Inactive included: the panel closes shortly after the pick, a slow chain can start after that
        AugmentPanel augmentPanel = FindAnyObjectByType<AugmentPanel>(FindObjectsInactive.Include);

        GiveRandomAugments(augmentPanel, playerAugments);
        Destroy(this.gameObject);
    }

    private void GiveRandomAugments(AugmentPanel augmentPanel, Transform playerAugments)
    {
        List<GameObject> candidates = GetCandidates(augmentPanel);

        // A pool with fewer augments than AugmentCount simply gives what is left
        int amount = Mathf.Min(AugmentCount, candidates.Count);
        for (int i = 0; i < amount; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            GameObject augmentPrefab = candidates[randomIndex];
            candidates.RemoveAt(randomIndex);

            // The player owns it now, so the augment panel must not offer it again
            augmentPanel.AugmentItems.Remove(augmentPrefab);
            GameObject newAugment = Instantiate(augmentPrefab, playerAugments);
            // Same as in AugmentPanel: sit exactly on the player, whatever offset the prefab has
            newAugment.transform.localPosition = Vector3.zero;

            AugmentInformation augmentInformation = newAugment.GetComponent<AugmentInformation>();
            augmentInformation.augmentTitle = TitlePrefix + augmentInformation.augmentTitle;
        }
    }

    private List<GameObject> GetCandidates(AugmentPanel augmentPanel)
    {
        List<GameObject> candidates = new List<GameObject>();
        foreach (GameObject augment in augmentPanel.AugmentItems)
        {
            if (augment.GetComponent<AugmentInformation>().augmentRarity == TargetRarity)
            {
                candidates.Add(augment);
            }
        }
        return candidates;
    }
}
