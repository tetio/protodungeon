using System;
using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{
    int layerMask = ~(1 << 10);
    private float speed = 5f; //0.05f;

    public Vector3 dstPosition;

    private float duration = 0.2f;

    bool free = true;


    [SerializeField] private VirtualJoystick inputSource;

    // public GameObject _gameManager;
    // GameManager gameManager;

Rigidbody2D rb;
    // Start is called before the first frame update
    void Awake()
    {
        // gameManager = _gameManager.GetComponent<GameManager>();//GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
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
        
        if (Math.Abs(vertical) > Math.Abs(horizontal) && vertical > 0)
        {
            dstPosition = transform.position + Vector3.up;
            if (CheckForWall(dstPosition))
                StartCoroutine(LerpPosition(dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) > Math.Abs(horizontal) && vertical < 0)
        {
            dstPosition = transform.position + Vector3.down;
            if (CheckForWall(dstPosition))
                StartCoroutine(LerpPosition(dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) <= Math.Abs(horizontal) && horizontal > 0)
        {
            dstPosition = transform.position + Vector3.right;
            if (CheckForWall(dstPosition))
                StartCoroutine(LerpPosition(dstPosition, duration)); //will do the lerp over two seconds
        }
        else if (Math.Abs(vertical) <= Math.Abs(horizontal) && horizontal < 0)
        {
            dstPosition = transform.position + Vector3.left;
            if (CheckForWall(dstPosition))
                StartCoroutine(LerpPosition(dstPosition, duration)); //will do the lerp over two seconds
        }
        
    }

    private bool CheckForWall(Vector3 position)
     {
    //    Collider[] colliders = Physics.OverlapSphere(position, 0.0f);
    //    return colliders.Length == 0; //returns all the colliders that contain your position
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 15f, layerMask);
        if (hit.collider != null && hit.collider.tag == "WALL")
        {
            return false; // tHere's a wall
        }
        return true; // no wall!
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        free = false;
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        free = true;
        //Input.ResetInputAxes();
    }
}