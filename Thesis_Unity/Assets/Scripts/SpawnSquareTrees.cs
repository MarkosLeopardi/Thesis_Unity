using UnityEngine;

public class SpawnSquare : MonoBehaviour
{
    // Array of prefabs to spawn (trees, rocks, etc.)
    public GameObject[] prefabsToSpawn;

    // How many objects to spawn on each side of the square
    public int objectsPerSide = 100;

    // Half of the total size of the square, assuming the full area is 250x250
    // This would be 125 (half of 250)
    public float halfSize = 125f;

    // The "thickness" of the forest frame (how wide the border is around the square)
    public float borderWidth = 10f;

    // Vertical offset for spawning (set to 0 for flat terrain, or adjust for hills)
    public float yOffset = 0f;

    // Range for random scale of spawned objects (trees will vary in size)
    public Vector2 scaleRange = new Vector2(1f, 1.5f);

    // If true, objects will have a random rotation on the Y-axis
    public bool randomizeRotation = true;

    // Start is called when the script is first run
    void Start()
    {
        // Loop over 4 sides of the square (top, bottom, left, right)
        for (int i = 0; i < objectsPerSide; i++)
        {
            // Calculate the percentage "t" of the current index along the side (from 0 to 1)
            // This ensures the objects are evenly spaced along each edge
            float t = (float)i / (objectsPerSide - 1);

            // Calculate positions along each side of the square
            // For each side (bottom, top, left, right) we need to calculate X or Z
            // X and Z values are linearly interpolated from -halfSize to +halfSize
            float x = Mathf.Lerp(-halfSize, halfSize, t);
            float z = Mathf.Lerp(-halfSize, halfSize, t);

            // Call SpawnLine for each edge of the square
            // Bottom edge (spawns objects from (-halfSize-borderWidth, yOffset, -halfSize) to (-halfSize, yOffset, -halfSize))
            SpawnLine(new Vector3(x, yOffset, -halfSize - borderWidth), new Vector3(x, yOffset, -halfSize));

            // Top edge (spawns objects from (x, yOffset, halfSize) to (x, yOffset, halfSize + borderWidth))
            SpawnLine(new Vector3(x, yOffset, halfSize), new Vector3(x, yOffset, halfSize + borderWidth));

            // Left edge (spawns objects from (-halfSize-borderWidth, yOffset, z) to (-halfSize, yOffset, z))
            SpawnLine(new Vector3(-halfSize - borderWidth, yOffset, z), new Vector3(-halfSize, yOffset, z));

            // Right edge (spawns objects from (halfSize, yOffset, z) to (halfSize + borderWidth, yOffset, z))
            SpawnLine(new Vector3(halfSize, yOffset, z), new Vector3(halfSize + borderWidth, yOffset, z));
        }
    }

    // This function spawns objects along a line between two points (from -> to)
    void SpawnLine(Vector3 from, Vector3 to)
    {
        // Randomly interpolate along the line between "from" and "to"
        // The Random.value gives us a value between 0 and 1, so we get a random point along the line
        Vector3 spawnPos = Vector3.Lerp(from, to, Random.value) + transform.position;

        // Select a random prefab from the prefabs list (tree, rock, etc.)
        GameObject prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

        // Instantiate the object at the spawn position with no initial rotation (rotation is randomized later)
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        // If random rotation is enabled, apply a random Y-axis rotation to make it look more natural
        if (randomizeRotation)
        {
            obj.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        }

        // Apply a random scale to the object, within the scale range defined in the Inspector
        float scale = Random.Range(scaleRange.x, scaleRange.y);
        obj.transform.localScale = new Vector3(scale, scale, scale);
    }
}
