using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject leaderboardScreen;
    [SerializeField] GameObject enterNameScreen;
    [SerializeField] GameObject mainMenu;

    public string playerName;
    public string saveName;

    public TextMeshProUGUI inputText;

    public TextMeshProUGUI leaderSlotOne;
    public TextMeshProUGUI leaderSlotTwo;
    public TextMeshProUGUI leaderSlotThree;

    public int highestRound;

    bool startGone;

    void Start()
    {
        mainMenu.SetActive(false);
        startScreen.SetActive(true);
        leaderboardScreen.SetActive(false);
        enterNameScreen.SetActive(false);
        highestRound = PlayerPrefs.GetInt("HighestRound");
        SetLeaderSlot();
    }

    void Update()
    {
        playerName = PlayerPrefs.GetString("name");

        if(Input.GetButton("Fire1") && !startGone)
        {
            startGone = true;
            startScreen.SetActive(false);
            mainMenu.SetActive(true);
        }

        print(playerName);
    }

    public void ExitGameBtn()
    {
        Application.Quit();
    }

    public void StartGameBtn()
    {
        enterNameScreen.SetActive(true);
        mainMenu.SetActive(false);
        startScreen.SetActive(false); ;
        leaderboardScreen.SetActive(false);
        enterNameScreen.SetActive(true);
    }

    public void LeaderboardBtn()
    {
        mainMenu.SetActive(false);
        startScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
        enterNameScreen.SetActive(false);
    }

    public void BackBtn()
    {
        enterNameScreen.SetActive(true);
        mainMenu.SetActive(true);
        startScreen.SetActive(false); ;
        leaderboardScreen.SetActive(false);
        enterNameScreen.SetActive(false);
    }

    public void SetName()
    {
        saveName = inputText.text;
        PlayerPrefs.SetString("name", saveName);
        SceneManager.LoadScene("Final Scene", LoadSceneMode.Single);
    }

    void SetLeaderSlot()
    {
        if(highestRound != 0)
        {
            leaderSlotOne.text = PlayerPrefs.GetString("name") + " - Round " + highestRound;
        }

        if(highestRound == 0)
        {
            leaderSlotOne.text = "";
        }
    }
}
