using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// gain one random Blossom Augment
public class InfectAndMutate_Blossom : MonoBehaviour
{
    private GameObject AugmentPanel;
    private GameObject GameManager;
    private Transform Player;
    private Transform PlayerAugments;
    private AugmentPanel augmentPanel;
    private List<GameObject> BlossomAugments;

    private void Start()
    {
        AugmentPanel = GameObject.Find("AugmentPanel");
        Player = this.transform.root;
        augmentPanel = AugmentPanel.GetComponent<AugmentPanel>();
        PlayerAugments = Player.transform.Find("PlayerAugments");
        GiveRandomBlossomAugment();
    }
    
    private void GiveRandomBlossomAugment()
    {
        GetBlossomAugments();
        AddRandomAugmentToPlayer();
    }

    private void GetBlossomAugments()
    {
        BlossomAugments = new List<GameObject>();
        List<GameObject> AugmentItems = augmentPanel.AugmentItems;
        foreach (GameObject augment in AugmentItems)
        {
            if (augment.GetComponent<AugmentInformation>().augmentRarity == 2)
            {
                BlossomAugments.Add(augment);
            }
        }
    }

    private void AddRandomAugmentToPlayer()
    {
        int randomIndex = Random.Range(0, BlossomAugments.Count);
        GameObject NewAugment = Instantiate(BlossomAugments[randomIndex]);
        NewAugment.transform.SetParent(PlayerAugments.transform, false);
    }
}
