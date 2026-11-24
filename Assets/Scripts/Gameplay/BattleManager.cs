using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
	public struct Team
	{
		public List<Player> Players { get; set; }
		public Dictionary<GameObject, AirplaneInfo> Airplanes { get; set; }
	}

	public struct AirplaneInfo
	{
		public GameObject GameObject { get; set; }
		public Owning Owning { get; set; }
	}

	[SerializeField]
	private GameObject planePrefab;
	[SerializeField]
	private AirplaneControllerData airplaneControllerData;
	[SerializeField]
	private Transform spawnPoint1;
	[SerializeField]
	private Transform spawnPoint2;
	[SerializeField]
	private float startForce = 100000f;

	public AirplaneInfo HostPlane { get; set; }
	public Player HostPlayer { get; set; }
	public static BattleManager Instance { get; set; }
	public List<Player> Players { get; set; } = new List<Player>();
	public Dictionary<GameObject, AirplaneInfo> Airplanes { get; set; } = new Dictionary<GameObject, AirplaneInfo>();
	public Team team1 = new Team();
	public Team team2 = new Team();
	public event Action<AirplaneInfo> destructed;
	public event Action<Dictionary<GameObject, AirplaneInfo>> planesSpawned;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		HostPlayer = new Player() { Id = 1, Name = "player" };
		Players.Add(HostPlayer);
		Players.AddRange(new List<Player>() {
			new Player() { Id = 2, Name = "player1" },
			new Player() { Id = 3, Name = "player2" },
			new Player() { Id = 4, Name = "player3" },
			new Player() { Id = 5, Name = "player4" },
			new Player() { Id = 6, Name = "player5" },
			new Player() { Id = 7, Name = "player6" },
			new Player() { Id = 8, Name = "player7" },
			new Player() { Id = 9, Name = "player8" },
			new Player() { Id = 10, Name = "player9" },
			new Player() { Id = 11, Name = "player10" },
			new Player() { Id = 12, Name = "player11" },
			new Player() { Id = 13, Name = "player12" },
			new Player() { Id = 14, Name = "player13" },
			new Player() { Id = 15, Name = "player14" },
			new Player() { Id = 16, Name = "player15" },
		});

		SpawnPlayers();
	}

	void PlaneDestructed(GameObject planeObject)
	{
		destructed?.Invoke(Airplanes[planeObject]);

		//team1.Airplanes.Remove(planeObject);
		//team2.Airplanes.Remove(planeObject);

		Airplanes.Remove(planeObject);
	}

	void SpawnTeam(List<Player> players, int teamId, Transform spawnPoint, Dictionary<GameObject, AirplaneInfo> result)
	{
		//Dictionary<GameObject, AirplaneInfo> result = new Dictionary<GameObject, AirplaneInfo>();

		foreach (var player in players)
		{
			GameObject planeObject = Instantiate(planePrefab);
			var collider = planeObject.GetComponentInChildren<Collider>();

			float sphereRadius = collider == null ? 1 : collider.bounds.extents.magnitude;
			do
			{
				planeObject.transform.position = spawnPoint.position + UnityEngine.Random.insideUnitSphere * players.Count * sphereRadius;
			} while(Physics.OverlapSphere(planeObject.transform.position, sphereRadius).Count(c => c != collider) > 0);
			

			Destructable destructable = planeObject.GetComponent<Destructable>();
			if (destructable != null)
			{
				destructable.destructed += PlaneDestructed;
			}

			Airplane airplane = planeObject.GetComponent<Airplane>();
			if (airplane != null)
			{
				airplane.ThrustPercent = 1f;
			}

			Rigidbody rigidbody = planeObject.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.AddForce(planeObject.transform.forward * startForce, ForceMode.Impulse);
			}

			Owning owning = planeObject.AddComponent<Owning>();
			owning.Player = player;
			owning.Team = teamId;

			AirplaneInfo airplaneInfo = new AirplaneInfo { GameObject = planeObject, Owning = owning };

			if (player == HostPlayer)
			{
				HostPlane = airplaneInfo;
				var controller = planeObject.AddComponent<AirplaneController>();
				controller.ControllerData = airplaneControllerData;
				
				CameraOrbit.Instance.Target = planeObject.transform;
			}

			result[planeObject] = airplaneInfo;
		}

		//return result;
	}

	public void SpawnPlayers()
	{
		int halfCount = Players.Count / 2;
		team1.Players = Players.GetRange(0, halfCount);
		team2.Players = Players.GetRange(halfCount, Players.Count - halfCount);

		//team1.Airplanes = SpawnTeam(team1.Players, 1, spawnPoint1);
		//team2.Airplanes = SpawnTeam(team2.Players, 2, spawnPoint2);
		//team1.Airplanes = new Dictionary<GameObject, AirplaneInfo>();
		//team2.Airplanes = new Dictionary<GameObject, AirplaneInfo>();

		SpawnTeam(team1.Players, 1, spawnPoint1, Airplanes);
		SpawnTeam(team2.Players, 2, spawnPoint2, Airplanes);

		//planesSpawned?.Invoke(team1.Airplanes.Concat(team2.Airplanes));
		planesSpawned?.Invoke(Airplanes);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnDestroy()
	{
		planesSpawned = null;
		destructed = null;
	}
}
