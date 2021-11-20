using System.Collections;
using System.Collections.Generic;
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
    public List<GameObject> Mobs
    {
        get {return mobs;}
    }


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
    }


    private void addChild(GameObject go)
    {
        go.transform.parent = transform;
    }

}
