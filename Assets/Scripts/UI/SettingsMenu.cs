using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
	string[] qualityNames =
	{
		"Быстрейшее",
		"Быстрое",
		"Обычное",
		"Хорошое",
		"Красивое",
		"Наилучшее"
	};

	static class UIElementNames
	{
		public const string DefaultButton = "defaultButton";
		public const string BackButton = "backButton";
		public const string QualityDropdown = "qualityDropdown";
		public const string SensitivityField = "sensitivityField";
	}

	[SerializeField]  
	private VisualTreeAsset uxml;

	VisualElement m_visualTree;
	Button m_defaultButton;
	Button m_backButton;
	DropdownField m_qualityDropdown;
	FloatField m_sensitivityField;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void OnEnable()
	{
		//m_visualTree = GetComponent<UIDocument>().rootVisualElement;
		m_visualTree = uxml.CloneTree();

		m_visualTree.SetBinding("style.display", new DataBinding
		{
			dataSource = GameSettings.Instance,
			dataSourcePath = new PropertyPath(GameSettings.SettingsStylePropertyName),
			bindingMode = BindingMode.ToTarget,
		});

		m_backButton = m_visualTree.Q<Button>(UIElementNames.BackButton);
		m_backButton.clicked += OnBackClicked;

		m_defaultButton = m_visualTree.Q<Button>(UIElementNames.DefaultButton);
		m_defaultButton.clicked += OnDefaultClicked;

		// m_qualityDropdown = m_visualTree.Q<DropdownField>(UIElementNames.QualityDropdown);
		// m_qualityDropdown.RegisterValueChangedCallback(OnQualitySelected);
		// m_qualityDropdown.choices = new List<string>(qualityNames);
		// m_qualityDropdown.index = QualitySettings.GetQualityLevel();

		// m_sensitivityField = m_visualTree.Q<FloatField>(UIElementNames.SensitivityField);
		// m_sensitivityField.RegisterValueChangedCallback(OnSensitivityChanged);
	}

	void Start()
	{
		MainScreen mainScreen = GetComponent<MainScreen>();
		mainScreen.AddChild(m_visualTree);
	}

	void OnBackClicked()
	{
		GameSettings.Instance.MainMenuState = MainMenuState.MainMenuScreen;
	}

	void OnDefaultClicked()
	{
		GameSettings.Instance.PlayerSettings_ = new GameSettings.PlayerSettings();
	}

	void OnQualitySelected(ChangeEvent<string> evt)
	{
		GameSettings.Instance.PlayerSettings_.QualityLevel = m_qualityDropdown.index;
		QualitySettings.SetQualityLevel(m_qualityDropdown.index, false);
	}

	void OnSensitivityChanged(ChangeEvent<float> evt)
	{
		GameSettings.Instance.PlayerSettings_.Sensitivity = evt.newValue;
	}
}
