using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMeter : MonoBehaviour
{
    public GameObject jumpMeter;
    public ChargeJumpPlayer ps;
    // Start is called before the first frame update
    void Start()
    {
        jumpMeter = GameObject.Find("JumpMeter");
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<ChargeJumpPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.state==ChargeJumpPlayer.JumpState.Charging)
        {
            // Increase the Jump Meter to show the player is charging up
            if (jumpMeter.transform.localScale.x <= 1)
            {
                jumpMeter.transform.localScale += new Vector3(ps.ChargePercent, 0, 0);
            }
            // Ensures the Jump Meter doesn't show it is over 1
            if (jumpMeter.transform.localScale.x > 1)
            {
                jumpMeter.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else
        {
            // Reset back to 0
            jumpMeter.transform.localScale = new Vector3(0, 1, 1);
        }
    }
}
