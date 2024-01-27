using UnityEngine;

public class TpUnderMap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(4.3f, 1.7f, -3.5f);
    }
}
