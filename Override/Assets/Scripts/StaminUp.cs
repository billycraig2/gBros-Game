using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StaminUp : MonoBehaviour
{
    TextMeshProUGUI promptText;
    bool isInTrigger;
    [SerializeField] int perkCost = 2500;
    bool hasDisplayedText;
    StatTracker statTracker;
    Player player;
    AudioSource drinkSound;

    void Start()
    {
        drinkSound = GetComponent<AudioSource>();
        promptText = GameObject.FindWithTag("PromptText").GetComponent<TextMeshProUGUI>();
        statTracker = GameObject.FindWithTag("StatTracker").GetComponent<StatTracker>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !player.hasStamina)
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
            promptText.text = "Press F to buy Speed Bonus for " + perkCost + " points";
            if (Input.GetKeyDown(KeyCode.F) && statTracker.playerPoints >= perkCost)
            {
                drinkSound.Play();
                player.hasStamina = true;
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
