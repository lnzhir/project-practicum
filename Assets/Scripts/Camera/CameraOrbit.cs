using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float minDistance = 2f;
	[SerializeField] private float maxDistance = 20f;
	[SerializeField] private Vector2 pitchConstr = new Vector2(-80f, 80f);
	[SerializeField] private Vector3 offset = Vector3.zero;

	public Transform Target { get => target; set { target = value; }}
	public Vector3 Offset { get => offset; set { offset = value; }}

	private float distance;
	public float Distance
	{
		get => distance;
		set
		{
			distance = Mathf.Clamp(value, minDistance, maxDistance);
			//UpdateState();
		}
	}
	private Vector2 angle;
	public Vector2 Angle
	{
		get => angle;
		set
		{
			angle = value;
			angle.x = Mathf.Clamp(angle.x, pitchConstr.x, pitchConstr.y);
			//UpdateState();
		}
	}

	public static CameraOrbit Instance = null;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		distance = (maxDistance + minDistance) / 2;

		Vector3 angles = transform.eulerAngles;
		angle.y = angles.y;
		angle.x = angles.x > 180f ? angles.x - 360f : angles.x;
	}

	void LateUpdate()
	{
		if (target)
		{
			Quaternion rotation = Quaternion.Euler(angle.x, angle.y, 0f);
			transform.position = target.position + offset - rotation * Vector3.forward * distance;
			transform.rotation = rotation;
		}
	}

	private void UpdateState()
	{
		Quaternion rotation = Quaternion.Euler(angle.x, angle.y, 0f);
		transform.position = target.position + offset - rotation * Vector3.forward * distance;
		transform.rotation = rotation;
	}

}