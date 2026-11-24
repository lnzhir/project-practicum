using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

public class SplashScreen : MonoBehaviour
{
	static class UIElementNames
	{
		public const string Spinner = "spinner";
	}

	public float rotateSpeed = 1.0f;

	VisualElement m_visualTree;
	VisualElement m_spinner;
	Angle spinnerAngle = 0;

	void Start()
	{
		GameSettings.Instance.gameStateChanged += GameStateChanged;
	}

	void GameStateChanged(GlobalGameState state)
	{
		enabled = state == GlobalGameState.Loading;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void OnEnable()
	{
		m_visualTree = GetComponent<UIDocument>().rootVisualElement;

		m_visualTree.ClearBindings();
		m_visualTree.SetBinding("style.display", new DataBinding
		{
			dataSource = GameSettings.Instance,
			dataSourcePath = new PropertyPath(GameSettings.SplashScreenStylePropertyName),
			bindingMode = BindingMode.ToTarget,
		});


		m_spinner = m_visualTree.Q<VisualElement>(UIElementNames.Spinner);
	}

	// Update is called once per frame
	void Update()
	{
		if (GameSettings.Instance.GameState == GlobalGameState.Loading)
		{
			spinnerAngle.value += rotateSpeed * Time.deltaTime;

			m_spinner.style.rotate = new Rotate(spinnerAngle);
		}
		
	}
}
