using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuyableDoor : MonoBehaviour
{
    [SerializeField] int doorCost = 500;
    TextMeshProUGUI promptText;
    bool isInCollider;
    StatTracker statTracker;

    void Start()
    {
        promptText = GameObject.FindWithTag("PromptText").GetComponent<TextMeshProUGUI>();
        statTracker = GameObject.FindWithTag("StatTracker").GetComponent<StatTracker>();
        promptText.text = "";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInCollider = true;
            promptText.text = "Press E to buy door for " + doorCost + " points";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInCollider = false;
            promptText.text = "";
        }
    }

    void Update()
    {
        if(isInCollider && statTracker.playerPoints >= doorCost && Input.GetKeyDown(KeyCode.E))
        {
            statTracker.playerPoints -= doorCost;
            Destroy(gameObject);
        }
    }
}
