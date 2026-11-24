using UnityEngine;
using System.Collections.Generic;

public class MissileLauncher : Gun
{
	public List<Missile> Missiles { get; set; } = new List<Missile>();

	
	[SerializeField]
	private float fireDelay = 1f;

	protected override float FireDelay { get => fireDelay; }

	protected override void OnFire()
	{
		if (Missiles.Count > 0)
		{
			var missile = Missiles[0];
			missile.ParentInfo = parentInfo;
			missile.transform.parent = null;
			missile.Launch();
			Missiles.RemoveAt(0);
		}
	}
}
