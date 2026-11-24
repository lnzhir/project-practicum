using System;
using UnityEngine;

public abstract class Gun : MonoBehaviour {
	protected virtual float FireDelay { get; }

	public event Action recharged;

	protected ParentInfo parentInfo;

	//private long lastFire = 0;
	protected float fireTimeRemainig = 0;
	protected bool rechargeNotifyed = true;

	// Use this for initialization
	protected virtual void Start () {

		parentInfo = new ParentInfo()
		{
			Instance = transform.root.gameObject,
			Owning = transform.root.gameObject.GetComponentInChildren<Owning> (),
			Rigidbody = transform.root.gameObject.GetComponentInChildren<Rigidbody> ()
		};
	}

	protected abstract void OnFire();

	public virtual void Fire() {

		if (fireTimeRemainig <= 0)
		{
			OnFire();
			
			fireTimeRemainig = FireDelay;
		}
	}

	protected virtual void Update()
	{
		// if (fireTimeRemainig <= 0)
		// {
		// 	if (rechargeNotifyed)
		// 	{
		// 		recharged?.Invoke();
		// 	}
		// } 
		// else
		// {
		// 	fireTimeRemainig -= Time.deltaTime;
		// }
		fireTimeRemainig -= Time.deltaTime;
	}
}


public interface IShootable {
	public void Fire ();
}