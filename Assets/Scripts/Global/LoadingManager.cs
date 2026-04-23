using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
	public static string TargetScene;
	[SerializeField] private Slider progressSlider;

	private string sceneToLoad;

	private void Start()
	{
		sceneToLoad = TargetScene;

		if (string.IsNullOrEmpty(sceneToLoad))
		{
			Debug.LogError("No scene set for loading!");
			return;
		}

		StartCoroutine(LoadSceneAsync());
	}

	private IEnumerator LoadSceneAsync()
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
		operation.allowSceneActivation = false;

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);

			progressSlider.value = progress;

			if (operation.progress >= 0.9f)
			{
				yield return new WaitForSeconds(0.5f);

				operation.allowSceneActivation = true;
			}

			yield return null;
		}
	}

}
