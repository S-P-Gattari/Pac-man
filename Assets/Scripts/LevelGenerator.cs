using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] sprites;  // 存储精灵Prefab
    public Vector2 tileSize = new Vector2(1, 1);  // 每个tile的大小
    private Camera mainCamera;


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
        mainCamera = Camera.main; // 获取 Main Camera

        GameObject map01 = GameObject.Find("Map01");   // 隐藏名为 "Map01" 的组
        if (map01 != null)
        {
            map01.SetActive(false);
        }
        // 先水平对称，然后垂直对称，生成完整地图
        int[,] fullMap = MirrorVertically(MirrorHorizontally(levelMap));
        GenerateLevel(fullMap);  // 根据完整地图生成关卡


        // 调整摄像机大小和位置
        AdjustCamera(fullMap);  // 使用 fullMap 调整摄像机

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
                CreateTile(tileType, position, rotation, map, x, y); 
                }
            }
        }
    

    }

    // 创建tile
void CreateTile(int tileType, Vector2 position, Quaternion rotation, int[,] map, int x, int y)
{
    GameObject tilePrefab = sprites[tileType - 1];
    GameObject tile = Instantiate(tilePrefab, position, rotation);

    // 判断T型的缩放逻辑
    if (tileType == 7) // T型
    {
        if (
            // 左边和右边是墙、角或T型
            ((x > 0 && IsWallOrCornerOrTShape(x - 1, y, map)) &&
             (x < map.GetLength(1) - 1 && IsWallOrCornerOrTShape(x + 1, y, map))) &&
            // 上面是墙或角
            (y > 0 && (IsWallOrCorner(x, y - 1, map) || IsCorner(x, y - 1, map))))
        {
            // 如果满足条件，Y轴缩放设为 -1
            tile.transform.localScale = new Vector3(tile.transform.localScale.x, -1, tile.transform.localScale.z);
        }
    }
}


