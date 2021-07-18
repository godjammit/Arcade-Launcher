using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaroselWheel : MonoBehaviour
{
	public float speed;
	public float gameSpacing;

	public float _indexSmoothed;

	public TMPro.TMP_Text title, author, players;

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

	public float SelectionScaleMultiplier = 1.2f;
	[Range(0f, 10f)]
	public float SelectionFalloff = 3f;
	private Vector3 defaultScale;

	public Transform FlipOutPosition;
	public float FlipOutDuration = 0.5f;
	public float FlipBackDuration = 0.1f;
	public AnimationCurve FlipOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float SelectionFloatMagnitude = 0.01f;
	public float SelectionFloatSpeed = 1f;

	float timeSinceGameSelection;
	public float TextRevealDuration = 0.75f;

	private void Start()
	{
		defaultScale = gamePrefab.transform.localScale;

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

		int gamesIndex = mod(currentPositionTarget, gameAssets.Length);
		Jukebox.Instance.PlaySong(gameAssets[gamesIndex].GameData.Song);
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
		int max = min + AmountToSpawn;
		int startPoint = (AmountToSpawn * Mathf.CeilToInt(((float)min/(float)AmountToSpawn)));

		Vector3 mountPoint = this.FlipOutPosition.position;
		mountPoint += new Vector3(0f, Mathf.Sin(SelectionFloatSpeed * Time.time) * SelectionFloatMagnitude);

		for (int i = 0; i < AmountToSpawn; i++)
		{
			Transform gameTransform = games[i].transform;
			int pos = startPoint + i;
			if (pos > max)
				pos -= AmountToSpawn;
			int gamesIndex = mod(pos, gameAssets.Length);

			games[i].Conf(gameAssets[gamesIndex]);
			var y = -pos * Spacing;
			float distance01 = Mathf.Clamp01( Mathf.Abs((float)pos - currentPositionFloat) / SelectionFalloff);
			games[i].transform.localScale = Vector3.Lerp(defaultScale * SelectionScaleMultiplier, defaultScale, distance01);

			bool isSelection = pos == currentPositionTarget;

			if (isSelection)
			{
				games[i].animationT = Mathf.MoveTowards(games[i].animationT, 1f, Time.deltaTime / FlipOutDuration);
			}
			else
			{
				games[i].animationT = Mathf.MoveTowards(games[i].animationT, 0f, Time.deltaTime / FlipBackDuration);
			}

			games[i].animationT = Mathf.Clamp01(games[i].animationT);

			float time01 = FlipOutCurve.Evaluate(games[i].animationT);

			//float wave = Mathf.Lerp(-10f, 10f, Mathf.PerlinNoise(y, y * 1.32f));
			float rand = Mathf.Lerp(-12f, 12f, Mathf.PerlinNoise(pos * 1.245f, y * 2.12f));
			float randZ = Mathf.Lerp(-.2f, .2f, Mathf.PerlinNoise(pos * 3.123f, y * 2.1123f));
			float randX = Mathf.Lerp(-.1f, .1f, Mathf.PerlinNoise(pos * 1.123f, .2f));

			gameTransform.position = Vector3.LerpUnclamped(gameContainer.transform.TransformPoint(new Vector3(randX, y, randZ)), mountPoint, time01);
			gameTransform.rotation = Quaternion.Euler(0f, rand, 0f);

			games[i].SetSelection(isSelection);
		}
	}

	private void Update()
	{
		if (Runner.IsLaunching == true)
			return;

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

		int prevPos = currentPosition;
		currentPosition = Mathf.RoundToInt(currentPositionFloat);

		//reset timer if index changed
		if (prevPos != currentPosition)
			timeSinceGameSelection = 0f;
		timeSinceGameSelection += Time.deltaTime;

		gameContainer.transform.localPosition = new Vector3(0f, currentPositionFloat * Spacing, 0f);

		UpdateSelectionFields();
		PositionGames();

		//reload launcher after 9 hours to retain floating point accuracy
		if (Time.timeSinceLevelLoad > 60f * 60f * 9f)
		{
			SceneManager.LoadScene(0);
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
		int gamesIndex = mod(currentPositionTarget, gameAssets.Length);
		Jukebox.Instance.PlaySong(gameAssets[gamesIndex].GameData.Song);
	}

	private void ShiftTilesBack()
	{
		currentPositionTarget--;
		int gamesIndex = mod(currentPositionTarget, gameAssets.Length);
		Jukebox.Instance.PlaySong(gameAssets[gamesIndex].GameData.Song);
	}

	private void UpdateSelectionFields()
	{
		int index = mod(currentPosition, gameAssets.Length);
		Game data = gameAssets[index].GameData;
		title.text = data.Title;
		author.text = data.Author;
		players.text = data.Players + ((data.Players == 1) ? " Player" : " Players");

		if (timeSinceGameSelection < TextRevealDuration)
		{
			var reveal01 = (1f - Mathf.Clamp01(timeSinceGameSelection / TextRevealDuration));
			reveal01 *= reveal01 * reveal01; //cubed falloff

			title.firstVisibleCharacter = Mathf.RoundToInt(reveal01 * (float)data.Title.Length);
			author.firstVisibleCharacter = Mathf.RoundToInt(reveal01 * (float)data.Author.Length);
			players.firstVisibleCharacter = Mathf.RoundToInt(reveal01 * (float)players.text.Length);
		}
		else
		{
			title.firstVisibleCharacter = 0;
			author.firstVisibleCharacter = 0;
			players.firstVisibleCharacter = 0;
		}
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