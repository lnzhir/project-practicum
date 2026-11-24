using UnityEngine;

[CreateAssetMenu(fileName = "NewMissileData", menuName = "Missile Data")]
public class MissileData : ScriptableObject {
	[SerializeField]
	private GameObject explosionEffectPrefab;
	[SerializeField]
	private GameObject trailPrefab;
	[SerializeField]
	private float thrustForse = 1000f;
	[SerializeField]
	private float thrustTime = 1f; // в секундах
	[SerializeField]
	private float thrustDelay = 0f;
	[SerializeField]
	private float forceDown = 0f;
	[SerializeField]
	private int maxDistance = 2000;
	[SerializeField]
	private float explosionRadius = 10;
	[SerializeField]
	private float explosionForce = 1000;
	[SerializeField]
	private float damage = 1f;

	public GameObject ExplosionEffectPrefab { get => explosionEffectPrefab; }
	public GameObject TrailPrefab { get => trailPrefab; } 
	public float Damage { get => damage; }
	public float ThrustForse { get => thrustForse; }
	public float ThrustTime { get => thrustTime; } // в секундах
	public float ThrustDelay { get => thrustDelay; }
	public float ForceDown { get => forceDown; }
	public int MaxDistance { get => maxDistance; }
	public float ExplosionRadius { get => explosionRadius; }
	public float ExplosionForce { get => explosionForce; }
}
