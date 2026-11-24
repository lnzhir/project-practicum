using System;
using UnityEngine;

public class Owning : MonoBehaviour
{

	[SerializeField]
	private int team = 0;

	[SerializeField]
	private int destroyScore = 0;

	public int Team { get => team; set { team = value; } }

	public int DestroyScore { get => destroyScore; set { destroyScore = value; } }
	
	public Player Player { get; set; }
}
