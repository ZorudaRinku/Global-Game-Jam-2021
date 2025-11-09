using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerationManager : MonoBehaviour
{
    [SerializeField] private GameObject[] roomPrefabs; // Assign in Inspector
    Dictionary<Vector2Int, GameObject> generatedRooms = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, Vector2Int> doorPositions = new Dictionary<Vector2Int, Vector2Int>();
    // Track adjacencies: roomPos -> list of (adjacentRoomPos, direction)
    Dictionary<Vector2Int, List<(Vector2Int adjacentPos, Vector2Int direction)>> adjacencyMap = new Dictionary<Vector2Int, List<(Vector2Int, Vector2Int)>>();
    [SerializeField] private Tile DoorNorthTile;
    [SerializeField] private Tile DoorEastTile;
    [SerializeField] private Tile DoorSouthTile;
    [SerializeField] private Tile DoorWestTile;
    private const int STAR_COUNT = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Loaded " + roomPrefabs.Length + " room prefabs.");
        if (roomPrefabs.Length > 0)
        {
            // Generate first room at origin
            GameObject firstRoom = GenerateRoom(roomPrefabs[Random.Range(0, roomPrefabs.Length)], Vector2Int.zero, null, null, false);
            generatedRooms.Add(Vector2Int.zero, firstRoom);
            adjacencyMap[Vector2Int.zero] = new List<(Vector2Int, Vector2Int)>();

            // Continue generating rooms until we have enough
            int attemptsWithoutSuccess = 0;
            int maxAttemptsWithoutSuccess = STAR_COUNT * 10; // Give up after many failed attempts

            while (generatedRooms.Count < STAR_COUNT && attemptsWithoutSuccess < maxAttemptsWithoutSuccess)
            {
                // Pick a random existing room to try spawning from
                int randomRoomIndex = Random.Range(0, generatedRooms.Count);
                var currentRoomEntry = generatedRooms.ElementAt(randomRoomIndex);
                GameObject currentRoom = currentRoomEntry.Value;
                Vector2Int currentGridPos = currentRoomEntry.Key;

                // Try multiple times to place a room from this position
                bool roomPlaced = false;
                int attempts = 0;
                int maxAttempts = 10; // Limit attempts per room to avoid infinite loops

                while (!roomPlaced && attempts < maxAttempts && generatedRooms.Count < STAR_COUNT)
                {
                    attempts++;

                    // Pick a random direction from the room to navigate to next
                    List<Vector2Int> possibleDirections = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                    GameObject newRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

                    // Get the CURRENT room's tilemap size to calculate offset
                    Tilemap currentRoomTilemap = currentRoom.transform.GetChild(0).GetComponent<Tilemap>();
                    BoundsInt currentRoomBounds = currentRoomTilemap.cellBounds;

                    // Pick a random direction
                    Vector2Int chosenDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];

                    // Get the NEW room's bounds for calculating proper placement
                    Tilemap newRoomTilemap = newRoomPrefab.transform.GetChild(0).GetComponent<Tilemap>();
                    newRoomTilemap.CompressBounds();
                    BoundsInt newRoomBounds = newRoomTilemap.cellBounds;

                    Vector2Int newRoomGridPos = CalculateNewRoomGridPosition(currentGridPos, chosenDirection, currentRoomBounds, newRoomBounds);

                    // Check for overlap before generating
                    if (!WouldRoomOverlap(newRoomGridPos, newRoomBounds))
                    {
                        GameObject generatedRoom = GenerateRoom(newRoomPrefab, newRoomGridPos, null, null, false);
                        generatedRooms.Add(newRoomGridPos, generatedRoom);

                        // Track adjacency relationship
                        if (!adjacencyMap.ContainsKey(currentGridPos))
                            adjacencyMap[currentGridPos] = new List<(Vector2Int, Vector2Int)>();
                        if (!adjacencyMap.ContainsKey(newRoomGridPos))
                            adjacencyMap[newRoomGridPos] = new List<(Vector2Int, Vector2Int)>();

                        adjacencyMap[currentGridPos].Add((newRoomGridPos, chosenDirection));
                        adjacencyMap[newRoomGridPos].Add((currentGridPos, -chosenDirection)); // Opposite direction

                        roomPlaced = true;
                        attemptsWithoutSuccess = 0; // Reset counter on success
                    }
                }

                if (!roomPlaced)
                {
                    attemptsWithoutSuccess++;
                }
            }
            Debug.Log($"Generated {generatedRooms.Count} rooms total.");
        }
        else
            Debug.LogError("No room prefabs assigned! Please assign them in the Inspector.");

        // After all rooms are generated, place doors using adjacency map
        foreach (var roomEntry in generatedRooms)
        {
            Vector2Int roomGridPos = roomEntry.Key;
            GameObject roomObject = roomEntry.Value;
            Tilemap wallTilemap = roomObject.transform.GetChild(0).GetComponent<Tilemap>();
            wallTilemap.CompressBounds();
            BoundsInt roomBounds = wallTilemap.cellBounds;
            Tilemap groundTilemap = roomObject.transform.GetChild(1).GetComponent<Tilemap>();

            // Check for adjacent rooms using adjacency map (O(1) lookup instead of O(n))
            PlaceDoorsEfficient(roomBounds, roomGridPos, wallTilemap, groundTilemap);
        }

        // Verify all doors have a companion door adjacent to itself (only once, after all doors are placed)
        VerifyDoors();
    }

    private void VerifyDoors()
    {
        Debug.Log(doorPositions);
        foreach (var doorEntry in doorPositions)
        {
            if (!CheckForAdjacentDoors(doorEntry))
            {
                Debug.LogWarning($"Door at {doorEntry.Key} has no adjacent companion door!");
            }
        }
    }

    private bool CheckForAdjacentDoors(KeyValuePair<Vector2Int, Vector2Int> doorEntry)
    {
        foreach (var adjacent in doorPositions)
        {
            if ((doorEntry.Key - adjacent.Key).magnitude <= 1)
            {
                return true;
            }
        }
        return false;
    }

    private void PlaceDoorsEfficient(BoundsInt roomBounds, Vector2Int roomGridPos, Tilemap wallTilemap, Tilemap groundTilemap)
    {
        // Use adjacency map - O(adjacentRooms) instead of O(totalRooms)
        if (!adjacencyMap.ContainsKey(roomGridPos))
            return;

        foreach (var (adjacentPos, direction) in adjacencyMap[roomGridPos])
        {
            GameObject otherRoom = generatedRooms[adjacentPos];
            Tilemap otherTilemap = otherRoom.transform.GetChild(0).GetComponent<Tilemap>();
            BoundsInt otherBounds = otherTilemap.cellBounds;

            // Calculate world bounds for both rooms
            BoundsInt worldBounds = new BoundsInt(roomGridPos.x + roomBounds.x, roomGridPos.y + roomBounds.y, 0, roomBounds.size.x, roomBounds.size.y, 1);
            BoundsInt otherWorldBounds = new BoundsInt(adjacentPos.x + otherBounds.x, adjacentPos.y + otherBounds.y, 0, otherBounds.size.x, otherBounds.size.y, 1);

            Tile doorTile = GetDoorTileForDirection(direction);
            // Calculate door position in world space (between the two rooms)
            Vector2Int doorWorldPos = CalculateDoorWorldPosition(worldBounds, otherWorldBounds, direction);

            if (doorPositions.ContainsKey(doorWorldPos))
            {
                // Door already placed by the adjacent room
                continue;
            }

            // Convert world position to tilemap cell coordinates
            Vector2Int doorLocalPos = new Vector2Int(
                doorWorldPos.x - roomGridPos.x,
                doorWorldPos.y - roomGridPos.y
            );

            Debug.Log($"Room at {roomGridPos}, bounds: {roomBounds}, placing door at tilemap cell {doorLocalPos} (world {doorWorldPos}) towards {direction}");
            doorPositions.Add(doorWorldPos, direction);
            groundTilemap.SetTile(new Vector3Int(doorLocalPos.x, doorLocalPos.y, 0), doorTile);
            wallTilemap.SetTile(new Vector3Int(doorLocalPos.x, doorLocalPos.y, 0), null);
        }
    }

    private Vector2Int CalculateNewRoomGridPosition(Vector2Int currentGridPos, Vector2Int doorDirection, BoundsInt currentRoomBounds, BoundsInt newRoomBounds)
    {
        // Calculate the position where rooms should be flush against each other
        Vector2Int offset = Vector2Int.zero;

        if (doorDirection == Vector2Int.right)
        {
            // Current room's right edge + new room's left edge offset
            offset.x = currentRoomBounds.xMax - newRoomBounds.x;
            // Center-align vertically
            int currentCenterY = currentRoomBounds.y + currentRoomBounds.size.y / 2;
            int newCenterY = newRoomBounds.y + newRoomBounds.size.y / 2;
            offset.y = currentCenterY - newCenterY;
        }
        else if (doorDirection == Vector2Int.left)
        {
            // Current room's left edge - new room's right edge offset
            offset.x = currentRoomBounds.x - (newRoomBounds.x + newRoomBounds.size.x);
            // Center-align vertically
            int currentCenterY = currentRoomBounds.y + currentRoomBounds.size.y / 2;
            int newCenterY = newRoomBounds.y + newRoomBounds.size.y / 2;
            offset.y = currentCenterY - newCenterY;
        }
        else if (doorDirection == Vector2Int.up)
        {
            // Current room's top edge + new room's bottom edge offset
            offset.y = currentRoomBounds.yMax - newRoomBounds.y;
            // Center-align horizontally
            int currentCenterX = currentRoomBounds.x + currentRoomBounds.size.x / 2;
            int newCenterX = newRoomBounds.x + newRoomBounds.size.x / 2;
            offset.x = currentCenterX - newCenterX;
        }
        else if (doorDirection == Vector2Int.down)
        {
            // Current room's bottom edge - new room's top edge offset
            offset.y = currentRoomBounds.y - (newRoomBounds.y + newRoomBounds.size.y);
            // Center-align horizontally
            int currentCenterX = currentRoomBounds.x + currentRoomBounds.size.x / 2;
            int newCenterX = newRoomBounds.x + newRoomBounds.size.x / 2;
            offset.x = currentCenterX - newCenterX;
        }

        return currentGridPos + offset;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private GameObject GenerateRoom(GameObject roomPrefab, Vector2Int gridPosition, Vector2Int? returnDoorDirection, Vector2Int? previousDoorWorldPos, bool hasEnoughRooms)
    {
        // Get room size from prefab
        Tilemap prefabTilemap = roomPrefab.transform.GetChild(0).GetComponent<Tilemap>();

        // Compress bounds to only include tiles that actually exist
        prefabTilemap.CompressBounds();
        BoundsInt bounds = prefabTilemap.cellBounds;

        // Calculate world position from grid position
        // Grid position represents the bottom-left corner of the room
        Vector3 worldPosition = new Vector3(gridPosition.x, gridPosition.y, 0);

        GameObject roomInstance = Instantiate(roomPrefab, worldPosition, Quaternion.identity, transform);

        return roomInstance;
    }

    private Vector2Int CalculateDoorWorldPosition(BoundsInt worldBounds, BoundsInt otherWorldBounds, Vector2Int direction)
    {
        // Calculate the door position in world space at the center of the overlap between rooms
        Vector2Int doorWorldPos = Vector2Int.zero;

        if (direction == Vector2Int.up || direction == Vector2Int.down)
        {
            // Find the overlapping X range and place door at center
            int overlapMin = Mathf.Max(worldBounds.xMin, otherWorldBounds.xMin);
            int overlapMax = Mathf.Min(worldBounds.xMax, otherWorldBounds.xMax);
            // overlapMax is exclusive, so subtract 1 to get the last valid cell, then find center
            int centerX = (overlapMin + overlapMax - 1) / 2;

            if (direction == Vector2Int.up)
                doorWorldPos = new Vector2Int(centerX, worldBounds.yMax - 1);
            else // down
                doorWorldPos = new Vector2Int(centerX, worldBounds.yMin);
        }
        else // left or right
        {
            // Find the overlapping Y range and place door at center
            int overlapMin = Mathf.Max(worldBounds.yMin, otherWorldBounds.yMin);
            int overlapMax = Mathf.Min(worldBounds.yMax, otherWorldBounds.yMax);
            // overlapMax is exclusive, so subtract 1 to get the last valid cell, then find center
            int centerY = (overlapMin + overlapMax - 1) / 2;

            if (direction == Vector2Int.right)
                doorWorldPos = new Vector2Int(worldBounds.xMax - 1, centerY);
            else // left
                doorWorldPos = new Vector2Int(worldBounds.xMin, centerY);
        }

        return doorWorldPos;
    }

    private Vector2Int CalculateAlignedDoorPosition(BoundsInt bounds, Vector2Int direction, Vector2Int previousDoorWorldPos, Vector2Int newRoomGridPos)
    {
        // Calculate door position in local coordinates that aligns with the previous room's door
        Vector2Int localDoorPos = Vector2Int.zero;

        if (direction == Vector2Int.up)
        {
            // Door on top wall - align X coordinate with previous door
            localDoorPos = new Vector2Int(previousDoorWorldPos.x - newRoomGridPos.x, bounds.yMax - 1);
        }
        else if (direction == Vector2Int.down)
        {
            // Door on bottom wall - align X coordinate with previous door
            localDoorPos = new Vector2Int(previousDoorWorldPos.x - newRoomGridPos.x, bounds.y);
        }
        else if (direction == Vector2Int.left)
        {
            // Door on left wall - align Y coordinate with previous door
            localDoorPos = new Vector2Int(bounds.x, previousDoorWorldPos.y - newRoomGridPos.y);
        }
        else if (direction == Vector2Int.right)
        {
            // Door on right wall - align Y coordinate with previous door
            localDoorPos = new Vector2Int(bounds.xMax - 1, previousDoorWorldPos.y - newRoomGridPos.y);
        }

        return localDoorPos;
    }

    private Tile GetDoorTileForDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            return DoorNorthTile;
        else if (direction == Vector2Int.down)
            return DoorSouthTile;
        else if (direction == Vector2Int.left)
            return DoorWestTile;
        else if (direction == Vector2Int.right)
            return DoorEastTile;

        return null;
    }

    private bool WouldRoomOverlap(Vector2Int newRoomGridPos, BoundsInt newRoomBounds)
    {
        // Calculate the world space bounds of the new room
        BoundsInt newRoomWorldBounds = new BoundsInt(
            newRoomGridPos.x + newRoomBounds.x,
            newRoomGridPos.y + newRoomBounds.y,
            0,
            newRoomBounds.size.x,
            newRoomBounds.size.y,
            1
        );

        // Check against all existing rooms
        foreach (var existingRoomEntry in generatedRooms)
        {
            GameObject existingRoom = existingRoomEntry.Value;
            Vector2Int existingGridPos = existingRoomEntry.Key;

            // Check all tilemaps in the existing room
            for (int i = 0; i < existingRoom.transform.childCount; i++)
            {
                Tilemap existingTilemap = existingRoom.transform.GetChild(i).GetComponent<Tilemap>();
                if (existingTilemap == null) continue;

                BoundsInt existingBounds = existingTilemap.cellBounds;

                // Calculate world space bounds for existing room
                BoundsInt existingWorldBounds = new BoundsInt(
                    existingGridPos.x + existingBounds.x,
                    existingGridPos.y + existingBounds.y,
                    0,
                    existingBounds.size.x,
                    existingBounds.size.y,
                    1
                );

                // Check if bounds intersect
                if (newRoomWorldBounds.xMin < existingWorldBounds.xMax &&
                    newRoomWorldBounds.xMax > existingWorldBounds.xMin &&
                    newRoomWorldBounds.yMin < existingWorldBounds.yMax &&
                    newRoomWorldBounds.yMax > existingWorldBounds.yMin)
                {
                    return true; // Overlap detected
                }
            }
        }

        return false; // No overlap
    }
}
