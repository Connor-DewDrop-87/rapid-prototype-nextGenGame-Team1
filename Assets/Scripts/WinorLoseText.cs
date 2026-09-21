using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinorLoseText : MonoBehaviour
{
    public GameObject nextLevelTextBox;
    public GameObject resetTextBox;
    public Player ps;
    // Start is called before the first frame update
    void Start()
    {
        nextLevelTextBox = GameObject.Find("NextLevel");
        resetTextBox = GameObject.Find("Reset");
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.currentState == Player.State.DEAD)
        {
            resetTextBox.SetActive(true);
        }
        else
        {
            resetTextBox.SetActive(false);
        }
        if (ps.currentState == Player.State.WON)
        {
            nextLevelTextBox.SetActive(true);
        }
        else
        {
            nextLevelTextBox.SetActive(false);
        }
    }
}
