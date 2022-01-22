using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject mob;
    [SerializeField] private GameObject mushroom;

    [SerializeField] private int mobPerLevel = 10;
    [SerializeField] private int mushroomsPerLevel = 20;
    [SerializeField] private int coinsPerLevel = 20;

    private int width = 16;
    private List<int> length = new List<int> { 8, 192, 202 };
    private List<int> corridor1 = new List<int> { 8, 1, 7 };
    private List<int> corridor2 = new List<int> { 7, 1, 8 };
    private List<int> room1 = new List<int> { 2, 12, 2 };
    private List<int> room2 = new List<int> { 6, 4, 6 };
    private List<Vector2> walls = new List<Vector2>();
    private List<Vector2> valid4Spawning = new List<Vector2>();
    public List<Vector2> Walls
    {
        get { return walls; }
    }
    private List<GameObject> mobs = new List<GameObject>();
    public List<GameObject> Mobs
    {
        get { return mobs; }
    }
    //private System.Random rng = new System.Random();

    public List<Vector2> generateDungeon(int level)
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
        int low = Random.Range(0, 2) + 1;
        int height = Random.Range(0, 7) + low;
        bool lockedOnRoom = false;
        int nextRoomAt = 1 + length[0] + Random.Range(0, 4);
        for (int j = 0 + length[0]; j < length[1]; j++)
        {
            if (!lockedOnRoom && nextRoomAt == j)
            {
                low = j;
                height = Random.Range(0, 7) + low + 3;
                List<int> room = (nextRoomAt % 2 == 0) ? room1 : room2;
                wallLeft = room[0];
                corridor = room[1];
                wallRight = room[2];
                lockedOnRoom = true;
            }
            else if (j > height)
            {
                lockedOnRoom = false;
                nextRoomAt = j + Random.Range(0, 4);
                List<int> newCorridor = (height % 2 == 0) ? corridor1 : corridor2;
                wallLeft = newCorridor[0];
                corridor = newCorridor[1];
                wallRight = newCorridor[2];
            }
            BuildRoom(wallLeft, corridor, wallRight, lockedOnRoom, j);
        }
        // Last room
        List<int> finalRoom = room1;
        wallLeft = finalRoom[0];
        corridor = finalRoom[1];
        wallRight = finalRoom[2];
        lockedOnRoom = true;
        for (int j = 0 + length[1]; j < length[2]; j++)
        {
                BuildRoom(wallLeft, corridor, wallRight, lockedOnRoom, j);
        }
        // Last wall, no exit
        BuildRoom(16, 0, 0, false, length[2]);
        spawnMobsAndItems();

        return walls;
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
                // if (Random.Range(0, 100) <= 5)
                // {
                //     addChild(Instantiate(coin, new Vector3(i, j, -1), Quaternion.identity));
                // }
                // else if (Random.Range(0, 100) < 2)
                // {
                //     var mob = Instantiate(this.mob, new Vector3(i, j, -1), Quaternion.identity);
                //     mobs.Add(mob);
                //     addChild(mob);

                // }
                valid4Spawning.Add(new Vector2(i, j));
            }
        }
        for (int i = wallLeft + corridor; i < wallLeft + corridor + wallRight; i++)
        {
            var pos = new Vector2(i, j);
            walls.Add(pos);
            addChild(Instantiate(wall, pos, Quaternion.identity));
        }

    }

    private void spawnMobsAndItems()
    {
        // Spawn mobs and coins
        for (int i = 0; i < mobPerLevel; i++)
        {
            Vector3 location = getNewSpawnLocation();
            var mob = Instantiate(this.mob, location, Quaternion.identity);
            mobs.Add(mob);
            addChild(mob);
        }
        for (int i = 0; i < coinsPerLevel; i++)
        {
            Vector3 location = getNewSpawnLocation();
            var coin = Instantiate(this.coin, location, Quaternion.identity);
            addChild(coin);
        }
        for (int i = 0; i < mushroomsPerLevel; i++)
        {
            Vector3 location = getNewSpawnLocation();
            var mushroom = Instantiate(this.mushroom, location, Quaternion.identity);
            addChild(mushroom);
        }
    }

    private Vector3 getNewSpawnLocation()
    {
        int index = Random.Range(0, valid4Spawning.Count);
        Vector3 loc = new Vector3(valid4Spawning[index].x, valid4Spawning[index].y, -1);
        valid4Spawning.RemoveAt(index);
        return loc;
    }
    private void addChild(GameObject go)
    {
        go.transform.parent = transform;
    }

}