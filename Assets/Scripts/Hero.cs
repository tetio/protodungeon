using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Hero : MonoBehaviour
{

    [SerializeField] private AudioClip footstep;
    [SerializeField] private AudioClip coin;
    AudioSource sound;
    int layerMask = ~(1 << 10);
    private float speed = 5f; //0.05f;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!free)
        {
            return;
        }


        // float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        // float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;


        float vertical = inputSource.Direction.z;
        float horizontal = inputSource.Direction.x;
        GameObject hero = this.transform.gameObject;

        if (Math.Abs(vertical) > Math.Abs(horizontal) && vertical > 0.5f)
        {
            dstPosition = transform.position + Vector3.up;
            if (CanMove(hero, dstPosition))
                StartCoroutine(LerpPosition(hero, dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) > Math.Abs(horizontal) && vertical < -0.5f)
        {
            dstPosition = transform.position + Vector3.down;
            if (CanMove(hero, dstPosition))
                StartCoroutine(LerpPosition(hero, dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) < Math.Abs(horizontal) && horizontal > 0.5f)
        {
            dstPosition = transform.position + Vector3.right;
            if (CanMove(hero, dstPosition))
                StartCoroutine(LerpPosition(hero, dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) < Math.Abs(horizontal) && horizontal < -0.5f)
        {
            dstPosition = transform.position + Vector3.left;
            if (CanMove(hero, dstPosition))
                StartCoroutine(LerpPosition(hero, dstPosition, duration)); //will do the lerp over two seconds
        }
        if (vertical != 0 || horizontal != 0)
        {
            MoveMobs();
        }

    }

    void MoveMobs()
    {
        var activeMobs = gameManager.Mobs.Where(mob => distanceFromHero(mob.transform.position) <= 5);
        var dir = (rng.Next(10) % 2 == 0) ? Vector3.left : Vector3.right;
        activeMobs.ToList().ForEach(mob =>
        {
            var newPosition = mob.transform.position + dir;
            if (CanMove(mob, newPosition))
                StartCoroutine(LerpPosition(mob, newPosition, duration));
        });
    }

    private bool CanMove(GameObject go, Vector3 position)
    {
        if (go.tag == "HERO")
        {
            sound.clip = footstep;
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
                sound.clip = coin;
                Destroy(hit.collider.transform.gameObject);
            }
            else
            {
                return false;
            }
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
        }
        //Input.ResetInputAxes();
    }

    private float distanceFromHero(Vector2 mobPosition)
    {
        return Vector2.Distance(mobPosition, this.transform.position);
    }
}