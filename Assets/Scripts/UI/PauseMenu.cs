using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;


[RequireComponent(typeof(UIDocument))]
public class PauseMenu : MonoBehaviour
{
	static class UIElementNames
	{
		public const string ResumeButton = "resumeButton";
		public const string SettingsButton = "settingsButton";
		public const string ExitButton = "exitButton";
	}

	[SerializeField]  
	private VisualTreeAsset uxml;

	VisualElement m_visualTree;
	Button m_resumeButton;
	Button m_settingsButton;
	Button m_exitButton;

	void OnEnable()
	{
		//m_visualTree = GetComponent<UIDocument>().rootVisualElement;
		m_visualTree = uxml.CloneTree();

		m_visualTree.SetBinding("style.display", new DataBinding
		{
			dataSource = GameSettings.Instance,
			dataSourcePath = new PropertyPath(GameSettings.PauseMenuStylePropertyName),
			bindingMode = BindingMode.ToTarget,
		});

		m_resumeButton = m_visualTree.Q<Button>(UIElementNames.ResumeButton);
		m_resumeButton.clicked += OnResumePressed;

		m_settingsButton = m_visualTree.Q<Button>(UIElementNames.SettingsButton);
		m_settingsButton.clicked += OnSettingsPressed;

		m_exitButton = m_visualTree.Q<Button>(UIElementNames.ExitButton);
		m_exitButton.clicked += OnExitPressed;
	}

	void Start()
	{
		MainScreen mainScreen = GetComponent<MainScreen>();
		mainScreen.AddChild(m_visualTree);
	}

	static void OnResumePressed()
	{
		GameSettings.Instance.IsPauseMenuOpen = false;
	}

	static void OnSettingsPressed()
	{
		GameSettings.Instance.MainMenuState = MainMenuState.SettingsScreen;
		GameSettings.Instance.GameScreenState = GameScreenState.SettingsScreen;
	}

	static void OnExitPressed()
	{
		GameManager.Instance.UnLoadLevel();
	}
}
