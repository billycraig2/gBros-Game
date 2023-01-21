using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatTracker : MonoBehaviour
{
    public int playerKills;
    public int playerPoints;

    TextMeshProUGUI pointsText;

    void Start()
    {
        pointsText = GameObject.FindWithTag("PointsCounter").GetComponent<TextMeshProUGUI>();
        playerPoints = 0;
        playerKills = 0;
    }

    void Update()
    {
        SetPointsCounter();
    }

    void SetPointsCounter()
    {
        pointsText.text = "$" + playerPoints;
    }
}
