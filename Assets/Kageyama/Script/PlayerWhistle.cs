using UnityEngine;
using System.Collections;

public class PlayerWhistle : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "LargeEnemy" || col.tag == "SmallEnemy")
        {
            col.GetComponent<Enemy3D>().SoundNotice(this.transform);
        }
    }
}
