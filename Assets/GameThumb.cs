using UnityEngine;
using UnityEngine.UI;

public class GameThumb : MonoBehaviour
{
	public Renderer Renderer;
	public TMPro.TMP_Text Title;
	public TMPro.TMP_Text Players;
	public Animator Animator;

	static int selectedHash;
	static int deselectedHash;

	bool selectionState;

	private void Awake()
	{
		if (selectedHash == 0 && deselectedHash == 0)
		{
			selectedHash = Animator.StringToHash("Selected");
			deselectedHash = Animator.StringToHash("Deselected");
		}
	}

	public void Conf(GameAsset asset)
	{
		if (Renderer)
			Renderer.material.mainTexture = asset.Card;

		if (Title)
			Title.text = asset.GameData.Title;

		if (Players)
		{
			if (asset.GameData.Players > 1)
				Players.text = $"{asset.GameData.Players} Players";
			else
				Players.text = $"{asset.GameData.Players} Player";
		}
	}

	public void SetSelection(bool val)
	{
		if (val && selectionState == false)
			Animator.CrossFade(selectedHash, 0.1f, 0);
		else if (val == false && selectionState == true)
			Animator.CrossFade(deselectedHash, 0.1f, 0);

		selectionState = val;
	}
}
