using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;




public class Destructable : MonoBehaviour {

	//public delegate void Action(GameObject go);
   //public delegate void ActionInt(GameObject go, int value);

	public float health = 1;
	public float collisionDamage = 1; // урон за м/с
	public bool friendlyFire = false;
	public bool collisionDamaging = false;
	public bool bulletHitDamaging = true;

	public event Action<GameObject, float> damaged;
	public event Action<GameObject> destructed;

	private Owning owning;
	private bool isDestructed = false;

	protected void Start()
	{
		owning = GetComponent<Owning>();
	}

   void CheckHealth() {
		if (health <= 0) {
			destructed?.Invoke(gameObject);

			isDestructed = true;

			Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log(Vector3.Dot(collision.contacts[0].normal, collision.impulse));
		if (collisionDamaging && !isDestructed) {
			float mass = collision.rigidbody == null ? 1f : collision.rigidbody.mass;
			float damage = Vector3.Dot(collision.contacts[0].normal, collision.relativeVelocity / mass) * collisionDamage;
			health -= Mathf.Abs(damage);

			damaged?.Invoke(gameObject, damage);

			CheckHealth ();
		}
	}

	public void Hit(Owning bulletOwning, float damage)
	{
		if (bulletHitDamaging && !isDestructed) {
			if (friendlyFire || !bulletOwning || !owning || owning.Team != bulletOwning.Team)
			{
				health -= damage;
			}

			damaged?.Invoke(gameObject, damage);
			CheckHealth ();
		}
	}

	void OnDestroy()
	{
		damaged = null;
		destructed = null;
	}

	/*void OnTriggerEnter(Collider other) {
		Debug.Log("hit");

		if (bulletHitDamaging && !isDestructed && other.CompareTag (Tag.Bullet)) {
			Debug.Log("hit2");
			

			Bullet bullet = other.gameObject.GetComponent<Bullet> ();
			if (bullet) {
				Debug.Log("hit3");

				Owning bulletOwning = bullet.ParentInfo.Owning;

				if (friendlyFire || !bulletOwning || !owning || owning.Team != bulletOwning.Team)
				{
					health -= bullet.Damage;
				}

				damaged?.Invoke(gameObject, bullet.Damage);
				CheckHealth ();
			}
		}
	}*/
}