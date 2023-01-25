using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public int currentRound;
    [SerializeField] int robotsThisRound;
    public int robotsLeft;
    [SerializeField] int roundChangeTime;
    bool roundChanging;
    public bool instantKillActive;
    TextMeshProUGUI roundText;
    public int highestRound;  

    void Start()
    {
        highestRound = PlayerPrefs.GetInt("HighestRound");
        currentRound = 1;
        robotsThisRound = 10;
        robotsLeft = robotsThisRound;
        roundText = GameObject.FindWithTag("RoundCounter").GetComponent<TextMeshProUGUI>();
        roundText.text = "Round: " + currentRound;
    }

    void Update()
    {
        if(robotsLeft == 0 && !roundChanging && !GameObject.FindWithTag("Robot") && !GameObject.FindWithTag("RobotShooter"))
        {
            StartCoroutine(RoundChange());
        }       
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
        yield return new WaitForSeconds(1f);
        roundText.text = "";
        yield return new WaitForSeconds(1f);
        roundText.text = "Round: " + currentRound;
        yield return new WaitForSeconds(1f);
        roundText.text = "";
        yield return new WaitForSeconds(1f);
        roundText.text = "Round: " + currentRound;
        currentRound += 1;
        yield return new WaitForSeconds(1f);
        roundText.text = "";
        yield return new WaitForSeconds(1f);
        roundText.text = "Round: " + currentRound;
        yield return new WaitForSeconds(1f);
        roundText.text = "";
        yield return new WaitForSeconds(1f);
        roundText.text = "Round: " + currentRound;
        robotsThisRound += 10;
        robotsLeft = robotsThisRound;
        roundChanging = false;
    }

    public void RecordRound()
    {
        print("Dead");
        if(currentRound > highestRound)
        {
            PlayerPrefs.SetInt("HighestRound", currentRound);
            print("New highscore");
        }
        else
        {
            print("This is not a highscore!");
        }
    }
}
