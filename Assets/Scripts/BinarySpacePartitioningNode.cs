using UnityEngine;

public class BinarySpacePartitioningNode
{
    public float width;
    public float height;

    public Vector2 topLeftCorner;
    public Vector2 bottomRightCorner;
    public Vector2 center;

    public BinarySpacePartitioningNode left;
    public BinarySpacePartitioningNode right;

    public BinarySpacePartitioningNode(Vector2 topLeft, Vector2 bottomRight)
    {
        this.topLeftCorner = topLeft;
        this.bottomRightCorner = bottomRight;

        width = bottomRight.x - topLeft.x;
        height = topLeft.y - bottomRight.y;
    }
}
