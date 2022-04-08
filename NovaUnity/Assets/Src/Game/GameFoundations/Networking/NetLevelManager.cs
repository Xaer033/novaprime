using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetLevelManager : NetworkSceneManagerBase
{ 

	public int _lobbyIndex;
	public int[] _levelList;
		
	private Scene _loadedScene;
	
	public void LoadLevel(int nextLevelIndex)
	{
		Runner.SetActiveScene(nextLevelIndex < 0 ? _lobbyIndex : _levelList[nextLevelIndex]);
	}

	protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
	{
		Debug.Log($"Switching Scene from {prevScene} to {newScene}");
		if (newScene <= 0)
		{
			finished?.Invoke(new List<NetworkObject>());
			yield break;
		}

		yield return null;
		Debug.Log($"Start loading scene {newScene} in single peer mode");

		if (_loadedScene != default)
		{
			Debug.Log($"Unloading Scene {_loadedScene.buildIndex}");
			yield return SceneManager.UnloadSceneAsync(_loadedScene);
		}

		_loadedScene = default;
		Debug.Log($"Loading scene {newScene}");

		List<NetworkObject> sceneObjects = new List<NetworkObject>();
		if (newScene >= 0)
		{
			yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
			
			_loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
			Debug.Log($"Loaded scene {newScene}: {_loadedScene}");
			sceneObjects = FindNetworkObjects(_loadedScene, disable: false);
		}

		// Delay one frame
		yield return null;
		
		// Activate the next level
		Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} scene objects");
		finished?.Invoke(sceneObjects);
	}
}
