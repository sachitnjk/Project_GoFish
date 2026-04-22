using UnityEngine;

public class FishBase : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Animator animator;

	private void Update()
	{
		FaceCam();
	}

	public void PopulateVisualData(FishRuntimeData fishRuntimeData)
 	{
		spriteRenderer.sprite = fishRuntimeData.fishData.fishSprite;
		if(fishRuntimeData.fishData.overrideController != null)
		{
			animator.runtimeAnimatorController = fishRuntimeData.fishData.overrideController;
		}
		else
		{
			animator.runtimeAnimatorController = null;
		}
	}

	private void FaceCam()
	{
		if (Camera.main == null) return;

		Vector3 direction = Camera.main.transform.position - transform.position;
		transform.forward = direction;
	}
}
