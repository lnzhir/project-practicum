using UnityEngine;

[CreateAssetMenu(fileName = "NewCannonData", menuName = "Cannon Data")]
public class CannonData : ScriptableObject {
	[SerializeField]
	private GameObject bulletPrefab;
	[SerializeField]
	private GameObject muzzleEffectPrefab;
	[SerializeField]
	private float spread = 1.0f;
	[SerializeField]
	private float startVelocity = 500.0f;

	[SerializeField]
	private float fireDelay = 0.1f; // in seconds
	[SerializeField]
	private float damage = 1f;
	[SerializeField]
	private float maxDistance = 2000.0f;

	public GameObject BulletPrefab { get { return bulletPrefab; } }
	public GameObject MuzzleEffectPrefab { get { return muzzleEffectPrefab; } } 
	public float Spread { get { return spread; } }
	public float StartVelocity { get { return startVelocity; } }

	public float FireDelay { get { return fireDelay; } }
	public float Damage { get { return damage; } }
	public float MaxDistance { get { return maxDistance; } }
}
