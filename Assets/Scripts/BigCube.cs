using UnityEngine;

public class BigCube : MonoBehaviour
{
    [SerializeField]
    private GameObject bigCube;

    [SerializeField]
    private Vector3 rotationAxis;

    private void Start()
    {
        bigCube.SetActive(true);
    }

    private void Rotation()
    {
        bigCube.transform.Rotate(rotationAxis * Time.deltaTime, Space.World);
    }

    private void Update()
    {
        Rotation();
    }
}
