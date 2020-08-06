using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private static Main _main;
    public static Main Instance
    {
        get { return _main; }
    }

    void Awake()
    {
        _main = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject PointsContainer;
    public GameObject ProppelerEffects;
    public GameObject GameView;
    public GameObject MainMenuView;

    public GameData CompletedGame;

    // Use this for initialization
    void Start()
    {
        PointsContainer.SetActive(false);
        ProppelerEffects.SetActive(false);
        GameView.SetActive(false);
        MainMenuView.SetActive(true);

    }

    public void StartGame()
    {
        MainMenuView.SetActive(false);
        SceneManager.LoadScene("GameScene_2");
    }

    public void OnEndGame()
    {
        if (MainMenuView.activeSelf == true)
            MainMenuView.SetActive(false);
        SceneManager.LoadScene("LeaderBoards");
    }

    public void OnEdnGameTest()
    {
        CompletedGame = new GameData(215461);
        OnEndGame();
    }
}
