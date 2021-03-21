using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    // I made sure every detail of the smaller cubes could be tweaked in the Editor
    // Designer can choose the range of their size, lifetime, the speed at which they are fired

    [Header("References")]

    [SerializeField]
    private GameObject smallCube;

    [SerializeField]
    private GameObject parentCube;

    [Header("Cube Properties")]

    [SerializeField]
    private int cubesToSpawn;

    [SerializeField, Min(0)]
    private float minTimer;

    [SerializeField]
    private float maxTimer;

    [SerializeField, Min(0)]
    private float minSize;

    [SerializeField]
    private float maxSize;

    [Header("Fire Properties")]

    [SerializeField, Min(0.01f)]
    private float fireTime;

    [SerializeField, Min(0.01f)]
    private float fireSpeed;

    [SerializeField]
    private KeyCode keyToFireCubes;

    private List<SmallCube> cubesList = new List<SmallCube>();

    private bool isFiring;

    private void Awake()
    {
        // Objects inside the list are instatiated for the first time
        for (int i = 0; i < cubesToSpawn; i++)
        {
            GameObject cube = Instantiate(smallCube);

            cube.transform.SetParent(parentCube.transform, true);
            cube.SetActive(false);

            cubesList.Add(cube.GetComponent<SmallCube>());
        }
    }

    /// <summary>
    /// This function checks if there are inactive objects in the scene
    /// So they can be reused in the pooling system
    /// </summary>
    private SmallCube GetPooledCube()
    {
        foreach (var cube in cubesList)
        {
            if (!cube.gameObject.activeInHierarchy)
            {
                return cube;
            }
        }

        return null;
    }

    /// <summary>
    /// This function checks for a cube that is inactive so it can be spawned
    /// Faster than the fuction above for it is not checking for null
    /// </summary>
    private bool IsCubeAvailable()
    {
        foreach (var cube in cubesList)
        {
            if (!cube.gameObject.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Smaller cubes are spawned here, initialising the values of the prefab
    /// Every single cube will manage its own lifetime and fading colour
    /// </summary>
    private void SpawnCube()
    {
        var availableCube = GetPooledCube();

        if (availableCube == null)
        {
            return;
        }

        // If the pooled cube has not a parent, it will be parented to the big cube
        // This may happen for instance after the firing animation
        if (availableCube.transform.parent == null)
        {
            availableCube.transform.SetParent(parentCube.transform, true);
        }

        // This crates a cube of a random size
        float randomScaleAxis = Random.Range(minSize, maxSize);
        Vector3 randomSize = Vector3.one * randomScaleAxis;

        // This calculates the maximum spawn bounds
        // making sure the cubes will spawn within the parent cube's edges
        Vector3 spawnBounds = (Vector3.one * 0.5f) - (randomSize / 2f);
        spawnBounds = Vector3.Scale(spawnBounds, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));

        // This calculates a random lifetime
        var lifespan = Random.Range(minTimer, maxTimer);

        // Here I initialise all values of the cube
        availableCube.Init(spawnBounds, randomSize, Random.ColorHSV(), lifespan);

        // Then I activate it, because the pooled cube would be inactive
        availableCube.gameObject.SetActive(true);
    }

    /// <summary>
    /// This coroutine will fire all the cubes outward for a certain timer
    /// that can be chosen by the designer
    /// Then the cubes will return to spawn normally
    /// </summary>
    private IEnumerator FireCubes()
    {
        isFiring = true;

        foreach (var cube in cubesList)
        {
            cube.StopRunning();

            cube.transform.SetParent(null, true);
        }

        float currentFireTimer = 0;

        while (currentFireTimer < fireTime)
        {
            foreach (var cube in cubesList)
            {
                Vector3 dir = (cube.transform.position - parentCube.transform.position).normalized;
                cube.transform.position += dir * fireSpeed;
            }

            currentFireTimer += Time.deltaTime;

            yield return null;
        }

        // All the current active cubes are deactivated
        // once currentFireTimer reaches fireTime (specifiable in the editor)
        foreach (var cube in cubesList)
        {
            cube.gameObject.SetActive(false);
        }

        // This check will make it possible to spawn cubes again
        isFiring = false;
    }

    private void Update()
    {
        //This checks that the coroutine is created just once (!!!)
        if (!isFiring && Input.GetKeyDown(keyToFireCubes))
        {
            StartCoroutine(FireCubes());
        }

        // This prevents to the cubes to keep spawning
        // once the coroutine has started
        if (isFiring)
        {
            return;
        }

        // Cubes are spawned as long as there are any available
        if (IsCubeAvailable())
        {
            SpawnCube();
        }
    }
}
