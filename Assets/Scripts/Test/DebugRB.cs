using UnityEngine;
using UnityEngine.UI;

public class DebugRB : MonoBehaviour
{
	public Text textSpeed;
	Airplane airplane;
	Rigidbody rb;
	float fps = 0;
	bool updatedFPS = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		airplane = GetComponent<Airplane>();
	}

	// Update is called once per frame
	void Update()
	{
		textSpeed.text = "V: " + ((int)rb.linearVelocity.magnitude).ToString("D3") + " m/s\n";
		textSpeed.text += "A: " + ((int)transform.position.y).ToString("D4") + " m\n";
		textSpeed.text += "T: " + (int)(airplane.ThrustPercent * 100) + "%\n";
		textSpeed.text += "FPS: " + fps.ToString() + "\n";

		if (!updatedFPS)
		{
			updatedFPS = true;
			Invoke("UpdateFPS", 1f);
		}
		
	}

	void UpdateFPS()
	{
		fps = Mathf.Round(1f / Time.unscaledDeltaTime);
		updatedFPS = false;
	}
}
