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
    }

    public void ShowAltitude()
    {
        altitude.text = $"Altitude: {(Mathf.Round(ps.playerRef.position.y*100))/100}m";
    }
}
