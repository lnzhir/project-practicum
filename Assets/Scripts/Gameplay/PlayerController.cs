using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
   private InputSystem_Actions m_Actions;                  
	private InputSystem_Actions.PlayerActions m_Player;     

	public Vector2 CursorPos { set; get; }
	public static PlayerController Instance = null;

	void Awake()
	{
		Instance = this;
		m_Actions = new InputSystem_Actions();              
		m_Player = m_Actions.Player;
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
		
	}

	public void OnPause(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			GameSettings.Instance.IsPauseMenuOpen = !GameSettings.Instance.IsPauseMenuOpen;
		}
	}
}
