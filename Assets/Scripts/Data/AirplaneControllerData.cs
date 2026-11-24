using UnityEngine;

[CreateAssetMenu(fileName = "NewAirplaneControllerData", menuName = "Airplane Controller Data")]
public class AirplaneControllerData : ScriptableObject 
{
   [SerializeField] private float maxRoll = 1.05f;
	[SerializeField] private float rollPerYaw = 0.1f;
	[SerializeField] private Vector2 moveSensitivity = Vector2.one;
	[SerializeField] private float smoothing = 20f;
	[SerializeField] private float thrustSensitivity = 0.2f;
	[SerializeField] private  AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.0f, 0f, 5f), new Keyframe(1f, 1f, 0f, 0f));

	public float MaxRoll { get => maxRoll; }
	public float RollPerYaw { get => rollPerYaw; }
	public Vector2 MoveSensitivity { get => moveSensitivity; }
	public float Smoothing { get => smoothing; }
	public float ThrustSensitivity { get => thrustSensitivity; }
	public AnimationCurve MouseSensitivityCurve { get => mouseSensitivityCurve; }
}
