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

	public Renderer ShellRendererFront;
	public Renderer ShellRendererBack;
	public Renderer DiscRenderer;

	int propertyName_BaseMap;

	private void Awake()
	{
		if (selectedHash == 0 && deselectedHash == 0)
		{
			selectedHash = Animator.StringToHash("Selected");
			deselectedHash = Animator.StringToHash("Deselected");
		}
		propertyName_BaseMap = Shader.PropertyToID("_BaseMap");
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

	GameAsset configuredAsset = null;

	public void Conf(GameAsset asset)
	{
		if (asset == configuredAsset)
			return;
		configuredAsset = asset;

		if (Renderer)
			Renderer.materials[RendererMaterialIndex].mainTexture = asset.Card;

		if (Title) {
			Title.enabled = selectionState == false;
			Title.text = asset.GameData.Title;
		}

		if (Players)
		{
			if (asset.GameData.Players > 1)
				Players.text = $"{asset.GameData.Players} Players";
			else
				Players.text = $"{asset.GameData.Players} Player";
		}

		if (ShellRendererFront)
		{
			Color c;
			ColorUtility.TryParseHtmlString(asset.GameData.JewelCaseFrontColor, out c);
			c.a = ShellRendererFront.materials[0].color.a;
			ShellRendererFront.materials[0].color = c;
		}
		if (ShellRendererBack)
		{
			Color c;
			ColorUtility.TryParseHtmlString(asset.GameData.JewelCaseBackColor, out c);
			c.a = ShellRendererBack.materials[0].color.a;
			ShellRendererBack.materials[0].color = c;
			Vector2 offset = Vector2.zero;
			ShellRendererBack.materials[0].SetTextureOffset(propertyName_BaseMap, new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
			ShellRendererBack.materials[1].color = c;
			ShellRendererBack.materials[1].SetTextureOffset(propertyName_BaseMap, new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
		}
		if (DiscRenderer)
		{
			Color c;
			ColorUtility.TryParseHtmlString(asset.GameData.DiscColor, out c);
			c.a = DiscRenderer.materials[0].color.a;
			DiscRenderer.materials[0].color = c;
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
