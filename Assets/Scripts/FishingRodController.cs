using System.Collections;
using UnityEngine;

public class FishingRodController : MonoBehaviour
{
	[SerializeField] private float maxNegativeRotation = -20.0f;
	[SerializeField] private float maxPositiveRotation = 50.0f;
	[SerializeField] private float windupSpeed = 5.0f;
	[SerializeField] private float castSpeed = 10.0f;
	[Range(0.5f, 5f)]
	[Tooltip("How fast the fishing rod would return back to the normal rotation after casting")]
	[SerializeField] private float returnSpeed = 0.5f;
	[Tooltip("Small pause at end of windup for better feel")]
	[SerializeField] private float windupPause = 0.05f;
	[Tooltip("Small pause at end of cast for better follow through feel")]
	[SerializeField] private float castFollowThroughPause = 0.25f;

	private Coroutine castRoutine;
	private Quaternion baseFishingRodRotation;

	private void Start()
	{
		baseFishingRodRotation = transform.localRotation;
	}

	public void StartFishingRodCastWindup()
	{
		//for polish later
	}

	public void ReleaseCastWindup(System.Action onCastComplete)
	{
		if(castRoutine != null)
		{
			StopCoroutine(castRoutine);
		}

		castRoutine = StartCoroutine(CastCoroutine(onCastComplete));
	}

	private IEnumerator CastCoroutine(System.Action onCastComplete)
	{
		yield return RotateToAngle(maxNegativeRotation, windupSpeed);

		yield return new WaitForSeconds(windupPause);

		yield return RotateToAngle(maxPositiveRotation, castSpeed);

		yield return new WaitForSeconds(castFollowThroughPause);

		yield return RotateToAngle(0f, returnSpeed * 0.5f);

		onCastComplete?.Invoke();
	}

	private IEnumerator RotateToAngle(float targetAngle, float speed)
	{
		Quaternion startRot = transform.localRotation;
		Quaternion targetRot = baseFishingRodRotation * Quaternion.Euler(targetAngle, 0f, 0f);

		float time = 0f;

		while (time < 1f)
		{
			time += Time.deltaTime * speed;
			float easedTime = Mathf.SmoothStep(0f, 1f, time);

			transform.localRotation = Quaternion.Slerp(startRot, targetRot, easedTime);
			yield return null;
		}

		transform.localRotation = targetRot;
	}
}
