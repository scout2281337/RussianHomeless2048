using UnityEngine;

public class DangerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController contr = other.GetComponent<PlayerController>();
        if (contr != null)
        {
            contr.SetFightMode(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController contr = other.GetComponent<PlayerController>();
        if (contr != null)
        {
            contr.SetFightMode(false);
        }
    }
}
