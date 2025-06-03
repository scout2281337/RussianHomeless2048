using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandsIK : MonoBehaviour
{
    [Header("IK Targets")]
    [SerializeField] Transform HandTarget; // Цель для левой руки
    [SerializeField] Rig HandRig; 

    [Header("IK Settings")]
    [Range(0, 1)] public float handIKWeight = 1f; // Сила влияния IK на руки


    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        HandTarget.position = cameraTransform.position + cameraTransform.forward - cameraTransform.up;
    }

    public void SetHandIK(bool i)
    {
        HandRig.weight = i ? handIKWeight : 0;
    }
}
