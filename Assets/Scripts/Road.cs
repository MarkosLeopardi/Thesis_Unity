using UnityEngine;
using UnityEngine.Splines;

public class Road : MonoBehaviour
{
    public GameObject splineObject; // Reference to the GameObject containing the SplineContainer component
    public GameObject roadPrefab; // Prefab for the road segment
    public GameObject cornerPrefab; // Prefab for the corner segment
    public GameObject tRoadPrefab; // Prefab for the T-road segment

    private Spline roadSpline; // Reference to the Spline data
    private float roadSegmentLength; // Length of the road segment

    void Start()
    {
        if (splineObject == null)
        {
            Debug.LogError("Spline GameObject is not assigned!");
            return;
        }

        // Get the SplineContainer component from the provided GameObject
        SplineContainer splineContainer = splineObject.GetComponent<SplineContainer>();
        if (splineContainer == null)
        {
            Debug.LogError("No SplineContainer component found on the assigned GameObject!");
            return;
        }

        // Access the Spline data from the SplineContainer
        roadSpline = splineContainer.Spline;

        // Calculate the length of the road segment based on its bounds
        if (roadPrefab != null)
        {
            Renderer roadRenderer = roadPrefab.GetComponent<Renderer>();
            if (roadRenderer != null)
            {
                roadSegmentLength = roadRenderer.bounds.size.z; // Assuming the road's length is along the Z-axis
            }
            else
            {
                Debug.LogError("Road prefab does not have a Renderer component!");
                return;
            }
        }
        else
        {
            Debug.LogError("Road prefab is not assigned!");
            return;
        }

        GenerateRoad();
    }

    void GenerateRoad()
    {
        for (int i = 0; i < roadSpline.Count; i++)
        {
            // Convert local spline positions to world positions
            Vector3 position = splineObject.transform.TransformPoint(roadSpline[i].Position);
            Quaternion rotation = splineObject.transform.rotation * roadSpline[i].Rotation;

            if (i == 0) // First knot
            {
                Instantiate(cornerPrefab, position, rotation * Quaternion.Euler(0, 90, 0));
            }
            else if (i % 2 == 0) // Knots [0, 2, 4, 6, ...]
            {
                Instantiate(cornerPrefab, position, rotation * Quaternion.Euler(0, 180, 0));
            }
            else if (i % 2 == 1) // Knots [1, 3, 5, 7, ...]
            {
                Instantiate(tRoadPrefab, position, rotation * Quaternion.Euler(0, 180, 0));
            }

            // Spawn road segments between this knot and the next one
            if (i < roadSpline.Count - 1)
            {
                SpawnRoadSegmentsBetweenKnots(i, i + 1);
            }
        }
    }

    void SpawnRoadSegmentsBetweenKnots(int startKnotIndex, int endKnotIndex)
    {
        Vector3 startPosition = splineObject.transform.TransformPoint(roadSpline[startKnotIndex].Position);
        Vector3 endPosition = splineObject.transform.TransformPoint(roadSpline[endKnotIndex].Position);

        Vector3 direction = (endPosition - startPosition).normalized;
        float distance = Vector3.Distance(startPosition, endPosition);

        // Start spawning from the edge of the first knot
        Vector3 currentPosition = startPosition + direction * (roadSegmentLength / 2);

        while (distance >= roadSegmentLength)
        {
            Instantiate(roadPrefab, currentPosition, Quaternion.LookRotation(direction));
            currentPosition += direction * roadSegmentLength;
            distance -= roadSegmentLength;
        }
    }
}
