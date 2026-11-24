using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject {
	[SerializeField]
	private int id;
	[SerializeField]
	private string title;
    [SerializeField]
	private string levelSceneName;
	[SerializeField]
	private string mapSceneName;

	public int Id { get { return id; } }
	public string Title { get { return title; } }
    public string LevelSceneName { get { return levelSceneName; } }
	public string MapSceneName { get { return mapSceneName; } }
}
