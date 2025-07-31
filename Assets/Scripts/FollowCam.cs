using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [SerializeField] private float offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = new(Mathf.Clamp(target.transform.position.x, minX, maxX), Mathf.Clamp(target.transform.position.y, minY, maxY), offset);
        }
    }
}
