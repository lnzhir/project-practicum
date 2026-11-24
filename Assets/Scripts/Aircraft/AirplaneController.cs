using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Обратите внимание на Console.WriteLine -> в Unity нужно использовать Debug.Log
public class AirplaneController : MonoBehaviour, InputSystem_Actions.IAirplaneActions
{	
	[SerializeField]
	private AirplaneControllerData controllerData;
	public AirplaneControllerData ControllerData { get => controllerData; set { controllerData = value; } }

	[Range(-1, 1)] float pitch;
	[Range(-1, 1)] float yaw;
	[Range(-1, 1)] float roll;
	[Range(-1, 1)] float thrust;
	Vector2 move;
	bool attackCannon = false;
	bool look = false;
	private Vector2 smoothedDelta;

	Airplane airplane;

	private InputSystem_Actions m_Actions;                  
	private InputSystem_Actions.AirplaneActions m_Player;     

	public Vector2 CursorPos { set; get; }
	public static AirplaneController Instance = null;

	void Awake()
	{
		Instance = this;
		m_Actions = new InputSystem_Actions();              
		m_Player = m_Actions.Airplane;
		m_Player.AddCallbacks(this);                      
	}
    
	void OnDestroy()
	{
		m_Actions.Dispose();                              
	}

	void OnEnable()
	{
		m_Player.Enable();                                
	}

	void OnDisable()
	{
		m_Player.Disable();                               
	}

	void Start()
	{
		airplane = GetComponent<Airplane>();

		//RebindManager.Instance.StartRebinding(m_Actions.Player.Pitch, InputActionExtensions.GetBindingIndexForPart(m_Actions.Player.Pitch, "Positive"));

		GameSettings.Instance.pauseChanged += PauseChanged;
	}

	void PauseChanged(bool state)
	{
		enabled = !state;
	}

	void LateUpdate()
	{
		// var newAngle = move / moveSensitivity;

		// var sensitivityFactor = mouseSensitivityCurve.Evaluate(move.magnitude / 30.0f);
		// CameraOrbit.Instance.Angle += new Vector2(-move.y, move.x) * moveSensitivity * sensitivityFactor;

		// Debug.Log($"magnitude {move.magnitude / 30.0f}");
		// Debug.Log($"sensitivityFactor {sensitivityFactor}");

		// move = Vector2.zero;

		Vector2 rawDelta = move;
		move = Vector2.zero;

		// Сглаживание, не зависящее от частоты кадров
		float t = 1f - Mathf.Exp(-controllerData.Smoothing * Time.unscaledDeltaTime);
		smoothedDelta = Vector2.Lerp(smoothedDelta, rawDelta, t);

		CameraOrbit.Instance.Angle +=
			new Vector2(-smoothedDelta.y, smoothedDelta.x) * controllerData.MoveSensitivity;
	}

	void FixedUpdate()
	{
		float targetYaw = 0;
		float targetPitch = 0;
		float rollCommand = 0;
		if (!look)
		{
			Vector3 camForward = CameraOrbit.Instance.transform.forward;
			Vector3 localDir = transform.InverseTransformDirection(camForward).normalized;
			Vector2 horizontalDir = new Vector2(localDir.x, localDir.z);

			targetYaw = Mathf.Atan2(localDir.x, localDir.z);

			float horizontalLength = new Vector2(localDir.x, localDir.z).magnitude;
			targetPitch = Mathf.Atan2(localDir.y, horizontalLength);

			if (horizontalDir.sqrMagnitude > 0.0001f)
			{
				float lateral = horizontalDir.normalized.x;

				if (localDir.z < 0f && Mathf.Abs(lateral) < 0.00001f)
					lateral = 0f;

				rollCommand = lateral * controllerData.MaxRoll;
			}

			rollCommand = Mathf.Clamp(
				rollCommand * controllerData.RollPerYaw,
				-controllerData.MaxRoll,
				controllerData.MaxRoll
			);

			// float rollCommand = Mathf.Clamp(
			// 	targetYaw * rollPerYaw,
			// 	-maxRoll,
			// 	maxRoll
			// );
		}

		airplane.SetControlSurfecesAngles(
			pitch - targetPitch,
			rollCommand + roll,
			yaw - targetYaw,
			0
		);

		airplane.ThrustPercent += Time.fixedDeltaTime * thrust * controllerData.ThrustSensitivity;

		if (attackCannon)
		{
			airplane.FireCannon();
		}
	}

	public void OnPitch(InputAction.CallbackContext context)
	{
		pitch = context.ReadValue<float>();
	}

	public void OnYaw(InputAction.CallbackContext context)
	{
		yaw = context.ReadValue<float>();
	}

	public void OnRoll(InputAction.CallbackContext context)
	{
		roll = context.ReadValue<float>();
	}

	public void OnLook(InputAction.CallbackContext context)
	{
		move += context.ReadValue<Vector2>();
		
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		attackCannon = context.performed ? true : (context.canceled ? false : attackCannon);
	}

	public void OnLaunchMissile(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			airplane.FireMissile();
		}
	}

	public void OnThrust(InputAction.CallbackContext context)
	{
		thrust = context.ReadValue<float>();
	}

	public void OnOnlyLook(InputAction.CallbackContext context)
	{
		look = context.performed ? true : (context.canceled ? false : look);
	}

	public void OnZoom(InputAction.CallbackContext context)
	{
		CameraOrbit.Instance.Distance -= context.ReadValue<float>();
	}
}
