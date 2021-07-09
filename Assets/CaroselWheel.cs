using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;

public class CaroselWheel : MonoBehaviour
{
    public float speed;
    public Vector3 leftOffScreen;
    public Vector3 leftScreen;
    public Vector3 centerScreen;
    public Vector3 rightScreen;
    public Vector3 rightOffScreen;

    public TextMesh title, author, players;

    public GameObject gamePrefab;
    private GameObject[] games;
    private GameAsset[] gameAssets;
    private int currentPosition = 0;

    public Runner Runner;

    private void Start()
    {   
        Go.defaultEaseType = GoEaseType.CubicInOut;
        DataLoader dl = new DataLoader();
        List<GameAsset> assets = dl.GetGameAssetsList();
        if (!assets.Any())
        {
            title.text = "No Games Found";
        }
        else
        {
            InstantiateGames(assets);
            SetUpGamesAtStart();
            UpdateData();   
        }
    }

    private void InstantiateGames(List<GameAsset> a)
    {
        games = new GameObject[a.Count];
        gameAssets = new GameAsset[a.Count];

        int i = 0;
        foreach (GameAsset ga in a)
        {
            GameObject g = (GameObject) Instantiate(gamePrefab);
            games[i] = g;
            gameAssets[i] = ga;
            g.GetComponent<Renderer>().material.mainTexture = ga.Card;
            i++;
        }
    }

    private void SetUpGamesAtStart()
    {
        for (int i = 0; i < games.Length; i++)
        {
            if (i == 0)
            {
                games[0].transform.position = centerScreen;
            }
            else if (i == 1)
            {
                games[1].transform.position = rightScreen;
            }
            else
            {
                games[i].transform.position = rightOffScreen;
            }
        }
    }

    private void Update()
    {
        if (currentPosition > 0 && GetKeyInputLeft()) // move to left game
        {
            ShiftTilesRight(currentPosition);
            UpdateData();
        }
        else if (currentPosition < games.Length - 1 && GetKeyInputRight()) // move to right game
        {
            ShiftTilesLeft(currentPosition);
            UpdateData();
        }
        else if (GetKeyInputButton1())
        {
            LaunchGame();
        }
    }

    private bool GetKeyInputLeft()
    {
        return Input.GetButtonDown("P1_Left")
            || Input.GetButtonDown("P2_Left")
            || Input.GetButtonDown("P3_Left")
            || Input.GetButtonDown("P4_Left");
    }

    private bool GetKeyInputRight()
    {
        return Input.GetButtonDown("P1_Right")
            || Input.GetButtonDown("P2_Right")
            || Input.GetButtonDown("P3_Right")
            || Input.GetButtonDown("P4_Right");
    }

    private bool GetKeyInputButton1()
    {
        return Input.GetButtonDown("P1_Button1")
            || Input.GetButtonDown("P2_Button1")
            || Input.GetButtonDown("P3_Button1")
            || Input.GetButtonDown("P4_Button1");
    }

    private void ShiftTilesRight(int current)
    {
        if (currentPosition - 1 >= 0)
        {
            if (currentPosition + 1 < games.Length)
            {
                // shift right off screen
                //Go.to(games[currentPosition + 1].transform, rightOffScreen, GoEasing.Cubic.EaseInOut);
                games[currentPosition + 1].transform.positionTo(0.5f, rightOffScreen);
            }

            // shift center to right
            //Go.instance.positionTo(games[currentPosition].transform, .5f, rightScreen, 0, GoEasing.Cubic.EaseInOut);
            games[currentPosition].transform.positionTo(0.5f, rightScreen);

            // shift left to center
            //Go.instance.positionTo(games[currentPosition - 1].transform, .5f, centerScreen, 0, GoEasing.Cubic.EaseInOut);
            games[currentPosition - 1].transform.positionTo(0.5f, centerScreen);

            if (currentPosition - 2 >= 0)
            {
                // shift off screen left to left
                //Go.instance.positionTo(games[currentPosition - 2].transform, .5f, leftScreen, 0, GoEasing.Cubic.EaseInOut);
                games[currentPosition - 2].transform.positionTo(0.5f, leftScreen);
            }
            currentPosition--;
        }
        
    }

    private void ShiftTilesLeft(int current)
    {
        if (currentPosition + 1 < games.Length)
        {       
            if (currentPosition - 1 >= 0)
            {
                // shift left off screen
                //Go.instance.positionTo(games[currentPosition - 1].transform, .5f, leftOffScreen, 0, GoEasing.Cubic.EaseInOut);
                games[currentPosition - 1].transform.positionTo(0.5f, leftOffScreen);
            }
            // shift center to left
            //Go.instance.positionTo(games[currentPosition].transform, .5f, leftScreen, 0, GoEasing.Cubic.EaseInOut);
            games[currentPosition].transform.positionTo(0.5f, leftScreen);

            // shift right to center
            //Go.instance.positionTo(games[currentPosition + 1].transform, .5f, centerScreen, 0, GoEasing.Cubic.EaseInOut);
            games[currentPosition + 1].transform.positionTo(0.5f, centerScreen);

            if (currentPosition + 2 < games.Length)
            {
                // shift right off screen to right
                //Go.instance.positionTo(games[currentPosition + 2].transform, .5f, rightScreen, 0, GoEasing.Cubic.EaseInOut);
                games[currentPosition + 2].transform.positionTo(0.5f, rightScreen);
            }
            currentPosition++;
        }
    }

    private void UpdateData()
    {
        Game data = gameAssets[currentPosition].GameData;
        title.text = data.Title;
        author.text = data.Author;
        players.text = data.Players + ((data.Players == 1) ? " Player" : " Players");
    }

    private void LaunchGame()
    {
        this.Runner.Run(gameAssets[currentPosition].ExecutablePath);
    }
}