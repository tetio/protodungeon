using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;


public class Hero : Entity
{

    private int experience = 0;
    public int Experience {
        get {return experience;}
    }
    private TMPro.TextMeshProUGUI bubbleText;
    private TMPro.TextMeshProUGUI scoreText;

    [SerializeField] private AudioClip footstep;
    [SerializeField] private AudioClip coin;
    
    



  
    bool free = true;

    private GameManager gameManager;



    private System.Random rng = new System.Random();

    
    Rigidbody2D rb;


    void Awake()
    {
        gameManager = GameObject.Find("/GameManager").GetComponent<GameManager>();//GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();

        bubbleText = GetComponentsInChildren<TMPro.TextMeshProUGUI>()[0];
        scoreText = GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1];
        bubbleText.text = "IDLE";
    }

    public void setBubbleTextMessage(string message) {
        bubbleText.text = message;
    }

    public void setBubbleTextColor(Color color) {
        bubbleText.color = color;
    }

    public void setBubbleTextPosition(Vector3 position) {
        bubbleText.transform.position = position;
    }

    public void setScoreTextValue(int score) {
        scoreText.text = $"Score: {score.ToString().PadLeft(4, '0') }";
    }


    public AudioClip getAudioClipFootStep(){
        return footstep;
    }

    public AudioClip getAudioClipCoin() {
        return coin;
    }
}