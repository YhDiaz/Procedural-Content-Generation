using UnityEngine;

public class BinarySpacePartitioning : MonoBehaviour
{
    [SerializeField] private bool viewPartitions = false;
    [SerializeField] private int maximumDepth = 1;
    [SerializeField] private int minimumRoomSize = 5;
    [SerializeField, Range(0.05f, 0.5f)] private float partitionCenterBias = 0.15f;
    [SerializeField, Range(0.1f, 0.95f)] private float roomCoverageMin = 0.5f;
    [SerializeField, Range(0.1f, 0.95f)] private float roomCoverageMax = 0.85f;
    //[SerializeField, Range(0.3f, 0.95f)] private float roomCoverage = 0.8f;
    [SerializeField] private Vector2 rootTopLeftCorner = new(0f, 50f);
    [SerializeField] private Vector2 rootBottomRightCorner = new(50f, 0f);
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject corridorPrefab;

    private int splitVerticallyDefault;

    private void Start()
        => InitializeTree();

    private void Update()
        => RegenerateRooms();

    private void OnValidate()
    {
        if (roomCoverageMin >= roomCoverageMax)
            roomCoverageMin = Mathf.Max(roomCoverageMax - .1f, .1f);
    }

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
        float centerX = (node.topLeftCorner.x + node.bottomRightCorner.x) / 2f;
        float maximumOffsetFromCenter = Mathf.Min((node.bottomRightCorner.x - node.topLeftCorner.x) * partitionCenterBias, currentMinimumSize);
        float minimumDivisionLimit = centerX - maximumOffsetFromCenter;
        float maximumDivisionLimit = centerX + maximumOffsetFromCenter;
        float divisionPoint = Random.Range(minimumDivisionLimit, maximumDivisionLimit);

        BinarySpacePartitioningNode left = new(node.topLeftCorner, new(divisionPoint, node.bottomRightCorner.y));
        BinarySpacePartitioningNode right = new(new(divisionPoint, node.topLeftCorner.y), node.bottomRightCorner);

        left.center = new((left.topLeftCorner.x + left.bottomRightCorner.x) / 2f, (left.topLeftCorner.y + left.bottomRightCorner.y) / 2f);
        right.center = new((right.topLeftCorner.x + right.bottomRightCorner.x) / 2f, (right.topLeftCorner.y + right.bottomRightCorner.y) / 2f);

        node.left = left;
        node.right = right;
    }

    private void SplitNodeHorizontally(BinarySpacePartitioningNode node, int currentMinimumSize)
    {
        float centerY = (node.topLeftCorner.y + node.bottomRightCorner.y) / 2f;
        float maximumOffsetFromCenter = Mathf.Min((node.topLeftCorner.y - node.bottomRightCorner.y) * partitionCenterBias, currentMinimumSize);
        float minimumDivision = centerY - maximumOffsetFromCenter;
        float maximumDivision = centerY + maximumOffsetFromCenter;
        float divisionPoint = Random.Range(minimumDivision, maximumDivision);

        BinarySpacePartitioningNode top = new(node.topLeftCorner, new(node.bottomRightCorner.x, divisionPoint));
        BinarySpacePartitioningNode bottom = new(new(node.topLeftCorner.x, divisionPoint), node.bottomRightCorner);

        top.center = new((top.topLeftCorner.x + top.bottomRightCorner.x) / 2f, (top.topLeftCorner.y + top.bottomRightCorner.y) / 2f);
        bottom.center = new((bottom.topLeftCorner.x + bottom.bottomRightCorner.x) / 2f, (bottom.topLeftCorner.y + bottom.bottomRightCorner.y) / 2f);

        node.left = top;
        node.right = bottom;
    }

    private void SpawnRoom(BinarySpacePartitioningNode node)
    {
        float partitionWidth = Mathf.Abs(node.bottomRightCorner.x - node.topLeftCorner.x);
        float partitionHeight = Mathf.Abs(node.topLeftCorner.y - node.bottomRightCorner.y);

        float coverage = Random.Range(roomCoverageMin, roomCoverageMax);
        float roomWidth = partitionWidth * coverage;
        float roomHeight = partitionHeight * coverage;

        Vector2 roomCenter = node.center;
        Vector2 roomTopLeftCorner = new(roomCenter.x - roomWidth / 2f, roomCenter.y + roomHeight / 2f);
        Vector2 roomBottomRightCorner = new(roomCenter.x + roomWidth / 2f, roomCenter.y - roomHeight / 2f);

        // Room relative center.
        //Vector2 roomCenter = new(roomWidth / 2f, roomHeight / 2f);
        //Vector2 roomSpawnPoint = new(roomTopLeftCorner.x + roomCenter.x, roomBottomRightCorner.y + roomCenter.y);
        //GameObject room = Instantiate(roomPrefab, roomSpawnPoint, Quaternion.identity, gameObject.transform);
        
        GameObject room = Instantiate(roomPrefab, roomCenter, Quaternion.identity, gameObject.transform);
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
