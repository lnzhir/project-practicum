using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Cannon {
	public int bulletCount = 5;

	protected override void OnFire() {
		for (int i = 0; i < bulletCount; i++) {
			base.OnFire ();
		}
	}
}
