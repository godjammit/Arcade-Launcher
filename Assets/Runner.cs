using System;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine.Audio;

public class Runner : MonoBehaviour
{
	public bool IsLaunching;
	public PlayableDirector Launch;
	public AudioMixerSnapshot MixerGameLaunched;
	public AudioMixerSnapshot MixerRegular;

	public void Run(String path)
	{
		UnityEngine.Debug.Log($"Running Game: {path}");

		Process myProcess = new Process();
		myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
		myProcess.StartInfo.CreateNoWindow = true;
		myProcess.StartInfo.UseShellExecute = false;
		myProcess.StartInfo.FileName = path;
		myProcess.EnableRaisingEvents = true;
		StartCoroutine(RunProcess(myProcess));
	}

	IEnumerator RunProcess(Process process)
	{
		IsLaunching = true;
		if (MixerGameLaunched)
			MixerGameLaunched.TransitionTo(2f);

		Launch.Play();
		yield return new WaitForSeconds((float)Launch.duration - 0.25f);

		Screen.fullScreen = false;
		yield return new WaitForSeconds(1f);
		process.Start();
		process.WaitForExit();
		Screen.fullScreen = true;
		Cursor.visible = false;
		Launch.Stop();
		if (MixerRegular)
			MixerRegular.TransitionTo(2f);
		IsLaunching = false;
	}
}
