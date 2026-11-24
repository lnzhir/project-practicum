using System;
using UnityEngine;

public class Cannon : Gun {

	[SerializeField]
	private CannonData gunData;

	protected override float FireDelay { get => gunData.FireDelay; }

	protected override void OnFire() {
		// эффект выстрела
		if (gunData.MuzzleEffectPrefab) {
			GameObject effect = Instantiate (
				gunData.MuzzleEffectPrefab, 
				transform.position, 
				transform.rotation,
				transform
			);
		}

		float s1 = UnityEngine.Random.Range (-gunData.Spread, gunData.Spread);
		float s2 = UnityEngine.Random.Range (-gunData.Spread, gunData.Spread);

		if (gunData.BulletPrefab) {
			// выстрел
			GameObject bulletObject = Instantiate (
				gunData.BulletPrefab, 
				transform.position, 
				Quaternion.Euler(
					transform.eulerAngles.x + s1,
					transform.eulerAngles.y + s2,
					0
				)
			);

			Projectile bullet = bulletObject.GetComponent<Projectile> ();
			if (bullet) {
				bullet.ParentInfo = parentInfo;
				bullet.StartVelocity = gunData.StartVelocity;
				bullet.MaxDistance = gunData.MaxDistance;
				bullet.Damage = gunData.Damage;
			}
		}
	}
}