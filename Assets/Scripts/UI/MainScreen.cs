using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using System.Collections.Generic;

public class MainScreen : MonoBehaviour
{
	static class UIElementNames
	{
		public const string Content = "content";
	}

	VisualElement m_visualTree;
	VisualElement content;

	public void AddChild(VisualElement visualElement)
	{
		content.Add(visualElement);
	}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
	void OnEnable()
	{
		m_visualTree = GetComponent<UIDocument>().rootVisualElement;

		m_visualTree.SetBinding("style.display", new DataBinding
		{
			dataSource = GameSettings.Instance,
			dataSourcePath = new PropertyPath(GameSettings.MainScreenStylePropertyName),
			bindingMode = BindingMode.ToTarget,
		});

		content = m_visualTree.Q<VisualElement>(UIElementNames.Content);
	}
}
