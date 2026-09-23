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
        jumpMeter.transform.localScale = new Vector3(ps.ChargePercent, 1, 1);
    }
}
