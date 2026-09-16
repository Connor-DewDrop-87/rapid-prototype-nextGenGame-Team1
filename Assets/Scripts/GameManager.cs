using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Audio Connections")]
    public AudioSource pas; // Public Audio Source
    [Header("ObjectConnections")]
    public Player ps;
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
        pas = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.isDead==true)
        {
            Debug.Log("You Lose!");
        }
    }
}
