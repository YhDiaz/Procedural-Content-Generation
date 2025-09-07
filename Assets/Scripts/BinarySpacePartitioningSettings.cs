using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioningSettings : MonoBehaviour
{
    public bool viewPartitions = true;
    public bool alternatePartitions = true;
    public int maximumDepth = 1;
    public float partitionCenterBias = 0.15f;
    public float roomCoverageMin = 0.5f;
    public float roomCoverageMax = 0.85f;
    public Vector2 rootTopLeftCorner = new(0f, 50f);
    public Vector2 rootBottomRightCorner = new(50f, 0f);
}
