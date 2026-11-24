using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;




public enum GlobalGameState
{
	MainMenu,
	InGame,
	Loading,
}

public enum GameScreenState
{
	PlayerHUD,
	SettingsScreen,
	PauseMenuScreen,
}

public enum MainMenuState
{
	MainMenuScreen,
	LevelsMenuScreen,
	SettingsScreen,
}

public enum BattleResultType
{
	Win = 0,
	Defeat = 1,
}


public class GameSettings : INotifyBindablePropertyChanged
{
	public class PlayerSettings
	{
		public int QualityLevel { get; set; } = QualitySettings.GetQualityLevel();
		public float Sensitivity { get; set; } = 1f;
	}

	public static GameSettings Instance { get; private set; } = null!;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void RuntimeInitializeOnLoad() => Instance = new GameSettings();

	public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;
		
	void Notify([CallerMemberName] string property = "") =>
		propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));

	public event Action<bool> pauseChanged;
	public event Action<GlobalGameState> gameStateChanged;
	public event Action<GameScreenState> gameScreenStateChanged;
	public event Action<MainMenuState> mainMenuStateChanged;

	GlobalGameState m_GameState;
	public GlobalGameState GameState
	{
		get => m_GameState;
		set
		{
			if (m_GameState == value)
			{
				return;
			}

			m_GameState = value;

			Notify(MainScreenStylePropertyName);
			Notify(MainMenuStylePropertyName);
			Notify(LevelsMenuStylePropertyName);
			Notify(PauseMenuStylePropertyName);
			Notify(PlayerHUDStylePropertyName);
			Notify(BattleResultStylePropertyName);
			Notify(SplashScreenStylePropertyName);

			gameStateChanged?.Invoke(value);
		}
	}

	GameScreenState m_GameScreenState;
	public GameScreenState GameScreenState
	{
		get => m_GameScreenState;
		set
		{
			if (m_GameScreenState == value)
			{
				return;
			}

			m_GameScreenState = value;

			Notify(LevelsMenuStylePropertyName);
			Notify(PauseMenuStylePropertyName);
			Notify(PlayerHUDStylePropertyName);
			Notify(SettingsStylePropertyName);

			gameScreenStateChanged?.Invoke(value);
		}
	}


	MainMenuState m_MainMenuState;
	public MainMenuState MainMenuState
	{
		get => m_MainMenuState;
		set
		{
			if (m_MainMenuState == value)
			{
				return;
			}

			m_MainMenuState = value;

			Notify(MainMenuStylePropertyName);
			Notify(LevelsMenuStylePropertyName);
			Notify(SettingsStylePropertyName);

			mainMenuStateChanged?.Invoke(value);
		}
	}

	bool m_IsPauseMenuOpen = false;
	public bool IsPauseMenuOpen
	{
		get => m_IsPauseMenuOpen;
		set
		{
			if (m_IsPauseMenuOpen == value)
			{
				return;
			}

			m_IsPauseMenuOpen = value;

			Notify(PlayerHUDStylePropertyName);
			Notify(MainScreenStylePropertyName);
			Notify(PauseMenuStylePropertyName);

			pauseChanged?.Invoke(value);
		}
	}

	bool m_IsFinished = false;
	public bool IsFinished
	{
		get => m_IsFinished;
		set
		{
			if (m_IsFinished == value)
			{
				return;
			}

			m_IsFinished = value;

			Notify(PauseMenuStylePropertyName);
			Notify(BattleResultStylePropertyName);
			Notify(PlayerHUDStylePropertyName);
		}
	}

	int m_planeKills = 0;
	public int PlaneKills
	{
		get => m_planeKills;
		set
		{
			m_planeKills = value;
			Notify(PlaneKillsPropertyName);
		}
	}

	int m_score = 0;
	public int Score
	{
		get => m_score;
		set
		{
			m_score = value;
			Notify(ScorePropertyName);
		}
	}

	int m_time = 0;
	public int Time
	{
		get => m_time;
		set
		{
			m_time = value;
			Notify(TimePropertyName);
		}
	}

	BattleResultType m_battleResult = 0;
	public BattleResultType BattleResult
	{
		get => m_battleResult;
		set
		{
			m_battleResult = value;
			Notify(BattleResultValuePropertyName);
		}
	}

	PlayerSettings m_playerSettings = new PlayerSettings();

	public PlayerSettings PlayerSettings_
	{
		get => m_playerSettings;
		set
		{
			m_playerSettings = value;
		}
	}
	
	public static readonly string MainScreenStylePropertyName = nameof(MainScreenStyle);
	[CreateProperty]
	DisplayStyle MainScreenStyle => m_GameState == GlobalGameState.MainMenu || (m_GameState == GlobalGameState.InGame && m_IsPauseMenuOpen) ? 
									DisplayStyle.Flex : DisplayStyle.None;
	public static readonly string MainMenuStylePropertyName = nameof(MainMenuStyle);
	[CreateProperty]
	DisplayStyle MainMenuStyle => m_GameState == GlobalGameState.MainMenu && 
									MainMenuState == MainMenuState.MainMenuScreen ? 
									DisplayStyle.Flex : DisplayStyle.None;

	public static readonly string LevelsMenuStylePropertyName = nameof(LevelsMenuStyle);
	[CreateProperty]
	DisplayStyle LevelsMenuStyle => m_GameState == GlobalGameState.MainMenu && 
									MainMenuState == MainMenuState.LevelsMenuScreen ? 
									DisplayStyle.Flex : DisplayStyle.None;

	public static readonly string SettingsStylePropertyName = nameof(SettingsStyle);
	[CreateProperty]
	DisplayStyle SettingsStyle => m_GameState == GlobalGameState.MainMenu && 
									MainMenuState == MainMenuState.SettingsScreen ? 
									DisplayStyle.Flex : DisplayStyle.None;

	public static readonly string PauseMenuStylePropertyName = nameof(PauseMenuStyle);
	[CreateProperty]
	DisplayStyle PauseMenuStyle => m_GameState == GlobalGameState.InGame && m_IsPauseMenuOpen && !m_IsFinished ? 
									DisplayStyle.Flex : DisplayStyle.None;

	public static readonly string BattleResultStylePropertyName = nameof(BattleResultStyle);
	[CreateProperty]
	DisplayStyle BattleResultStyle => m_GameState == GlobalGameState.InGame && 
									m_IsFinished ? 
									DisplayStyle.Flex : DisplayStyle.None;

	public static readonly string PlayerHUDStylePropertyName = nameof(PlayerHUDStyle);
	[CreateProperty]
	DisplayStyle PlayerHUDStyle => m_GameState == GlobalGameState.InGame && !m_IsFinished && !m_IsPauseMenuOpen ? 
									DisplayStyle.Flex : DisplayStyle.None;

	public static readonly string SplashScreenStylePropertyName = nameof(SplashScreenStyle);
	[CreateProperty]
	DisplayStyle SplashScreenStyle => m_GameState == GlobalGameState.Loading ? 
									DisplayStyle.Flex : DisplayStyle.None;


	public static readonly string PlaneKillsPropertyName = nameof(PlaneKillsProperty);
	[CreateProperty]
	int PlaneKillsProperty => m_planeKills;

	public static readonly string ScorePropertyName = nameof(ScoreProperty);
	[CreateProperty]
	int ScoreProperty => m_score;

	public static readonly string TimePropertyName = nameof(TimeProperty);
	[CreateProperty]
	int TimeProperty => m_time;

	public static readonly string BattleResultValuePropertyName = nameof(BattleResultValueProperty);
	[CreateProperty]
	BattleResultType BattleResultValueProperty => m_battleResult;



}
