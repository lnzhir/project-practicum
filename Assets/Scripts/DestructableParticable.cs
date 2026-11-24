using UnityEngine;

public class DestructableParticable : Destructable
{
    public GameObject deathPrefab;


    void Awake()
    {
        destructed += OnDeath_;       
    }

    protected void OnDeath_(GameObject go)
    {
        if (deathPrefab) {
			GameObject effect = Instantiate (
				deathPrefab, 
				transform.position, 
				transform.rotation
			);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
