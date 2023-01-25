using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoStation : MonoBehaviour
{
    TextMeshProUGUI promptText;
    bool isInTrigger;
    [SerializeField] int perkCost = 1500;
    bool hasDisplayedText;
    StatTracker statTracker;
    Player player;

    void Start()
    {
        promptText = GameObject.FindWithTag("PromptText").GetComponent<TextMeshProUGUI>();
        statTracker = GameObject.FindWithTag("StatTracker").GetComponent<StatTracker>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
        }
    }

    void Update()
    {
        if (isInTrigger == true)
        {
            hasDisplayedText = true;
            promptText.text = "Press F to buy Ammo for " + perkCost + " points";
            if (Input.GetKeyDown(KeyCode.F) && statTracker.playerPoints >= perkCost)
            {
                player.ammoStockpile += 50;
                statTracker.playerPoints -= perkCost;
            }
        }

        if (isInTrigger == false && hasDisplayedText == true)
        {
            hasDisplayedText = false;
            promptText.text = "";
        }
    }
}
