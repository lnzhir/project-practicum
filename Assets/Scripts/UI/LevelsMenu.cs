using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

public class LevelsMenu : MonoBehaviour
{
	static class UIElementNames
	{
		public const string Maps = "maps";
		public const string BackButton = "backButton";
		public const string StartButton = "startButton";
		public const string HostsButton = "hostsButton";

		public const string Caption = "caption";
	}

	[SerializeField]  
	private VisualTreeAsset uxml;
	[SerializeField]  
	private VisualTreeAsset uxmlMaps;
	[SerializeField]
	private List<LevelData> levels;
	private LevelData currentLevel;

	VisualElement m_visualTree;
	VisualElement m_maps;
	Button m_backButton;
	Button m_startButton;
	Button m_hostsButton;

	void OnEnable()
	{
		//m_visualTree = GetComponent<UIDocument>().rootVisualElement;
		m_visualTree = uxml.CloneTree();

		m_visualTree.SetBinding("style.display", new DataBinding
		{
			dataSource = GameSettings.Instance,
			dataSourcePath = new PropertyPath(GameSettings.LevelsMenuStylePropertyName),
			bindingMode = BindingMode.ToTarget,
		});

		m_backButton = m_visualTree.Q<Button>(UIElementNames.BackButton);
		m_backButton.clicked += OnPrevPressed;

		m_startButton = m_visualTree.Q<Button>(UIElementNames.StartButton);
		m_startButton.clicked += OnStartPressed;

		m_hostsButton = m_visualTree.Q<Button>(UIElementNames.HostsButton);
		m_hostsButton.clicked += OnHostsPressed;

		m_maps = m_visualTree.Q<VisualElement>(UIElementNames.Maps);

		currentLevel = levels[0];
		foreach (LevelData levelData in levels)
		{
			VisualElement mapCard = uxmlMaps.CloneTree();

			Label captionLabel = mapCard.Q<Label>(UIElementNames.Caption);
			captionLabel.text = levelData.Title;

			mapCard.RegisterCallback<ClickEvent>(evt =>
			{
				currentLevel = levelData;
			});
			
			m_maps.Add(mapCard);
		}
	}

	void Start()
	{
		MainScreen mainScreen = GetComponent<MainScreen>();
		mainScreen.AddChild(m_visualTree);
	}

	void OnPrevPressed()
	{
		GameSettings.Instance.MainMenuState = MainMenuState.MainMenuScreen;
	}

	void OnStartPressed()
	{
		GameManager.Instance.LoadLevel(currentLevel);
	}

	void OnHostsPressed()
	{
		
	}
}
