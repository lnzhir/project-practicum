using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

public partial class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField]
	public string mainSceneName = "main";
	[SerializeField]
	public string menuSceneName = "menuScene";
	public event Action levelLoaded;
	public event Action levelUnloads;
	
	LevelData currentLevel = null;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		GameSettings.Instance.GameState = GlobalGameState.Loading;
		
		LoadMenuScene();
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private async void LoadMenuScene()
	{
		await SceneManager.LoadSceneAsync(menuSceneName, LoadSceneMode.Additive);

		GameSettings.Instance.GameState = GlobalGameState.MainMenu;
		GameSettings.Instance.MainMenuState = MainMenuState.MainMenuScreen;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (currentLevel != null && currentLevel.LevelSceneName == scene.name)
		{
			levelLoaded?.Invoke();
		}
	}

	private async Task UnLoadLevel_() {
		if (currentLevel != null)
		{
			if (SceneManager.GetSceneByName(currentLevel.MapSceneName).isLoaded)
			{
				await SceneManager.UnloadSceneAsync(currentLevel.MapSceneName);
			}

			if (SceneManager.GetSceneByName(currentLevel.LevelSceneName).isLoaded)
			{
				await SceneManager.UnloadSceneAsync(currentLevel.LevelSceneName);
			}
		}
	}

	public async void UnLoadLevel()
	{
		levelUnloads?.Invoke();

		GameSettings.Instance.GameState = GlobalGameState.Loading;

		await UnLoadLevel_();
		await SceneManager.LoadSceneAsync(menuSceneName, LoadSceneMode.Additive);

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainSceneName));
		//MainCameraSingleton.Instance.enabled = true;
		GameSettings.Instance.GameState = GlobalGameState.MainMenu;
		GameSettings.Instance.MainMenuState = MainMenuState.LevelsMenuScreen;
	}

	public async void LoadLevel(LevelData levelData)
	{
		GameSettings.Instance.GameState = GlobalGameState.Loading;

		await UnLoadLevel_();

		Scene menuScene = SceneManager.GetSceneByName(menuSceneName);
		if (menuScene.IsValid())
		{
			await SceneManager.UnloadSceneAsync(menuScene);
		}
		

		currentLevel = levelData;
		await SceneManager.LoadSceneAsync(levelData.LevelSceneName, LoadSceneMode.Additive);
		await SceneManager.LoadSceneAsync(levelData.MapSceneName, LoadSceneMode.Additive);

		//MainCameraSingleton.Instance.enabled = false;

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelData.LevelSceneName));
		GameSettings.Instance.GameState = GlobalGameState.InGame;
		GameSettings.Instance.IsPauseMenuOpen = false;
		GameSettings.Instance.IsFinished = false;
	}

	public void Pause()
	{
		Time.timeScale = 0.0f;
	}

	public void Resume()
	{
		Time.timeScale = 1.0f;
	}
}
