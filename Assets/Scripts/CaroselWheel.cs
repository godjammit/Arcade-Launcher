using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public TextMesh title, author, players, loading;

    public GameObject gamePrefab;
    private GameObject[] games;
    private GameAsset[] gameAssets;
    private int currentPosition = 0;

    private bool input = false;
    private float currentTime;


    private void Start()
    {   
    	//make sure settings are set to windowed 1080p (reboot exe to take effect)
		PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", 0);
		PlayerPrefs.SetInt("Screenmanager Resolution Width", 1920);
		PlayerPrefs.SetInt("Screenmanager Resolution Height", 1080);
        Screen.showCursor = false;

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
    	//prevent null ref when no games are found
		if(games == null)
			return;

        if (input)
        {
            if ((currentTime += Time.deltaTime) > 2f)
            {
                currentTime = 0;
                //input = false;
                loading.text = "";
            }
        }

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
        
		if (!isLaunching && GetKeyInputButton1())
        {
            input = true;
            loading.text = "Loading";
            Debug.Log("load");
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
        // FIXME: temp fix to only allow player 1 to select games.
        return Input.GetButtonDown("P1_Button1");
           // || Input.GetButtonDown("P2_Button1")
           // || Input.GetButtonDown("P3_Button1")
           // || Input.GetButtonDown("P4_Button1");
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
        if (data.PlayersMax == data.PlayersMin)
            players.text = data.PlayersMin + ((data.PlayersMin == 1) ? " Player" : " Players");
        else
            players.text = data.PlayersMin + " to " + data.PlayersMax + " Players";
    }


	bool isLaunching = false;
	float launchCooldown = 5f;

    private void LaunchGame()
    {
		if(isLaunching)
			return;

        System.Diagnostics.Process.Start(gameAssets[currentPosition].ExecutablePath);

		StartCoroutine( LaunchCooldownRoutine() );
    }

	private IEnumerator LaunchCooldownRoutine()
	{
		isLaunching = true;
		yield return new WaitForSeconds(launchCooldown);
		isLaunching = false;
	}
}