using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Hero : Entity
{

    private TMPro.TextMeshProUGUI bubbleText;

    [SerializeField] private AudioClip footstep;
    [SerializeField] private AudioClip coin;
    AudioSource sound;
    int layerMask = ~(1 << 10);

    private Combat combat = new Combat();

    public Vector3 dstPosition;

    private float duration = 0.3f;

    bool free = true;

    private GameManager gameManager;
    private System.Random rng = new System.Random();

    //[SerializeField] private VirtualJoystick inputSource;
    [SerializeField] private Canvas inputSource;


    // public GameObject _gameManager;
    // GameManager gameManager;

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("/GameManager").GetComponent<GameManager>();//GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        sound = GetComponent<AudioSource>();
        sound.clip = footstep;
        bubbleText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        bubbleText.text = "IDLE";
        Debug.Log("AAA");
    }

    // Update is called once per frame
    void Update()
    {
        if (!free)
        {
            return;
        }
        float vertical = inputSource.Direction.z;
        float horizontal = inputSource.Direction.x;
        GameObject hero = this.transform.gameObject;
        bool heroIsMoving = (vertical != 0 || horizontal != 0);

        if (Math.Abs(vertical) > Math.Abs(horizontal) && vertical > 0)
        {
            dstPosition = transform.position + Vector3.up;
            if (CanMove(hero, dstPosition))
                StartCoroutine(LerpPosition(hero, dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) > Math.Abs(horizontal) && vertical < -0)
        {
            dstPosition = transform.position + Vector3.down;
            if (CanMove(hero, dstPosition))
                StartCoroutine(LerpPosition(hero, dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) < Math.Abs(horizontal) && horizontal > 0)
        {
            dstPosition = transform.position + Vector3.right;
            if (CanMove(hero, dstPosition))
                StartCoroutine(LerpPosition(hero, dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) < Math.Abs(horizontal) && horizontal < -0)
        {
            dstPosition = transform.position + Vector3.left;
            if (CanMove(hero, dstPosition))
                StartCoroutine(LerpPosition(hero, dstPosition, duration)); //will do the lerp over two seconds
        }
        if (heroIsMoving)
        {
            MobsTurn();
        }
        bubbleText.transform.position = this.transform.position + Vector3.up * 0.8f;
    }

    void MobsTurn()
    {
        var activeMobs = gameManager.Mobs.Where(mob => distanceFromHero(mob.transform.position) <= 5);
        var dir = (rng.Next(10) % 2 == 0) ? Vector3.left : Vector3.right;
        activeMobs.ToList().ForEach(mob =>
        {
            // TODO check if mob can attack (hero is at distance == 1)
            var newPosition = mob.transform.position + dir;
            if (CanMove(mob, newPosition))
                StartCoroutine(LerpPosition(mob, newPosition, duration));
        });
    }

    private bool CanMove(GameObject go, Vector3 position)
    {
        if (go.tag == "HERO")
        {
            bubbleText.color = new Color(255, 255, 255, 255);
            bubbleText.text = "MOVING";
            sound.clip = footstep;
        }
        else if (position == dstPosition)
        {
            return false;
        }

        //    Collider[] colliders = Physics.OverlapSphere(position, 0.0f);
        //    return colliders.Length == 0; //returns all the colliders that contain your position
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 15f, layerMask);
        if (hit.collider != null && hit.collider.tag == "WALL")
        {
            return false; // tHere's a wall
        }
        else if (hit.collider != null && hit.collider.tag == "COIN")
        {
            if (go.tag == "HERO")
            {
                bubbleText.color = new Color(0, 255, 255, 255);
                bubbleText.text = "I'M RICH!";
                sound.clip = coin;
                Destroy(hit.collider.transform.gameObject);
            }
            else
            {
                return false;
            }
        }
        else if (hit.collider != null && hit.collider.tag == "MOB" && go.tag == "HERO")
        {
            Debug.Log("HIT");
            int damageDone = combat.Attack(this, hit.collider.GetComponent<Entity>());
            if (damageDone > 0)
            {
                // check mob HP
                Mob mob = hit.collider.GetComponent<Mob>();
                mob.HitPoints -= damageDone;
                if (mob.HitPoints < 0) {
                    gameManager.Mobs.Remove(hit.collider.transform.gameObject);
                    Destroy(hit.collider.transform.gameObject);
                }
                Debug.Log($"Damage done => {damageDone}");
                bubbleText.color = new Color(255, 255, 255, 255);
                bubbleText.text = $"{damageDone}";
            }
            else
            {
                bubbleText.text = "MISS!";
                Debug.Log($"Damage done => Miss!");
            }
            StartCoroutine(HitCoroutine(go, duration));
            // needs fixing bubbleText.transform.position = this.transform.position;
            return false;
        }
        else if (hit.collider != null && hit.collider.tag == "HERO")
        {
            bubbleText.color = new Color(255, 0, 0, 255);
            bubbleText.text = "ARGH!";
            return false;
        }
        return true; // no wall!
    }


    // IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    // {
    //     free = false;
    //     sound.Play();
    //     float time = 0;
    //     Vector3 startPosition = transform.position;

    //     while (time < duration)
    //     {
    //         transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
    //         time += Time.deltaTime;
    //         yield return null;
    //     }
    //     transform.position = targetPosition;
    //     free = true;
    //     sound.Stop();
    //     //Input.ResetInputAxes();
    // }

    IEnumerator LerpPosition(GameObject go, Vector3 targetPosition, float duration)
    {
        free = false;
        if (go.tag == "HERO")
        {
            sound.Play();
        }
        float time = 0;
        Vector3 startPosition = go.transform.position;

        while (time < duration)
        {
            go.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        go.transform.position = targetPosition;
        free = true;
        if (go.tag == "HERO")
        {
            sound.Stop();
            bubbleText.color = new Color(255, 255, 255, 255);
            bubbleText.text = "IDLE";
        }
        //Input.ResetInputAxes();
    }


    IEnumerator HitCoroutine(GameObject go, float duration)
    {
        free = false;
        if (go.tag == "HERO")
        {
            //sound.Play();
        }
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        free = true;
        if (go.tag == "HERO")
        {
            // sound.Stop();
            bubbleText.text = "IDLE";
        }
    }


    private float distanceFromHero(Vector2 mobPosition)
    {
        return Vector2.Distance(mobPosition, this.transform.position);
    }
}