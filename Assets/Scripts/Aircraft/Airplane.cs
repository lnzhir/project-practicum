using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Airplane : MonoBehaviour
{
	[SerializeField]
	int seed = 0;
	[SerializeField]
	int bodyType = 0;
	[SerializeField]
	float surfaceSpeed = 1.5f;
	[SerializeField]
	GameObject trailPrefab;
	[SerializeField]
   GameObject enginePrefab;
	[SerializeField]
   GameObject cannonPrefab;
	[SerializeField]
   GameObject missilePrefab;
	[SerializeField]
	List<AeroSurface> controlSurfaces = null;

	AircraftPhysics aircraftPhysics;
	Rigidbody rb;
	LargeLaser.Plane currentPlane;
	MissileLauncher missileLauncher;
	List<GameObject> engineEffects = new List<GameObject>();

	[Range(0.0f, 1.0f)]
	float thrustPercent = 0.0f;
	public float ThrustPercent { 
		get => thrustPercent; 
		set
		{
			thrustPercent = Mathf.Clamp01(value);
			aircraftPhysics.SetThrustPercent(thrustPercent);

			Vector3 scaleFactor = Vector3.one * thrustPercent;
			foreach (var engineTrail in engineEffects)
			{
				Utils.ScaleTree(engineTrail.transform, scaleFactor);
			}
		}
	}
	private readonly Dictionary<AeroSurface, float> surfaceAngles = new();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
	{
		aircraftPhysics = GetComponent<AircraftPhysics>();
		rb = GetComponent<Rigidbody>();
		missileLauncher = GetComponent<MissileLauncher>();
		
		currentPlane = LargeLaser.Plane.Create(new LargeLaser.PlaneInit()
        {
			Seed = seed,
			BodyConfiguration = (LargeLaser.Plane.BodyConfiguration)bodyType
        });
		currentPlane.transform.SetParent(transform);
		currentPlane.transform.localPosition = Vector3.zero;
		currentPlane.transform.localRotation = Quaternion.identity;

		foreach (var surface in controlSurfaces)
		{
			surfaceAngles[surface] = 0;
		}

		foreach(var wingTipLocator in currentPlane.WingTipLocators)
		{
			var trail = GameObject.Instantiate(trailPrefab, wingTipLocator.transform);
			trail.transform.localPosition = Vector3.zero;
		}

		// add jet streams to the engines.
		foreach(var engineLocator in currentPlane.EngineLocators)
		{
			var trail = GameObject.Instantiate(enginePrefab, engineLocator.transform);
			trail.transform.localPosition = Vector3.zero;
			engineEffects.Add(trail);
		}

		foreach(var cannonLocator in currentPlane.CannonLocators)
		{
			var cannon = GameObject.Instantiate(cannonPrefab, cannonLocator.transform);
			cannon.transform.localPosition = Vector3.zero;
		}

		// генерация коллайдера

		MeshFilter meshFilter = GetComponentsInChildren<MeshFilter>().First(x => x.name == "Plane");
		var calc = new GK.ConvexHullCalculator();
		var verts = new List<Vector3>();
		var tris = new List<int>();
		var normals = new List<Vector3>();
		calc.GenerateHull(meshFilter.sharedMesh.vertices.ToList(), true, ref verts, ref tris, ref normals);

		var mesh = new Mesh();
		mesh.SetVertices(verts);
		mesh.SetTriangles(tris, 0);
		mesh.SetNormals(normals);

		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = true;
		meshCollider.sharedMesh = mesh;

		// инициализация ракет
		
		var missiles = GetComponentsInChildren<Transform>().Where(x => x.name == "Missile");
		foreach (var missileTransform in missiles)
		{
			InitMissile(missileTransform);
		}
	}

	private void InitMissile(Transform missileTransform)
	{
		Transform parent = missileTransform.parent;
		GameObject missileClone = Instantiate(missilePrefab, parent, false);
		missileClone.transform.position = missileTransform.position;

		Missile missile = missileClone.GetComponent<Missile>();
		missileLauncher.Missiles.Add(missile);
		
		missileTransform.parent = missileClone.transform;
	}

	public void FireCannon()
	{
		foreach (var cannon in GetComponentsInChildren<Cannon>())
		{
			cannon.Fire();
		}
	}

	public void FireMissile()
	{
		// Transform missileTransform = GetComponentsInChildren<Transform>().First(x => x.name == "Missile");
		// missileTransform.parent = null;

		// GameObject missileClone = Instantiate(missilePrefab, missileTransform);
		// Missile missile = missileClone.GetComponent<Missile>();
		// missile.Launch();

		missileLauncher.Fire();
	}

	public void SetControlSurfecesAngles(float pitch, float roll, float yaw, float flap)
	{
		foreach (var surface in controlSurfaces)
		{
			if (surface == null || !surface.IsControlSurface)
			{
				continue;
			}

			float input = surface.InputType switch
			{
				ControlInputType.Pitch => pitch,
				ControlInputType.Roll  => roll,
				ControlInputType.Yaw   => yaw,
				ControlInputType.Flap  => flap,
				_ => 0f
			};

			float targetAngle = input * surface.InputMultiplyer;

			if (!surfaceAngles.TryGetValue(surface, out float currentAngle))
			{
				currentAngle = 0f;
			}

			currentAngle = Mathf.MoveTowards(
				currentAngle,
				targetAngle,
				surfaceSpeed * Time.fixedDeltaTime
			);

			surfaceAngles[surface] = currentAngle;
			surface.SetFlapAngle(currentAngle);
		}
	}

	 // Update is called once per frame
	void FixedUpdate()
	{
		
	}
}
