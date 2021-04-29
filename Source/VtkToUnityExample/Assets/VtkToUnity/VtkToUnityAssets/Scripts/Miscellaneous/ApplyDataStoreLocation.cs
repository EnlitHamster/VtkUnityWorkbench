using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDataStoreLocation : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
		DataStore.Instance.ApplyPositonRotationY(this.gameObject);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
