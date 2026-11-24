using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class GunData : ScriptableObject {
	[SerializeField]
	private float fireDelay = 0.1f; // in seconds
	[SerializeField]
	private float damage = 1f;
	[SerializeField]
	private float maxDistance = 2000.0f;

	public float FireDelay { get { return fireDelay; } }
	public float Damage { get { return damage; } }
	public float MaxDistance { get { return maxDistance; } }
}