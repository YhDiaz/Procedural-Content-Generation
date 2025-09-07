using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioning : MonoBehaviour
{
    //[SerializeField] private bool viewPartitions = true;
    //[SerializeField] private bool alternatePartitions = true;
    //[SerializeField] private int maximumDepth = 1;
    //[SerializeField, Range(0.05f, 0.5f)] private float partitionCenterBias = 0.15f;
    //[SerializeField, Range(0.1f, 0.95f)] private float roomCoverageMin = 0.5f;
    //[SerializeField, Range(0.1f, 0.95f)] private float roomCoverageMax = 0.85f;
    //[SerializeField] private Vector2 rootTopLeftCorner = new(0f, 50f);
    //[SerializeField] private Vector2 rootBottomRightCorner = new(50f, 0f);
    [SerializeField] private BinarySpacePartitioningSettings settings;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private GameObject corridorPrefab;

    private int splitVerticallyDefault;
    //private PerlinNoiseManager perlinNoiseManager;
    private List<GameObject> rooms = new();

    //private void Start()
    //    => InitializeTree();

    //private void Update()
    //    => RegenerateRooms();

    private void OnValidate()
    {
        if (settings.roomCoverageMin >= settings.roomCoverageMax)
            settings.roomCoverageMin = Mathf.Max(settings.roomCoverageMax - .1f, .1f);
    }
    private void OnEnable()
    {
        PerlinNoiseManager.OnPerlinNoiseModified += UpdateRoomTexturesFromManager;
    }
    private void OnDisable()
    {
        PerlinNoiseManager.OnPerlinNoiseModified -= UpdateRoomTexturesFromManager;
    }

    private void UpdateRoomTexturesFromManager()
    {
        if (Application.isPlaying)
        {
            //if (perlinNoiseManager == null)
            //    perlinNoiseManager = FindObjectOfType<PerlinNoiseManager>();

            //if (perlinNoiseManager == null)
            //    return;

            //int mapWidth = Mathf.RoundToInt(settings.rootBottomRightCorner.x - settings.rootTopLeftCorner.x);
            //int mapHeight = Mathf.RoundToInt(settings.rootTopLeftCorner.y - settings.rootBottomRightCorner.y);
            //perlinNoiseManager.GenerateGlobalGradientMap(mapWidth, mapHeight);

            // Reasignar la textura a cada room
            foreach (var room in rooms)
            {
                if (room == null) continue;
                var sr = room.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                Vector2 roomCenter = room.transform.position;
                Vector2 roomScale = room.transform.localScale;
                Vector2 roomTopLeftCorner = new Vector2(roomCenter.x - roomScale.x / 2f, roomCenter.y + roomScale.y / 2f);

                int globalX = Mathf.RoundToInt(roomTopLeftCorner.x - settings.rootTopLeftCorner.x);
                int globalY = Mathf.RoundToInt(settings.rootTopLeftCorner.y - roomTopLeftCorner.y);
                int regionWidth = Mathf.RoundToInt(roomScale.x);
                int regionHeight = Mathf.RoundToInt(roomScale.y);

                RectInt region = new RectInt(globalX, globalY, regionWidth, regionHeight);

                //Texture2D roomTexture = perlinNoiseManager.GenerateRoomTextureFromGlobalMap(region);
                //sr.sprite = Sprite.Create(roomTexture, new Rect(0, 0, regionWidth, regionHeight), new Vector2(0.5f, 0.5f), pixelsPerUnit: 50);
            }
        }
    }

    private void InitializeTree()
    {
        splitVerticallyDefault = Random.Range(0, 2);
        BinarySpacePartitioningNode root = new(settings.rootTopLeftCorner, settings.rootBottomRightCorner);
        root.center = new((root.topLeftCorner.x + root.bottomRightCorner.x) / 2f, (root.topLeftCorner.y + root.bottomRightCorner.y) / 2f);

        //perlinNoiseManager = FindObjectOfType<PerlinNoiseManager>();
        //int mapWidth = Mathf.RoundToInt(settings.rootBottomRightCorner.x - settings.rootTopLeftCorner.x);
        //int mapHeight = Mathf.RoundToInt(settings.rootTopLeftCorner.y - settings.rootBottomRightCorner.y);
        //perlinNoiseManager.GenerateGlobalGradientMap(mapWidth, mapHeight);

        Random.InitState((int)System.DateTime.Now.Ticks);

        SubdivideNode(root, 0);
        ConnectRooms(root, 0);
    }

    public void GenerateRooms()
    {
        //if (!TriggerRegenerateRooms())
        //    return;

        DeleteCurrentRooms();
        InitializeTree();
    }

    private void DeleteCurrentRooms()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        rooms.Clear();
    }

    private bool TriggerRegenerateRooms()
        => Input.GetKeyDown(KeyCode.R);

    private void SubdivideNode(BinarySpacePartitioningNode node, int depth)
    {
        if (depth >= settings.maximumDepth)
        {
            SpawnRoom(node);
            return;
        }

        SplitNode(node, depth);

        if (settings.viewPartitions)
            ViewPartitions(node, depth);

        SubdivideNode(node.left, depth + 1);
        SubdivideNode(node.right, depth + 1);
    }

    private void SplitNode(BinarySpacePartitioningNode node, int depth)
    {
        int splitVertically = settings.alternatePartitions ? (depth + splitVerticallyDefault) % 2
                                                  : Random.Range(0, 2);

        if (splitVertically == 1)
            SplitNodeVertically(node);
        else
            SplitNodeHorizontally(node);
    }

    private void SplitNodeVertically(BinarySpacePartitioningNode node)
    {
        float centerX = (node.topLeftCorner.x + node.bottomRightCorner.x) / 2f;
        float maximumOffsetFromCenter = (node.bottomRightCorner.x - node.topLeftCorner.x) * settings.partitionCenterBias;
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

    private void SplitNodeHorizontally(BinarySpacePartitioningNode node)
    {
        float centerY = (node.topLeftCorner.y + node.bottomRightCorner.y) / 2f;
        float maximumOffsetFromCenter = (node.topLeftCorner.y - node.bottomRightCorner.y) * settings.partitionCenterBias;
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

        float coverage = Random.Range(settings.roomCoverageMin, settings.roomCoverageMax);
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

        int globalX = Mathf.RoundToInt(roomTopLeftCorner.x - settings.rootTopLeftCorner.x);
        int globalY = Mathf.RoundToInt(settings.rootTopLeftCorner.y - roomTopLeftCorner.y);
        int regionWidth = Mathf.RoundToInt(roomWidth);
        int regionHeight = Mathf.RoundToInt(roomHeight);

        RectInt region = new RectInt(globalX, globalY, regionWidth, regionHeight);

        // Genera la textura de la room a partir del mapa global
        //Texture2D roomTexture = perlinNoiseManager.GenerateRoomTextureFromGlobalMap(region);
        //room.GetComponent<SpriteRenderer>().sprite = Sprite.Create(roomTexture, new Rect(0, 0, regionWidth, regionHeight), new Vector2(0.5f, 0.5f), pixelsPerUnit: 50) ;

        rooms.Add(room);
    }

    private void ViewPartitions(BinarySpacePartitioningNode node, int depth)
    {
        GameObject partLeft = Instantiate(corridorPrefab, node.left.center, Quaternion.identity, gameObject.transform);
        partLeft.transform.localScale = new Vector2(node.left.width, node.left.height);
        partLeft.transform.position = node.left.center;
        partLeft.GetComponent<SpriteRenderer>().color = new(1f, 0f, 0f, .3f);
        partLeft.GetComponent<SpriteRenderer>().sortingOrder = -30;
        partLeft.name = "Left " + depth;

        GameObject partRight = Instantiate(corridorPrefab, node.right.center, Quaternion.identity, gameObject.transform);
        partRight.transform.localScale = new Vector2(node.right.width, node.right.height);
        partRight.transform.position = node.right.center;
        partRight.GetComponent<SpriteRenderer>().color = new(0f, 0f, 1f, .3f);
        partRight.GetComponent<SpriteRenderer>().sortingOrder = -30;
        partRight.name = "Right " + depth;
    }

    private void ConnectRooms(BinarySpacePartitioningNode node, int depth)
    {
        if (depth >= settings.maximumDepth)
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
