using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
	public virtual ParentInfo ParentInfo { get; set; }
	public virtual float Damage { get; set; }
}




public class ParentInfo
{
	public GameObject Instance { get; set; } = null;
	public Owning Owning { get; set; } = null;
	public Rigidbody Rigidbody { get; set; } = null;

	public ParentInfo()
	{
	}
}