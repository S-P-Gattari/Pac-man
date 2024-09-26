using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] sprites;  // 存储精灵Prefab
    public Vector2 tileSize = new Vector2(1, 1);  // 每个tile的大小

    int[,] levelMap =
    {
        { 1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        { 2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        { 2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        { 2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        { 2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        { 2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        { 2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        { 2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        { 2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        { 1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        { 0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        { 0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        { 0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        { 2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        { 0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    void Start()
    {
        GameObject map01 = GameObject.Find("Map01");
        if (map01 != null)
        {
            map01.SetActive(false);
        }
        // 先水平对称，然后垂直对称，生成完整地图
        int[,] fullMap = MirrorVertically(MirrorHorizontally(levelMap));
        GenerateLevel(fullMap);  // 根据完整地图生成关卡
        // 隐藏名为 "Map01" 的组
        
    }

    // 水平对称地图
    int[,] MirrorHorizontally(int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        int[,] mirroredMap = new int[rows, cols * 2];

        // 复制原数组及其水平对称部分
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                mirroredMap[y, x] = map[y, x];  // 左半部分
                mirroredMap[y, mirroredMap.GetLength(1) - 1 - x] = map[y, x];  // 右半部分
            }
        }

        return mirroredMap;
    }

    // 垂直对称地图
    int[,] MirrorVertically(int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
    // 创建垂直镜像后的数组，删除最后一行
        int[,] mirroredMap = new int[(rows * 2) - 1, cols];

        // 复制原数组及其垂直对称部分
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                mirroredMap[y, x] = map[y, x];  // 上半部分
                mirroredMap[mirroredMap.GetLength(0) - 1 - y, x] = map[y, x];  // 下半部分
            }
        }

        return mirroredMap;
    }

    // 生成关卡
    void GenerateLevel(int[,] map)
    {
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                int tileType = map[y, x];
                if (tileType != 0)
                {
                    Vector2 position = new Vector2(x * tileSize.x, -y * tileSize.y); // 紧邻排布，无间隙
                    Quaternion rotation = DetermineRotation(x, y, map);
                    CreateTile(tileType, position, rotation);
                }
            }
        }
    }

    // 创建tile
    void CreateTile(int tileType, Vector2 position, Quaternion rotation)
    {
        GameObject tilePrefab = sprites[tileType - 1];
        Instantiate(tilePrefab, position, rotation);
    }

    // 根据周围tile类型确定旋转角度
    Quaternion DetermineRotation(int x, int y, int[,] map)
    {
        Quaternion rotation = Quaternion.identity;
        int tileType = map[y, x];

        if (tileType == 2 || tileType == 4)  // 墙
        {
            if ((x > 0 && (IsWallOrCorner(x - 1, y, map) || IsCorner(x - 1, y, map))) ||
                (x < map.GetLength(1) - 1 && (IsWallOrCorner(x + 1, y, map) || IsCorner(x + 1, y, map))))
            {
                if (!(y > 0 && (IsWallOrCorner(x, y - 1, map) || IsCorner(x, y - 1, map))) &&
                    !(y < map.GetLength(0) - 1 && (IsWallOrCorner(x, y + 1, map) || IsCorner(x, y + 1, map))))
                {
                    rotation = Quaternion.Euler(0, 0, 0);
                }
            }

            if ((y > 0 && (IsWallOrCorner(x, y - 1, map) || IsCorner(x, y - 1, map))) ||
                (y < map.GetLength(0) - 1 && (IsWallOrCorner(x, y + 1, map) || IsCorner(x, y + 1, map))))
            {
                if (!(x > 0 && (IsWallOrCorner(x - 1, y, map) || IsCorner(x - 1, y, map))) &&
                    !(x < map.GetLength(1) - 1 && (IsWallOrCorner(x + 1, y, map) || IsCorner(x + 1, y, map))))
                {
                    rotation = Quaternion.Euler(0, 0, 90);
                }
            }
        }
        else if (tileType == 1 || tileType == 3)  // 角
        {
            if (x < map.GetLength(1) - 1 && y < map.GetLength(0) - 1 &&
                (IsWallOrCorner(x + 1, y, map) || IsCorner(x + 1, y, map)) &&
                (IsWallOrCorner(x, y + 1, map) || IsCorner(x, y + 1, map)))
            {
                rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (x < map.GetLength(1) - 1 && y > 0 &&
                     (IsWallOrCorner(x + 1, y, map) || IsCorner(x + 1, y, map)) &&
                     (IsWallOrCorner(x, y - 1, map) || IsCorner(x, y - 1, map)))
            {
                rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (x > 0 && y > 0 &&
                     (IsWallOrCorner(x - 1, y, map) || IsCorner(x - 1, y, map)) &&
                     (IsWallOrCorner(x, y - 1, map) || IsCorner(x, y - 1, map)))
            {
                rotation = Quaternion.Euler(0, 0, 180);
            }
            else if (x > 0 && y < map.GetLength(0) - 1 &&
                     (IsWallOrCorner(x - 1, y, map) || IsCorner(x - 1, y, map)) &&
                     (IsWallOrCorner(x, y + 1, map) || IsCorner(x, y + 1, map)))
            {
                rotation = Quaternion.Euler(0, 0, 270);
            }
        }

        return rotation;
    }

    bool IsWallOrCorner(int x, int y, int[,] map)
    {
        int tile = map[y, x];
        return (tile == 2 || tile == 4);
    }

   bool IsCorner(int x, int y, int[,] map)
    {
        int tile = map[y, x];
        return (tile == 1 || tile == 3);
    }
}
