using UnityEngine;

public class Missile : Bullet
{
	[SerializeField]
	private MissileData missileData;
	[SerializeField] 
	private LayerMask targetLayers;

	private Rigidbody rb;
	private bool launched = false;
	private Vector3 spawnPos;

	public override float Damage { get => missileData.Damage; set {} }

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Launch()
	{
		Invoke("_Launch", missileData.ThrustDelay);
	}

	public void _Launch()
	{
		spawnPos = transform.position;
		rb.isKinematic = false;
		launched = true;

		if (missileData.ForceDown > 0)
		{
			rb.AddRelativeForce(0, -missileData.ForceDown, 0);
		}

		if (ParentInfo.Rigidbody)
		{
			rb.linearVelocity += ParentInfo.Rigidbody.linearVelocity;
		}

		Invoke("UnLaunch", missileData.ThrustTime);

		var trail = Instantiate(missileData.TrailPrefab, transform);
		trail.transform.localPosition = Vector3.zero;
	}

	void UnLaunch()
	{
		launched = false;
	}

	void Detonate()
	{
		// эффект взрыва
		if (missileData.ExplosionEffectPrefab)
		{
			GameObject effect = Instantiate (
				missileData.ExplosionEffectPrefab, 
				transform.position,
				transform.rotation
			);
			
			foreach (Transform child in effect.GetComponentsInChildren<Transform>())
			{
				child.localScale *= missileData.ExplosionRadius;
			}
		}

		Debug.Log($"{missileData.ExplosionForce}, {transform.position}, {missileData.ExplosionRadius}");

		// смотрим кто попал под взрыв
		Collider[] colliders = Physics.OverlapSphere(transform.position, missileData.ExplosionRadius);
		foreach (Collider hit in colliders)
		{
			// проверяем объект - целевой, если да, то смотрим разрушаемый ли он
			if ((targetLayers & (1 << hit.gameObject.layer)) != 0)
			{
				Destructable destructable = hit.gameObject.GetComponent<Destructable>();
				if (destructable)
				{
					destructable.Hit(ParentInfo.Owning, Damage - Damage*(int)(Vector3.Distance(transform.position, hit.transform.position)/missileData.ExplosionRadius));
				}
			}

			Rigidbody rb = hit.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.AddExplosionForce(missileData.ExplosionForce, transform.position, missileData.ExplosionRadius, 0.0f, ForceMode.Impulse);
			}
		}
		
		Destroy(gameObject);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (launched)
		{
			if (Vector3.Distance (spawnPos, transform.position) > missileData.MaxDistance) {
				Detonate();
			}

			float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;

			// проверка столкновения
			if (Physics.Raycast(
				transform.position, 
				rb.linearVelocity.normalized, 
				out RaycastHit hit,
				distance) && hit.collider.gameObject != ParentInfo.Instance
			) {
				hit.collider.enabled = true;

				transform.position = hit.point;

				Detonate();
			}

			rb.AddForce(transform.forward * missileData.ThrustForse);
		}
	}
}
