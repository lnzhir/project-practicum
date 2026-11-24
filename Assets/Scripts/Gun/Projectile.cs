using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : Bullet {
	
	public float StartVelocity { get; set; }
	public float MaxDistance { get; set; } = 10000.0f;

	[SerializeField]
	private GameObject hitEffectPrefab;
	[SerializeField] 
	private LayerMask targetLayers;
	[SerializeField] 
	private LayerMask ignoreLayers;

	/*public float startForce = 1.0f;
	public float maxDistance = 10000.0f;
	public int damage = 1;
	public GameObject hitEffectPrefab;*/

	private Rigidbody rb;
	private Vector3 spawnPos;


	void Awake()
	{
		spawnPos = transform.position;

		rb = GetComponent<Rigidbody> ();
	}

    // Use this for initialization
   void Start () {
		rb.AddForce (transform.forward * StartVelocity, ForceMode.VelocityChange);

		if (ParentInfo.Rigidbody)
		{
			rb.linearVelocity += ParentInfo.Rigidbody.linearVelocity;
		}
	}

	void FixedUpdate() {
		if (Vector3.Distance (spawnPos, transform.position) > MaxDistance) {
			Destroy(gameObject);
		}

		float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;

		// проверка столкновения
		if (Physics.Raycast(
			transform.position, 
			rb.linearVelocity.normalized, 
			out RaycastHit hit, 
			distance,
			~ignoreLayers)
		) {
			//hit.collider.enabled = true;

			if (hitEffectPrefab)
			{
				GameObject effect = Instantiate (
					hitEffectPrefab, 
					hit.point,
					Quaternion.LookRotation(-hit.normal)
				);
			}

			// проверяем объект - целевой, если да, то смотрим разрушаемый ли он
			if ((targetLayers & (1 << hit.collider.gameObject.layer)) != 0)
			{
				Destructable destructable = hit.collider.gameObject.GetComponent<Destructable>();
				if (destructable)
				{
					destructable.Hit(ParentInfo.Owning, Damage);
				}
			}

			//Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);

			Destroy(gameObject);
		}
	}
}


