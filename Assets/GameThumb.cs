using UnityEngine;
using UnityEngine.UI;

public class GameThumb : MonoBehaviour
{
	public Renderer Renderer;
	public int RendererMaterialIndex = 1;
	public TMPro.TMP_Text Title;
	public TMPro.TMP_Text Players;
	public Animator Animator;

	static int selectedHash;
	static int deselectedHash;

	bool selectionState;
	public float animationT;

	public float TransitionDurationForOpen = 0.2f;
	public float TransitionDurationForClose = 0.2f;

	public Transform DiscTransform;
	Vector3 discCachedPos;
	[Range(0.01f, 5f)]
	public float DiscSpeed = 0.5f;
	[Range(0.0001f, 1f)]
	public float MaxDiscDistance = 0.1f;

	private void Awake()
	{
		if (selectedHash == 0 && deselectedHash == 0)
		{
			selectedHash = Animator.StringToHash("Selected");
			deselectedHash = Animator.StringToHash("Deselected");
		}
	}

	private void LateUpdate()
	{
		if (selectionState)
		{
			Vector3 target = DiscTransform.position;
			DiscTransform.position = Vector3.Lerp(
				discCachedPos,
				target,
				Time.deltaTime * DiscSpeed);
			Vector3 pos = DiscTransform.position;
			if (MaxDiscDistance * MaxDiscDistance < Vector3.SqrMagnitude(target - pos))
				DiscTransform.position = Vector3.ClampMagnitude(pos - target, MaxDiscDistance) + target;
			discCachedPos = DiscTransform.position;
		}
		else
		{
			discCachedPos = DiscTransform.position;
		}
	}

	public void Conf(GameAsset asset)
	{
		if (Renderer)
			Renderer.materials[RendererMaterialIndex].mainTexture = asset.Card;

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
		if (Animator.IsInTransition(0))
		{
			if (val && selectionState == false)
				Animator.Play(selectedHash, 0);
			else if (val == false && selectionState == true)
				Animator.Play(deselectedHash, 0);
		}
		else
		{
			if (val && selectionState == false)
				Animator.CrossFade(selectedHash, TransitionDurationForOpen, 0);
			else if (val == false && selectionState == true)
				Animator.CrossFade(deselectedHash, TransitionDurationForClose, 0);
		}

		selectionState = val;
	}
}
