using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AltitudeText : MonoBehaviour
{
    public TextMeshProUGUI altitude;
    public Player ps;
    // Start is called before the first frame update
    void Start()
    {
        altitude = GameObject.Find("Altitude").GetComponent<TextMeshProUGUI>();
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        altitude.text = $"Altitude: {((Mathf.Round(ps.playerRef.position.y * 100)) / 100) + 10}m";
    }
}
