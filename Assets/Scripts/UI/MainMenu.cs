using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;


[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour
{
	static class UIElementNames
	{
		public const string ExitButton = "exitButton";
		public const string StartButton = "startButton";
		public const string SettingsButton = "settingsButton";
	}

	[SerializeField]  
	private VisualTreeAsset uxml;

	VisualElement m_visualTree;
	Button m_exitButton;
	Button m_startButton;
	Button m_settinsButton;

	void OnEnable()
	{
		//m_visualTree = GetComponent<UIDocument>().rootVisualElement;
		m_visualTree = uxml.CloneTree();

		m_visualTree.SetBinding("style.display", new DataBinding
		{
			dataSource = GameSettings.Instance,
			dataSourcePath = new PropertyPath(GameSettings.MainMenuStylePropertyName),
			bindingMode = BindingMode.ToTarget,
		});

		m_startButton = m_visualTree.Q<Button>(UIElementNames.StartButton);
		m_startButton.clicked += OnStartPressed;

		m_settinsButton = m_visualTree.Q<Button>(UIElementNames.SettingsButton);
		m_settinsButton.clicked += OnSettingsPressed;

		m_exitButton = m_visualTree.Q<Button>(UIElementNames.ExitButton);
		m_exitButton.clicked += OnExitPressed;

		
	}

	void Start()
	{
		MainScreen mainScreen = GetComponent<MainScreen>();
		mainScreen.AddChild(m_visualTree);
	}

	static void OnStartPressed()
	{
		GameSettings.Instance.MainMenuState = MainMenuState.LevelsMenuScreen;
	}

	static void OnSettingsPressed()
	{
		GameSettings.Instance.MainMenuState = MainMenuState.SettingsScreen;
	}

	static void OnExitPressed()
	{
		Application.Quit();
	}
}
