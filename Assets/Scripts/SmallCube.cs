using UnityEngine;

public class SmallCube : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer cubeMeshRenderer;

    private float lifespan;

    private float currentTimer;

    private Color startColour;

    private readonly Color endColour = new Vector4(0f, 0f, 0f, 0f);

    private bool isRunning;

    /// <summary>
    /// This initialises the single small cube with following paramenters.
    /// </summary>
    /// <param name="spawnPosition">The position of the cube at spawn.</param>
    /// <param name="spawnScale">The size of the cube at spawn.</param>
    /// <param name="spawnColour">The colour of the cube at spawn.</param>
    /// <param name="lifespan">The duration, or "life", of the spawned cube.</param>
    public void Init(Vector3 spawnPosition, Vector3 spawnScale, Color spawnColour, float lifespan)
    {
        transform.localPosition = spawnPosition;

        // I've chosen to interpret the "rotate in sync" so the cubes will rotate 
        // around the parent cube's axis, while the previous time 
        // they were rotating around their own axis but with the parent cube's direction and speed
        transform.localRotation = Quaternion.identity;
        transform.localScale = spawnScale;

        this.lifespan = lifespan;
        this.startColour = spawnColour;

        currentTimer = 0f;

        isRunning = true;
    }

    public void StopRunning()
    {
        isRunning = false;
    }

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }

        // Cube ages and fades away as soon as it's spawned
        // The manager will pool the expired cube and replace it with another
        currentTimer += Time.deltaTime;

        var cubeColour = Color.Lerp(startColour, endColour, currentTimer / lifespan);

        cubeMeshRenderer.material.color = cubeColour;

        if (currentTimer > lifespan)
        {
            gameObject.SetActive(false);
        }
    }
}
