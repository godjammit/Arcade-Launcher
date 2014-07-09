using System;
using System.Collections.Generic;
using System.IO;
using Prime31.GoKitLite;
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

    public UILabel title, author, players;

    public GameObject gamePrefab;
    private GameObject[] games;
    private GameAsset[] gameAssets;
    private int currentPosition = 0;

    private void Start()
    {   
        DataLoader dl = new DataLoader();
        List<GameAsset> assets = dl.GetGameAssetsList();
        InstantiateGames(assets);
        SetUpGamesAtStart();
        UpdateData();
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
            g.renderer.material.mainTexture = ga.Card;
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
        if (currentPosition > 0 && Input.GetKeyDown(KeyCode.LeftArrow)) // move to left game
        {
            ShiftTilesRight(currentPosition);
            UpdateData();
        }
        else if (currentPosition < games.Length - 1 && Input.GetKeyDown(KeyCode.RightArrow)) // move to right game
        {
            ShiftTilesLeft(currentPosition);
            UpdateData();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            LaunchGame();
        }
    }

    private void ShiftTilesRight(int current)
    {
        if (currentPosition - 1 >= 0)
        {
            if (currentPosition + 1 < games.Length)
            {
                // shift right off screen
                GoKitLite.instance.positionTo(games[currentPosition + 1].transform, .5f, rightOffScreen, 0, GoKitLiteEasing.Cubic.EaseInOut);
            }

            // shift center to right
            GoKitLite.instance.positionTo(games[currentPosition].transform, .5f, rightScreen, 0, GoKitLiteEasing.Cubic.EaseInOut);

            // shift left to center
            GoKitLite.instance.positionTo(games[currentPosition - 1].transform, .5f, centerScreen, 0, GoKitLiteEasing.Cubic.EaseInOut);

            if (currentPosition - 2 >= 0)
            {
                // shift off screen left to left
                GoKitLite.instance.positionTo(games[currentPosition - 2].transform, .5f, leftScreen, 0, GoKitLiteEasing.Cubic.EaseInOut);
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
                GoKitLite.instance.positionTo(games[currentPosition - 1].transform, .5f, leftOffScreen, 0, GoKitLiteEasing.Cubic.EaseInOut);
            }
            // shift center to left
            GoKitLite.instance.positionTo(games[currentPosition].transform, .5f, leftScreen, 0, GoKitLiteEasing.Cubic.EaseInOut);

            // shift right to center
            GoKitLite.instance.positionTo(games[currentPosition + 1].transform, .5f, centerScreen, 0, GoKitLiteEasing.Cubic.EaseInOut);

            if (currentPosition + 2 < games.Length)
            {
                // shift right off screen to right
                GoKitLite.instance.positionTo(games[currentPosition + 2].transform, .5f, rightScreen, 0, GoKitLiteEasing.Cubic.EaseInOut);
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
        Debug.Log(gameAssets[currentPosition].ExecutablePath);
        System.Diagnostics.Process.Start(gameAssets[currentPosition].ExecutablePath);
    }
}