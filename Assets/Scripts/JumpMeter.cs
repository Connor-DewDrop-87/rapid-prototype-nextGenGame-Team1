using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMeter : MonoBehaviour
{
    public GameObject jumpMeter;
    public Player ps;
    // Start is called before the first frame update
    void Start()
    {
        jumpMeter = GameObject.Find("JumpMeter");
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.chargingUp == true)
        {
            if (jumpMeter.transform.localScale.x <= 1)
            {
                jumpMeter.transform.localScale += new Vector3(Time.deltaTime, 0, 0);
            }
        }
        else
        {
            jumpMeter.transform.localScale = new Vector3(0, 1, 1);
        }
    }
}
