using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject floorTile;
    [SerializeField] private GameObject wallTile;
    [SerializeField] private GameObject foodTile;

    private int width = 8;
    private List<int> length = new List<int> { 8, 192 };
    public List<Vector2> walls = new List<Vector2>();





    // Start is called before the first frame update
    void Awake()
    {
        System.Random rng = new System.Random();
        // float tileWidth = floorTile.GetComponents<SpriteRenderer>()[0].sprite.bounds.size.x;
        // float tileHeight = floorTile.GetComponents<SpriteRenderer>()[0].sprite.bounds.size.y;
        // // int tileWidth = floorTile.
        // Initial
        for (int j = 0; j < length[0]; j++)
        {
            for (int i = 0; i < width; i++)
            {
                addChild(Instantiate(floorTile, new Vector2(i, j), Quaternion.identity));
            }
        }
        // Dungeon
        int wallLeft = 3;
        int corridor = 2;
        int wallRight = 3;
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
                wallLeft = 1;
                corridor = 6;
                wallRight = 1;
                lockedOnRoom = true;
            }
            else if (j > height)
            {
                lockedOnRoom = false;
                nextRoomAt = j + rng.Next(4);
                wallLeft = 3;
                corridor = 2;
                wallRight = 3;
            }
            for (int i = 0; i < wallLeft; i++)
            {
                var pos = new Vector2(i, j);
                walls.Add(pos);
                addChild(Instantiate(wallTile, pos, Quaternion.identity));
            }
            for (int i = wallLeft; i < wallLeft + corridor; i++)
            {
                addChild(Instantiate(floorTile, new Vector2(i, j), Quaternion.identity));
            }
            for (int i = wallLeft + corridor; i < wallLeft + corridor + wallRight; i++)
            {
                var pos = new Vector2(i, j);
                walls.Add(pos);
                addChild(Instantiate(wallTile, pos, Quaternion.identity));
            }
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
