using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public float padding = 2.0f; // Extra space around the edges
    public float smoothTime = 0.3f; // Camera movement smoothing
    public float minimumSize = 5.0f; // Minimum orthographic size of the camera
    public Vector2 defaultSize = new Vector3(34f, 20f);

    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    public Level level; // Reference to the Level object

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam)
        {
            Debug.LogError("DynamicCamera script requires a Camera component on the same GameObject.");
        }

        UpdateCameraPositionAndSize(true);
    }

    void LateUpdate()
    {
        if (level == null)
        {
            level = FindObjectOfType<Level>();
        }

        UpdateCameraPositionAndSize(false);
    }

    public void UpdateCameraPositionAndSize(bool instantly)
    {
        // Find all Player objects in the scene
        Player[] players = FindObjectsOfType<Player>();

        Bounds bounds;
        Vector2 levelSize;
        levelSize = level != null ? level.GetCurrentSize() : new Vector2(1f,1f);
        if (level == null)
        {
            bounds = new Bounds(Vector3.zero, defaultSize);
        }
        else if (players.Length == 0)
        {
            Debug.LogWarning("No objects with the tag 'Player' found. Focusing on level size.");

            // Focus on the entire level size if no players are found
            bounds = new Bounds(Vector3.zero, new Vector3(levelSize.x, levelSize.y, 0));
        }
        else if (players.Length == 1)
        {
            bounds = new Bounds(players[0].transform.position, Vector3.zero);
        }
        else
        {
            // Calculate the bounding box that encompasses all players
            bounds = new Bounds(players[0].transform.position, Vector3.zero);
            foreach (Player player in players)
            {
                bounds.Encapsulate(player.transform.position);
            }

            // Expand bounds to include the visible level size
            if (level)
            {
                bounds.Encapsulate(new Vector3(-levelSize.x / 2, -levelSize.y / 2, 0));
                bounds.Encapsulate(new Vector3(levelSize.x / 2, levelSize.y / 2, 0));
            }
        }

        // Center the camera on the bounding box
        Vector3 targetPosition = bounds.center;
        targetPosition.z = transform.position.z; // Maintain current camera Z position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        if (instantly)
        {
            transform.position = targetPosition;
        }

        // Adjust the camera's orthographic size to fit the bounds
        float maxSize = Mathf.Min(levelSize.y / 2, Mathf.Max(bounds.size.x / cam.aspect, bounds.size.y) / 2 + padding);
        cam.orthographicSize = Mathf.Max(minimumSize, Mathf.Lerp(cam.orthographicSize, maxSize, Time.deltaTime / smoothTime));
        if (instantly)
        {
            cam.orthographicSize = Mathf.Max(minimumSize, maxSize);
        }
    }
}