using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Audio Connections")]
    public AudioSource pas; // Public Audio Source
    [Header("ObjectConnections")]
    [Header("PlayerStuff")]
    public Player ps;
    [Header("Settings")]
    public float animspeed = 1;
    public string previousScene; // To track what the previous scene was
    public string nextScene;
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
        pas = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps==null)
        {
            ps = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            return;
        }
        if (ps.currentState==Player.State.DEAD)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(previousScene);
            }
            Debug.Log("You Lose!");
        }
        if (ps.currentState == Player.State.WON)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(nextScene);
            }
            Debug.Log("You Won!");
        }
        
    }

    
}
