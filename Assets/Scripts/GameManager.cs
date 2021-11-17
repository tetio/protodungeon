using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject mob;

    [SerializeField] private GameObject hero;

    int layerMask = ~(1 << 10);
    private int width = 16;
    private List<int> length = new List<int> { 8, 192 };
    private List<int> corridor1 = new List<int> { 8, 1, 7 };
    private List<int> corridor2 = new List<int> { 7, 1, 8 };
    private List<int> room1 = new List<int> { 2, 12, 2 };
    private List<int> room2 = new List<int> { 6, 4, 6 };
    public List<Vector2> walls = new List<Vector2>();

    private System.Random rng = new System.Random();

    private List<GameObject> mobs = new List<GameObject>();

    // Mobs' stuff
    private float nextActionTime = 0.0f;
    private float mobActionPeriod = 2.0f;
    private float duration = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        // float tileWidth = floorTile.GetComponents<SpriteRenderer>()[0].sprite.bounds.size.x;
        // float tileHeight = floorTile.GetComponents<SpriteRenderer>()[0].sprite.bounds.size.y;
        // // int tileWidth = floorTile.
        // Initial
        for (int j = 0; j < length[0]; j++)
        {
            for (int i = 0; i < width; i++)
            {
                addChild(Instantiate(floor, new Vector2(i, j), Quaternion.identity));
            }
        }
        // Dungeon
        int wallLeft = corridor1[0];
        int corridor = corridor1[1];
        int wallRight = corridor1[2];
        int low = rng.Next(2) + 1;
        int height = rng.Next(7) + low;
        bool lockedOnRoom = false;
        int nextRoomAt = 1 + length[0] + rng.Next(4);
        for (int j = 0 + length[0]; j < length[1]; j++)
        {
            if (!lockedOnRoom && nextRoomAt == j)
            {
                low = j;
                height = rng.Next(7) + low + 3;
                List<int> room = (nextRoomAt % 2 == 0) ? room1 : room2;
                wallLeft = room[0];
                corridor = room[1];
                wallRight = room[2];
                lockedOnRoom = true;
            }
            else if (j > height)
            {
                lockedOnRoom = false;
                nextRoomAt = j + rng.Next(4);
                List<int> newCorridor = (height % 2 == 0) ? corridor1 : corridor2;
                wallLeft = newCorridor[0];
                corridor = newCorridor[1];
                wallRight = newCorridor[2];
            }
            BuildRoom(wallLeft, corridor, wallRight, lockedOnRoom, j);
        }
    }

    private void BuildRoom(int wallLeft, int corridor, int wallRight, bool lockedOnRoom, int j)
    {
        for (int i = 0; i < wallLeft; i++)
        {
            var pos = new Vector2(i, j);
            walls.Add(pos);
            addChild(Instantiate(wall, pos, Quaternion.identity));
        }
        for (int i = wallLeft; i < wallLeft + corridor; i++)
        {
            addChild(Instantiate(floor, new Vector2(i, j), Quaternion.identity));
            if (lockedOnRoom)
            {
                if (rng.Next(100) <= 5)
                {
                    addChild(Instantiate(coin, new Vector3(i, j, -1), Quaternion.identity));
                }
                else if (rng.Next(100) < 2)
                {
                    var mob = Instantiate(this.mob, new Vector3(i, j, -1), Quaternion.identity);
                    mobs.Add(mob);
                    addChild(mob);

                }
            }
        }
        for (int i = wallLeft + corridor; i < wallLeft + corridor + wallRight; i++)
        {
            var pos = new Vector2(i, j);
            walls.Add(pos);
            addChild(Instantiate(wall, pos, Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO mob
        if (Time.time > nextActionTime)
        {
            nextActionTime += mobActionPeriod;
            var activeMobs = mobs.Where(mob => distanceFromHero(mob.transform.position) <= 5);
            var dir = (rng.Next(10) % 2 == 0) ? Vector3.left : Vector3.right;
            activeMobs.ToList().ForEach(mob =>
            {
                var newPosition = mob.transform.position + dir;
                if (CanMove(newPosition))
                    StartCoroutine(LerpPosition(mob, newPosition, duration));
            });
        }

    }

    private float distanceFromHero(Vector2 mobPosition)
    {
        return Vector2.Distance(mobPosition, hero.transform.position);
    }

    private void addChild(GameObject go)
    {
        go.transform.parent = transform;
    }


    private bool CanMove(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector3.zero, 15f, layerMask);
        if (hit.collider != null && (hit.collider.tag == "WALL" || hit.collider.tag == "COIN"))
        {
            return false; // tHere's a wall
        } else if (hit.collider != null && hit.collider.tag == "HERO")
        {
            // TODO hero was attacked
            return false;
        }
        return true; // no wall!
    }

    IEnumerator LerpPosition(GameObject go, Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = go.transform.position;

        while (time < duration)
        {
            go.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        go.transform.position = targetPosition;
    }
}