// 根据周围tile类型确定旋转角度
// 根据周围tile类型确定旋转角度
Quaternion DetermineRotation(int x, int y, int[,] map)
{
    Quaternion rotation = Quaternion.identity;
    int tileType = map[y, x];

    if (tileType == 2 || tileType == 4)  // 墙
    {
        // 增加的逻辑：如果墙的左边和右边是角、墙或T型，同时上或下有墙，则保持水平
        if (
            // 左边和右边是角、墙或T型
            ((x > 0 && IsWallOrCornerOrTShape(x - 1, y, map)) && 
             (x < map.GetLength(1) - 1 && IsWallOrCornerOrTShape(x + 1, y, map))) &&
            // 上或下有墙
            ((y > 0 && IsWallOrTShape(x, y - 1, map)) || 
             (y < map.GetLength(0) - 1 && IsWallOrTShape(x, y + 1, map))))
        {
            // 保持水平（角度为0）
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // 其他情况：默认旋转为垂直
            if (
                // 上或下是墙或T型
                ((y > 0 && (IsWallOrTShape(x, y - 1, map))) || 
                 (y < map.GetLength(0) - 1 && (IsWallOrTShape(x, y + 1, map)))) &&
                // 左或右有墙
                ((x > 0 && IsWall(x - 1, y, map)) || 
                 (x < map.GetLength(1) - 1 && IsWall(x + 1, y, map))))
            {
                // 垂直旋转
                rotation = Quaternion.Euler(0, 0, 90);
            }
        }

        if ((x > 0 && (IsWallOrCorner(x - 1, y, map) || IsCorner(x - 1, y, map))) ||
            (x < map.GetLength(1) - 1 && (IsWallOrCorner(x + 1, y, map) || IsCorner(x + 1, y, map))))
        {
            if (!(y > 0 && (IsWallOrCorner(x, y - 1, map) || IsCorner(x, y - 1, map))) &&
                !(y < map.GetLength(0) - 1 && (IsWallOrCorner(x, y + 1, map) || IsCorner(x, y + 1, map))))
            {
                rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        if (
            // 上和下是墙
            ((y > 0 && IsWall(x, y - 1, map)) &&
             (y < map.GetLength(0) - 1 && IsWall(x, y + 1, map))) &&
            // 左边或右边是角
            ((x > 0 && IsCorner(x - 1, y, map)) || 
             (x < map.GetLength(1) - 1 && IsCorner(x + 1, y, map))))
        {
            // 垂直旋转
            rotation = Quaternion.Euler(0, 0, 90);
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
        if (IsThreeWallsWithOneCorner(x, y, map))
        {
            rotation = DetermineCornerRotation(x, y, map);
        }

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



// 判断tile是否是墙、角或T型
bool IsWallOrCornerOrTShape(int x, int y, int[,] map)
{
    int tile = map[y, x];
    return (tile == 1 || tile == 2 || tile == 3 || tile == 4 || tile == 7);  // 1, 2, 3, 4 表示墙或角, 7 表示 T型
}

// 判断tile是否是墙或T型
bool IsWallOrTShape(int x, int y, int[,] map)
{
    int tile = map[y, x];
    return (tile == 2 || tile == 4 || tile == 7);  // 2, 4 表示墙, 7 表示 T 型
}

bool IsWall(int x, int y, int[,] map)
{
    int tile = map[y, x];
    return (tile == 2 || tile == 4);  // 2 和 4 表示墙
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

bool IsThreeWallsWithOneCorner(int x, int y, int[,] map)
{
    int wallCount = 0;
    int cornerCount = 0;

    // 上方
    if (y > 0 && IsWall(x, y - 1, map)) wallCount++;
    // 下方
    if (y < map.GetLength(0) - 1 && IsWall(x, y + 1, map)) wallCount++;
    // 左方
    if (x > 0 && IsWall(x - 1, y, map)) wallCount++;
    // 右方
    if (x < map.GetLength(1) - 1 && IsWall(x + 1, y, map)) wallCount++;

    // 判断是否有一面是角
    if (x > 0 && IsCorner(x - 1, y, map)) cornerCount++;
    if (x < map.GetLength(1) - 1 && IsCorner(x + 1, y, map)) cornerCount++;
    if (y > 0 && IsCorner(x, y - 1, map)) cornerCount++;
    if (y < map.GetLength(0) - 1 && IsCorner(x, y + 1, map)) cornerCount++;

    return wallCount == 3 && cornerCount == 1;
}

// 根据墙的夹角确定角的旋转方向
Quaternion DetermineCornerRotation(int x, int y, int[,] map)
{
    // 情况 1：上、右是墙，左或下是角
    if (IsWall(x, y - 1, map) && IsWall(x + 1, y, map))
    {
        return Quaternion.Euler(0, 0, -90);  // 顺时针旋转90度
    }
    // 情况 2：右、下是墙，左或上是角
    else if (IsWall(x + 1, y, map) && IsWall(x, y + 1, map))
    {
        return Quaternion.Euler(0, 0, 180);  // 顺时针旋转180度
    }
    // 情况 3：下、左是墙，右或上是角
    else if (IsWall(x, y + 1, map) && IsWall(x - 1, y, map))
    {
        return Quaternion.Euler(0, 0, 90);  // 逆时针旋转90度
    }
    // 情况 4：左、上是墙，右或下是角
    else if (IsWall(x - 1, y, map) && IsWall(x, y - 1, map))
    {
        return Quaternion.Euler(0, 0, 0);  // 默认旋转0度
    }

    return Quaternion.Euler(0, 0, 0);  // 默认不旋转
}
    void AdjustCamera(int[,] map)
{
    float mapWidth = map.GetLength(1) * tileSize.x;
    float mapHeight = map.GetLength(0) * tileSize.y;

    // 设定边框大小，单位是 tile 的大小
    float borderSize = 2.0f; // 边框大小

    // 计算关卡中心
    Vector3 mapCenter = new Vector3(mapWidth / 2, -mapHeight / 2, -10);

    // 设置摄像机位置
    mainCamera.transform.position = mapCenter;

    // 调整摄像机大小，加入边框
    if (mapWidth >= mapHeight)
    {
        mainCamera.orthographicSize = (mapWidth + borderSize) / (2 * mainCamera.aspect);
    }
    else
    {
        mainCamera.orthographicSize = (mapHeight + borderSize) / 2;
    }
}



}
