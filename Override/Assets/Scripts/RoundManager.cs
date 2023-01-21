using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [SerializeField] int currentRound;
    [SerializeField] int robotsThisRound;
    public int robotsLeft;
    [SerializeField] int roundChangeTime;
    bool roundChanging;
    public bool instantKillActive;
    TextMeshProUGUI roundText;

    void Start()
    {
        currentRound = 1;
        robotsThisRound = 10;
        robotsLeft = robotsThisRound;
        roundText = GameObject.FindWithTag("RoundCounter").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if(robotsLeft == 0 && !roundChanging && !GameObject.FindWithTag("Robot"))
        {
            StartCoroutine(RoundChange());
        }

        roundText.text = "Round: " + currentRound;
    }

    public void StartInstantKill()
    {
        StartCoroutine(StartInstantKillRoutine());
    }

    IEnumerator StartInstantKillRoutine()
    {
        instantKillActive = true;
        yield return new WaitForSeconds(30);
        instantKillActive = false;
    }

    IEnumerator RoundChange()
    {
        roundChanging = true;
        yield return new WaitForSeconds(roundChangeTime);
        currentRound += 1;
        robotsThisRound += 10;
        robotsLeft = robotsThisRound;
        roundChanging = false;
    }
}
