using UnityEngine;

public class KeepSpriteUpright : MonoBehaviour
{
    private Quaternion initRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initRotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = initRotation;
    }
}
