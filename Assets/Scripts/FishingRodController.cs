using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FishingRodController : MonoBehaviour
{
	[Header("Fishing Rod value references")]
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

	[Header("Cast line references")]
	[SerializeField] private LineRenderer linePrefab;
	[SerializeField] private GameObject bobberPrefab;
	[SerializeField] private GameObject splashPrefab;
	[SerializeField] private Transform castLineStartPoint;
	[Range(15f, 30f)]
	[SerializeField] private float minCastDistance = 20.0f;
	[Range(15f, 80f)]
	[SerializeField] private float maxCastDistance = 80.0f;

	public Transform CastLineStartPoint => castLineStartPoint;

	private Coroutine castRoutine;
	private Quaternion baseFishingRodRotation;
	private GameObject currentBobber;
	private LineRenderer currentLine;
	public Vector3 CastEndPoint;

	private void Start()
	{
		baseFishingRodRotation = transform.localRotation;
	}

	private void Update()
	{
		if (currentBobber != null)
		{
			Vector3 pos = currentBobber.transform.position;
			pos.y = Mathf.Sin(Time.time * 2f) * 0.05f;
			currentBobber.transform.position = pos;
		}
	}

	private void LateUpdate()
	{
		if (currentLine == null) return;

		Vector3 start = castLineStartPoint.position;

		currentLine.SetPosition(0, start);
		currentLine.SetPosition(1, CastEndPoint);

		if (currentBobber != null)
			currentLine.SetPosition(1, currentBobber.transform.position);
		else
			currentLine.SetPosition(1, CastEndPoint);
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

		SpawnCastLine();

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

	private void SpawnCastLine()
	{
		if(currentLine != null)
		{
			Destroy(currentLine.gameObject);
		}

		currentLine = Instantiate(linePrefab);

		float distance = Random.Range(minCastDistance, maxCastDistance);

		Vector3 forwardFlat = transform.forward;
		forwardFlat.y = 0f;
		forwardFlat.Normalize();

		CastEndPoint = castLineStartPoint.position + forwardFlat * distance;
		CastEndPoint.y = 0f;

		currentLine.positionCount = 2;

		if(currentBobber != null)
		{
			Destroy(currentBobber.gameObject);
		}

		currentBobber = Instantiate(bobberPrefab, CastEndPoint, Quaternion.identity);
		Vector3 fxOffset = new Vector3(0f, 1f, 0f);
		Instantiate(splashPrefab, CastEndPoint + fxOffset, Quaternion.identity);

		GameManager.Instance.RippleController.TriggerRipple(CastEndPoint);
		//StartCoroutine(DelayedRipple());
	}

	public void UpdateLineEndPoint(Vector3 endPos)
	{
		if (currentLine == null) return;

		currentLine.SetPosition(0, castLineStartPoint.position);
		currentLine.SetPosition(1, endPos);
	}

	public void DisableLine()
	{
		if (currentLine != null)
		{
			Destroy(currentLine.gameObject);
			Destroy(currentBobber.gameObject);
			currentLine = null;
		}
	}

	//private IEnumerator DelayedRipple()
	//{
	//	yield return new WaitForSeconds(0.5f);
	//	GameManager.Instance.RippleController.TriggerRipple(castEndPoint);
	//}
}
