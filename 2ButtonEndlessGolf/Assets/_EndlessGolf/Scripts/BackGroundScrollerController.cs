using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Chain
{
    public int tail;
    public int current;
    public int head;

}

public class BackGroundScrollerController : MonoBehaviour {
    [Header("Reference Objects")]
    public GameObject[] backGround;

    private Chain chain;
    private float mileStone;
    private const float Range = 120.9f;
    // Use this for initialization
    void Start ()
    {
        chain = new Chain();
        chain.current = 1;
        chain.tail = 0;
        chain.head = 2;
        mileStone = Range;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(Camera.main.transform.position.x >= mileStone)
        {
            backGround[chain.tail].transform.position = new Vector3(backGround[chain.head].transform.position.x + Range, 
                backGround[chain.tail].transform.position.y, backGround[chain.tail].transform.position.z);
            Vector3 euler = backGround[chain.tail].transform.rotation.eulerAngles;
            euler.y += 180;
            backGround[chain.tail].transform.rotation = Quaternion.Euler(euler);
            int rem = chain.tail;
            chain.tail = chain.current;
            chain.current = chain.head;
            chain.head = rem;
            mileStone += Range;
        }
	}
}
