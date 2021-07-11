using UnityEngine;

public class GameThumb : MonoBehaviour
{
	public Renderer Renderer;
	public TextMesh Title;
	public TextMesh Players;

	public void Conf(GameAsset asset)
	{
		Renderer.material.mainTexture = asset.Card;
		Title.text = asset.GameData.Title;
		if (asset.GameData.Players > 1)
			Players.text = $"{asset.GameData.Players} Players";
		else
			Players.text = $"{asset.GameData.Players} Player";
	}
}
