using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandsIK : MonoBehaviour
{
    [Header("IK Targets")]
    [SerializeField] Transform HandTarget; // ���� ��� ����� ����
    [SerializeField] Rig HandRig;
    [SerializeField] Transform cameraTransform;

    [Header("IK Settings")]
    [Range(0, 1)] public float handIKWeight = 1f; // ���� ������� IK �� ����

    void FixedUpdate()
    {
        HandTarget.position = cameraTransform.position + cameraTransform.forward - cameraTransform.up;
    }

    public void SetHandIK(bool i)
    {
        HandRig.weight = i ? handIKWeight : 0;
    }
}
