using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;
using System.Collections.Generic;
using System.Linq;


public class PlayerHUD : MonoBehaviour
{
	static class UIElementNames
	{
		public const string Circle = "circle";
		public const string Markers = "markers";
		public const string FPS = "fps";
	}

	[SerializeField]
	private float maxCrossDistance = 3000f;
	[SerializeField]
	private float crossMarkerSize = 1;
	[SerializeField] 
	private VectorImage crossAsset;
	[SerializeField]
	private Vector3 markerPlaneOffset = Vector3.zero;
	[SerializeField]
	private Color markerFriendColor = Color.blue;
	[SerializeField]
	private Color markerEnemyColor = Color.red;

	VisualElement m_visualTree;
	VisualElement m_markers;
	Label m_fpsLabel;
	private Dictionary<GameObject, VisualElement> planeMarkers = new Dictionary<GameObject, VisualElement>();
	VisualElement crossMarker;

	float fps = 0;
	bool updatedFPS = false;

	void Start()
	{
		GameManager.Instance.levelLoaded += LevelLoaded;
		GameManager.Instance.levelUnloads += LevelUnloads;
		GameSettings.Instance.pauseChanged += PauseChanged;
		GameSettings.Instance.gameStateChanged += GameStateChanged;
	}

	void PauseChanged(bool state)
	{
		enabled = !state;

		if (enabled && GameSettings.Instance.GameState == GlobalGameState.InGame)
		{
			HideCursor();
		}
	}

	void GameStateChanged(GlobalGameState state)
	{
		if (state == GlobalGameState.InGame)
		{
			HideCursor();
		}
	}

	void HideCursor()
	{
		UnityEngine.Cursor.visible = false;
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
	}

	void LevelLoaded()
	{
		BattleManager.Instance.destructed += PlaneDestructed;
		BattleManager.Instance.planesSpawned += PlanesSpawned;
	}

	void LevelUnloads()
	{
		planeMarkers.Clear();
		m_markers.Clear();
	}

	void PlaneDestructed(BattleManager.AirplaneInfo planeObject)
	{
		planeMarkers.Remove(planeObject.GameObject);
	}

	void PlanesSpawned(Dictionary<GameObject, BattleManager.AirplaneInfo> planes)
	{
		// создание маркеров
		foreach (var (key, value) in planes)
		{
			if (key != BattleManager.Instance.HostPlane.GameObject)
			{
				Label marker = new Label(value.Owning.Player.Name);
				marker.style.position = Position.Absolute;
				marker.style.width = StyleKeyword.Auto;
				marker.style.height = StyleKeyword.Auto;
				marker.style.whiteSpace = WhiteSpace.NoWrap;
				marker.style.color = value.Owning.Team == BattleManager.Instance.HostPlane.Owning.Team ? markerFriendColor : markerEnemyColor;
				marker.pickingMode = PickingMode.Ignore;
				m_markers.Add(marker);

				planeMarkers[key] = marker;
			}
		}

		crossMarker = new Image
		{
			vectorImage = crossAsset,
			scaleMode = ScaleMode.ScaleToFit
		};
		crossMarker.style.position = Position.Absolute;
		crossMarker.style.width = Length.Percent(crossMarkerSize);
		crossMarker.style.height = Length.Percent(crossMarkerSize);
		m_markers.Add(crossMarker);
	}

	void OnEnable()
	{
		m_visualTree = GetComponent<UIDocument>().rootVisualElement;

		m_visualTree.ClearBindings();
		m_visualTree.SetBinding("style.display", new DataBinding
		{
			dataSource = GameSettings.Instance,
			dataSourcePath = new PropertyPath(GameSettings.PlayerHUDStylePropertyName),
			bindingMode = BindingMode.ToTarget,
		});

		m_markers = m_visualTree.Q<VisualElement>(UIElementNames.Markers);

		m_fpsLabel = m_visualTree.Q<Label>(UIElementNames.FPS);


		
	}

	void OnDisable()
	{
		UnityEngine.Cursor.visible = true;
		UnityEngine.Cursor.lockState = CursorLockMode.None;
	}

	void SetPosMarker(VisualElement marker, Vector3 position)
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint(position);

		if (screenPos.z <= 0)
		{
			marker.style.display = DisplayStyle.None;
			return;
		}

		marker.style.display = DisplayStyle.Flex;

		Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(
			marker.panel,
			new Vector2(screenPos.x, screenPos.y)
		);

		Vector2 localPos = marker.parent.WorldToLocal(
			new Vector2(panelPos.x, marker.panel.visualTree.layout.height - panelPos.y)
		);

		Vector2 markerSize = marker.layout.size;

		marker.style.left = localPos.x - markerSize.x * 0.5f;
		marker.style.top  = localPos.y - markerSize.y * 0.5f;
	}

	void LateUpdate()
	{
		foreach (var (planeObject, marker) in planeMarkers)  // раскладываем объект на свойства
		{
			SetPosMarker(marker, planeObject.transform.position + markerPlaneOffset);
		}
		
		if (crossMarker != null)
		{
			SetPosMarker(
				crossMarker, 
				BattleManager.Instance.HostPlane.GameObject.transform.position + BattleManager.Instance.HostPlane.GameObject.transform.forward * maxCrossDistance);
		}
		
	}

	void Update()
	{
		if (!updatedFPS)
		{
			updatedFPS = true;
			Invoke("UpdateFPS", 1f);
		}
	}

	void UpdateFPS()
	{
		fps = Mathf.Round(1f / Time.unscaledDeltaTime);
		m_fpsLabel.text = fps.ToString();

		updatedFPS = false;
	}
}
