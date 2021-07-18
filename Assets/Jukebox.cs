using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
	public static Jukebox Instance;
	public AudioSource Action;
	public AudioSource Creepy;
	public AudioSource Cute;
	public AudioSource Puzzle;
	public AudioSource Party;

	string currentSongName = null;
	private AudioSource currentSongSource = null;

	string queuedSongTarget = null;

	public float CrossfadeDuration = 0.5f;

	public float CrossfadeDelay = 3.5f;
	float _delayTimer;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (_delayTimer > 0f)
		{
			_delayTimer -= Time.unscaledDeltaTime;
			if (_delayTimer <= 0f)
			{
				AudioSource targetSong = GetSource(queuedSongTarget);
				currentSongSource = targetSong;
			}
		}
		ManageFade(Action);
		ManageFade(Creepy);
		ManageFade(Cute);
		ManageFade(Puzzle);
		ManageFade(Party);

		Sync(Action, Creepy, Cute, Puzzle, Party);
	}

	private void Sync(params AudioSource[] sources)
	{
		float targetTime = sources[0].time;
		for (int i = 1; i < sources.Length; i++)
		{
			if (sources[i].volume > 0.05f)
				continue;
			sources[i].time = targetTime;
		}
	}

	void ManageFade(AudioSource source)
	{
		source.volume = Mathf.MoveTowards(source.volume, (currentSongSource == source) ? 1f : 0f, Time.unscaledDeltaTime / CrossfadeDuration);
	}

	public void PlaySong(string song)
	{
		if (currentSongName == song)
			return;

		queuedSongTarget = song;
		_delayTimer = CrossfadeDelay;
	}

	AudioSource GetSource(string song)
	{
		switch (song.ToLower())
		{
			case "action":
				return Action;
			case "creepy":
				return Creepy;
			case "cute":
				return Cute;
			case "puzzle":
				return Puzzle;
			case "party":
			default:
				return Party;
		}
	}
}
