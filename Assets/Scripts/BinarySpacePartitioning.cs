using UnityEngine;

public class BinarySpacePartitioning : MonoBehaviour
{
    [SerializeField] private int maximumDepth = 1;
    [SerializeField] private int minimumRoomSize = 5;
    [SerializeField] private bool viewPartitions = false;
    [SerializeField] private Vector2 rootTopLeftCorner = new(0f, 50f);
    [SerializeField] private Vector2 rootBottomRightCorner = new(50f, 0f);
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject corridorPrefab;

    private int splitVerticallyDefault;

    private void Start()
        => InitializeTree();

    private void Update()
        => RegenerateRooms();

    private void InitializeTree()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        splitVerticallyDefault = Random.Range(0, 2);
        BinarySpacePartitioningNode root = new(rootTopLeftCorner, rootBottomRightCorner);
        root.center = new((root.topLeftCorner.x + root.bottomRightCorner.x) / 2f, (root.topLeftCorner.y + root.bottomRightCorner.y) / 2f);
        SubdivideNode(root, 0);
        ConnectRooms(root, 0);
    }

    private void RegenerateRooms()
    {
        if (!TriggerRegenerateRooms())
            return;

        DeleteCurrentRooms();
        InitializeTree();
    }

    private void DeleteCurrentRooms()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    private bool TriggerRegenerateRooms()
        => Input.GetKeyDown(KeyCode.R);

    private void SubdivideNode(BinarySpacePartitioningNode node, int depth)
    {
        if (depth >= maximumDepth)
        {
            SpawnRoom(node);
            return;
        }

        SplitNode(node, depth);

        if (viewPartitions)
            ViewPartitions(node, depth);

        SubdivideNode(node.left, depth + 1);
        SubdivideNode(node.right, depth + 1);
    }

    private void SplitNode(BinarySpacePartitioningNode node, int depth)
    {
        int currentMinimumSize = minimumRoomSize * (maximumDepth - depth);
        int splitVertically = depth + splitVerticallyDefault % 2;

        if (splitVertically == 1)
            SplitNodeVertically(node, currentMinimumSize);
        else
            SplitNodeHorizontally(node, currentMinimumSize);
    }

    private void SplitNodeVertically(BinarySpacePartitioningNode node, int currentMinimumSize)
    {
        int divisionPoint = (int)Random.Range(node.topLeftCorner.x + currentMinimumSize, node.bottomRightCorner.x - currentMinimumSize);

        BinarySpacePartitioningNode left = new(node.topLeftCorner, new(divisionPoint, node.bottomRightCorner.y));
        BinarySpacePartitioningNode right = new(new(divisionPoint, node.topLeftCorner.y), node.bottomRightCorner);

        left.center = new((left.topLeftCorner.x + left.bottomRightCorner.x) / 2f, (left.topLeftCorner.y + left.bottomRightCorner.y) / 2f);
        right.center = new((right.topLeftCorner.x + right.bottomRightCorner.x) / 2f, (right.topLeftCorner.y + right.bottomRightCorner.y) / 2f);

        node.left = left;
        node.right = right;
    }

    private void SplitNodeHorizontally(BinarySpacePartitioningNode node, int currentMinimumSize)
    {
        int divisionPoint = (int)Random.Range(node.bottomRightCorner.y + currentMinimumSize, node.topLeftCorner.y - currentMinimumSize);

        BinarySpacePartitioningNode top = new(node.topLeftCorner, new(node.bottomRightCorner.x, divisionPoint));
        BinarySpacePartitioningNode bottom = new(new(node.topLeftCorner.x, divisionPoint), node.bottomRightCorner);

        top.center = new((top.topLeftCorner.x + top.bottomRightCorner.x) / 2f, (top.topLeftCorner.y + top.bottomRightCorner.y) / 2f);
        bottom.center = new((bottom.topLeftCorner.x + bottom.bottomRightCorner.x) / 2f, (bottom.topLeftCorner.y + bottom.bottomRightCorner.y) / 2f);

        //top.origin = new(node.topLeft.x + (node.bottomRight.x - node.topLeft.x) / 2f, divisionPoint + (node.topLeft.y - divisionPoint) / 2f);
        //bottom.origin = new(node.topLeft.x + (node.bottomRight.x - node.topLeft.x) / 2f, (divisionPoint - node.bottomRight.y) / 2f);

        node.left = top;
        node.right = bottom;
    }

    private void SpawnRoom(BinarySpacePartitioningNode node)
    {
        int padding = 1;
        int minimumDistanceBetweenPoints = 3;

        Vector2 roomTopLeftCorner = new(Random.Range(node.topLeftCorner.x + padding, node.center.x), Random.Range(node.center.y, node.topLeftCorner.y - padding));
        Vector2 roomBottomRightCorner = new(Random.Range(roomTopLeftCorner.x + minimumDistanceBetweenPoints, node.bottomRightCorner.x - padding), Random.Range(node.bottomRightCorner.y + padding, roomTopLeftCorner.y - minimumDistanceBetweenPoints));

        float roomWidth = Mathf.Abs(roomBottomRightCorner.x - roomTopLeftCorner.x);
        float roomHeight = Mathf.Abs(roomTopLeftCorner.y - roomBottomRightCorner.y);

        // Room relative center.
        //Vector2 roomCenter = new(roomWidth / 2f, roomHeight / 2f);
        //Vector2 roomSpawnPoint = new(roomTopLeftCorner.x + roomCenter.x, roomBottomRightCorner.y + roomCenter.y);
        //GameObject room = Instantiate(roomPrefab, roomSpawnPoint, Quaternion.identity, gameObject.transform);

        GameObject room = Instantiate(roomPrefab, node.center, Quaternion.identity, gameObject.transform);
        room.transform.localScale = new Vector2(roomWidth, roomHeight);
        //PerlinNoiseGenerator perlinNoise = room.GetComponent<PerlinNoiseGenerator>();
        //perlinNoise.width = 128 * (int)roomWidth;
        //perlinNoise.height = 128 * (int)roomHeight;
        //perlinNoise.CreatePerlinNoise();
    }

    private void ViewPartitions(BinarySpacePartitioningNode node, int depth)
    {
        GameObject partLeft = Instantiate(corridorPrefab, node.left.center, Quaternion.identity, gameObject.transform);
        partLeft.transform.localScale = new Vector2(node.left.width, node.left.height);
        partLeft.transform.position = node.left.center;
        partLeft.GetComponent<SpriteRenderer>().color = new(1f, 0f, 0f, .3f);
        partLeft.GetComponent<SpriteRenderer>().sortingOrder = -10;
        partLeft.name = "Left " + depth;
        GameObject partRight = Instantiate(corridorPrefab, node.right.center, Quaternion.identity, gameObject.transform);
        partRight.transform.localScale = new Vector2(node.right.width, node.right.height);
        partRight.transform.position = node.right.center;
        partRight.GetComponent<SpriteRenderer>().color = new(0f, 0f, 1f, .3f);
        partRight.GetComponent<SpriteRenderer>().sortingOrder = -10;
        partRight.name = "Right " + depth;
    }

    private void ConnectRooms(BinarySpacePartitioningNode node, int depth)
    {
        if (depth >= maximumDepth)
            return;

        ConnectRooms(node.left, depth + 1);
        ConnectRooms(node.right, depth + 1);

        if (IsHorizontalConnection(node))
            SpawnHorizontalCorridor(node);
        else
            SpawnVerticalCorridor(node);
    }

    private bool IsHorizontalConnection(BinarySpacePartitioningNode node)
        => node.left.center.y == node.right.center.y;

    private void SpawnHorizontalCorridor(BinarySpacePartitioningNode node)
    {
        GameObject corridor = Instantiate(corridorPrefab, node.left.center, Quaternion.identity, gameObject.transform);
        float corridorWidth = node.right.center.x - node.left.center.x;
        corridor.transform.position = new Vector2(node.left.center.x + corridorWidth / 2f, node.left.center.y);
        corridor.transform.localScale = new Vector2(corridorWidth, 1f);
    }

    private void SpawnVerticalCorridor(BinarySpacePartitioningNode node)
    {
        GameObject corridor = Instantiate(corridorPrefab, node.left.center, Quaternion.identity, gameObject.transform);
        float corridorHeight = node.left.center.y - node.right.center.y;
        corridor.transform.position = new Vector2(node.left.center.x, node.left.center.y - corridorHeight / 2f);
        corridor.transform.localScale = new Vector2(1f, corridorHeight);
    }
}
