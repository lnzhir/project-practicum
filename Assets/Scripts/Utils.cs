using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
	public const float airDensity = 1.225f;

	public static float Angle(Vector2 vec1, Vector2 vec2) {
		return Mathf.Atan2 (vec2.y, vec2.x) - Mathf.Atan2 (vec1.y, vec1.x);
	}

	public static Vector2 AngleVec2(Vector3 vec1, Vector3 vec2) {
		Quaternion angle1 = Quaternion.LookRotation(vec1);
		Quaternion angle2 = Quaternion.LookRotation(vec2);

		Vector3 angles = angle2.eulerAngles - angle1.eulerAngles;

		/*if (angles.x > 180 || angles.x < -180)
			angles.x -= Mathf.Sign(angles.x) * 360;
		if (angles.y > 180 || angles.y < -180)
			angles.y -= Mathf.Sign(angles.y) * 360;

		return angles;*/

		return ClampAngles2 (angles);
	} 

	public static Vector3 AngleVec3(Vector3 vec1, Vector3 vec2) {
		Quaternion angle1 = Quaternion.LookRotation(vec1);
		Quaternion angle2 = Quaternion.LookRotation(vec2);

		Vector3 angles = angle2.eulerAngles - angle1.eulerAngles;

		/*if (angles.x > 180 || angles.x < -180)
			angles.x -= Mathf.Sign(angles.x) * 360;
		if (angles.y > 180 || angles.y < -180)
			angles.y -= Mathf.Sign(angles.y) * 360;
		if (angles.z > 180 || angles.z < -180)
			angles.z -= Mathf.Sign(angles.z) * 360;

		return angles;*/

		return ClampAngles3 (angles);
	}

	public static Vector2 ClampAngles2(Vector2 angles) {
		if (angles.x > 180 || angles.x < -180)
			angles.x -= Mathf.Sign(angles.x) * 360;
		if (angles.y > 180 || angles.y < -180)
			angles.y -= Mathf.Sign(angles.y) * 360;

		return angles;
	}

	public static Vector3 ClampAngles3(Vector3 angles) {
		if (angles.x > 180 || angles.x < -180)
			angles.x -= Mathf.Sign(angles.x) * 360;
		if (angles.y > 180 || angles.y < -180)
			angles.y -= Mathf.Sign(angles.y) * 360;
		if (angles.z > 180 || angles.z < -180)
			angles.z -= Mathf.Sign(angles.z) * 360;

		return angles;
	}

	public static Vector2 SignedAngles2(Vector2 angles) {
		if (angles.x > 180)
			angles.x -= 360;
		if (angles.y > 180)
			angles.y -= 360;

		return angles;
	}

	public static Vector3 SignedAngles3(Vector3 angles) {
		if (angles.x > 180)
			angles.x -= 360;
		if (angles.y > 180)
			angles.y -= 360;
		if (angles.z > 180)
			angles.z -= 360;

		return angles;
	}

	public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
	{
		return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}

	// от 0 до максимума
	public static float Map(float x, float in_max, float out_max)
	{
		return x * out_max / in_max;
	}


	public static float airDragMagnitude(float velocity, float dragArea) {
		return 0.5f * airDensity * velocity * velocity * dragArea;
	}


	public static void ScaleTree(Transform node, Vector3 factor)
	{
		//node.localScale = Vector3.Scale(node.localScale, factor);
		node.localScale = factor;

		foreach (Transform child in node)
		{
			ScaleTree(child, factor);
		}
		
	}
}