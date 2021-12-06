using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;


public class Hero : Entity
{

    private int experience = 0;
    public int Experience
    {
        get { return experience; }
    }
    public int addExpPoints(int expPoints)
    {
        experience += expPoints;
        experienceText.text = $"Exp: {experience.ToString().PadLeft(4, '0') }";
        return experience;
    }
    private TMPro.TextMeshProUGUI bubbleText;
    private TMPro.TextMeshProUGUI scoreText;
    private TMPro.TextMeshProUGUI experienceText;

    [SerializeField] private AudioClip footstep;
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip yummy;
    Rigidbody2D rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        bubbleText = GetComponentsInChildren<TMPro.TextMeshProUGUI>()[0];
        scoreText = GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1];
        experienceText = GetComponentsInChildren<TMPro.TextMeshProUGUI>()[2];
        bubbleText.text = "IDLE";
    }

    public void setBubbleTextMessage(string message)
    {
        bubbleText.text = message;
    }

    public void setBubbleTextColor(Color color)
    {
        bubbleText.color = color;
    }

    public void setBubbleTextPosition(Vector3 position)
    {
        bubbleText.transform.position = position;
    }

    public void setScoreTextValue(int score)
    {
        scoreText.text = $"Score: {score.ToString().PadLeft(4, '0') }";
    }


    public AudioClip getAudioClipFootStep()
    {
        return footstep;
    }

    public AudioClip getAudioClipCoin()
    {
        return coin;
    }

       public AudioClip getAudioClipYummy()
    {
        return yummy;
    }
}