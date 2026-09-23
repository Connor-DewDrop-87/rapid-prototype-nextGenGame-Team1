using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinorLoseText : MonoBehaviour
{
    public GameObject nextLevelTextBox;
    public GameObject resetTextBox;
    public ChargeJumpPlayer ps;
    // Start is called before the first frame update
    void Start()
    {
        nextLevelTextBox = GameObject.Find("NextLevel");
        resetTextBox = GameObject.Find("Reset");
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<ChargeJumpPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.isDead == true)
        {
            resetTextBox.SetActive(true);
        }
        else if (ps.touchedGrass == true)
        {
            nextLevelTextBox.SetActive(true);
        }
        else
        {
            nextLevelTextBox.SetActive(false);
            resetTextBox.SetActive(false);
        }
    }
}
