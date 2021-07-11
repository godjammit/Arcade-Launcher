using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;


public class CaroselWheel : MonoBehaviour
{
	public float speed;
	public float gameSpacing;

	public float _indexSmoothed;

	public TextMesh title, author, players;

	public GameObject gamePrefab;
	public GameObject gameContainer;
	private GameThumb[] games;
	private GameAsset[] gameAssets;
	private int currentPosition = 0;
	private int currentPositionTarget = 0;
	private float currentPositionFloat = 0f;
	private float currentPositionVel = 0f;

	public int AmountToSpawn = 10;

	public Runner Runner;

	[Range(0f, 10f)]
	public float AngularFrequency = 2f;
	[Range(0f, 1f)]
	public float Dampen = 0.2f;
	public float Speed = 5f;

	public float Spacing = 1f;

	private void Start()
	{
		DataLoader dl = new DataLoader();
		List<GameAsset> assets = dl.GetGameAssetsList();
		if (!assets.Any())
		{
			title.text = "No Games Found";
		}
		else
		{
			InstantiateGames(assets);
			UpdateSelectionFields();
		}
	}

	private void InstantiateGames(List<GameAsset> a)
	{
		gameAssets = new GameAsset[a.Count];
		for (int i = 0; i < a.Count; i++)
		{
			gameAssets[i] = a[i];
		}

		games = new GameThumb[AmountToSpawn];
		for (int i = 0; i < AmountToSpawn; i++)
		{
			GameObject g = (GameObject) Instantiate(gamePrefab);
			games[i] = g.GetComponent<GameThumb>();
			g.transform.SetParent(gameContainer.transform);
		}
	}

	private void PositionGames()
	{
		int midpoint = AmountToSpawn / 2;
		int min = currentPosition - midpoint;
		for (int i = 0; i < AmountToSpawn; i++)
		{
			int pos = min + i;
			int gamesIndex = mod(pos, gameAssets.Length);

			games[i].Conf(gameAssets[gamesIndex]);
			games[i].transform.localPosition = new Vector3(0f, -pos * Spacing, 0f);
		}
	}

	private void Update()
	{
		if (GetKeyInputDown()) // move to left game
		{
			ShiftTilesForward();
		}
		else if (GetKeyInputUp()) // move to right game
		{
			ShiftTilesBack();
		}
		else if (GetKeyInputButton1())
		{
			LaunchGame();
		}

		currentPositionFloat = NumericSpringing.Spring_Float(currentPositionFloat, ref currentPositionVel, (float)currentPositionTarget, Dampen, AngularFrequency, Time.deltaTime * Speed);
		currentPosition = Mathf.RoundToInt(currentPositionFloat);

		gameContainer.transform.localPosition = new Vector3(0f, currentPositionFloat * Spacing, 0f);

		UpdateSelectionFields();
		PositionGames();
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

	private bool GetKeyInputUp()
	{
		return Input.GetButtonDown("P1_Up")
			|| Input.GetButtonDown("P2_Up")
			|| Input.GetButtonDown("P3_Up")
			|| Input.GetButtonDown("P4_Up");
	}

	private bool GetKeyInputDown()
	{
		return Input.GetButtonDown("P1_Down")
			|| Input.GetButtonDown("P2_Down")
			|| Input.GetButtonDown("P3_Down")
			|| Input.GetButtonDown("P4_Down");
	}

	private bool GetKeyInputButton1()
	{
		return Input.GetButtonDown("P1_Button1")
			|| Input.GetButtonDown("P2_Button1")
			|| Input.GetButtonDown("P3_Button1")
			|| Input.GetButtonDown("P4_Button1");
	}

	private void ShiftTilesForward()
	{
		currentPositionTarget++;
	}

	private void ShiftTilesBack()
	{
		currentPositionTarget--;
	}

	private void UpdateSelectionFields()
	{
		int index = mod(currentPosition, gameAssets.Length);
		Game data = gameAssets[index].GameData;
		title.text = data.Title;
		author.text = data.Author;
		players.text = data.Players + ((data.Players == 1) ? " Player" : " Players");
	}

	int mod(int k, int n)
	{
		return ((k %= n) < 0) ? k + n : k;
	}


	private void LaunchGame()
	{
		int index = mod(currentPositionTarget, gameAssets.Length);
		this.Runner.Run(gameAssets[index].ExecutablePath);
	}
}