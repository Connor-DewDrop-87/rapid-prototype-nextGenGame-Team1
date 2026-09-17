using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Audio Connections")]
    public AudioSource pas; // Public Audio Source
    [Header("ObjectConnections")]
    [Header("PlayerStuff")]
    
    public Player ps;
    public TextMeshProUGUI altitude;
    public GameObject jumpMeter;
    // Makes sure that the GameObject, and the children of the GameObject, is Stored between scenes
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        altitude = GameObject.Find("Altitude").GetComponent<TextMeshProUGUI>();
        jumpMeter = GameObject.Find("JumpMeter");
        pas = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ShowAltitude();
        if (ps.isDead==true)
        {
            Debug.Log("You Lose!");
        }
        if (ps.touchedGrass==true)
        {
            Debug.Log("You Won!");
        }
        if (ps.chargingUp==true)
        {
            IncreaseJumpMeter();
        }
        else
        {
            jumpMeter.transform.localScale = new Vector3(0, 1, 1);
        }
    }

    public void ShowAltitude()
    {
        altitude.text = $"Altitude: {(Mathf.Round(ps.playerRef.position.y*100))/100}m";
    }
    public void IncreaseJumpMeter()
    {
        if (jumpMeter.transform.localScale.x <= 1)
        {
            jumpMeter.transform.localScale += new Vector3(Time.deltaTime, 0, 0);
        }
    }
}
