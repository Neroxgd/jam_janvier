using UnityEngine;
using UnityEngine.Rendering;

public class StunObject : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float minimumSpeedToTrigger = 15f;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() && rb.velocity.magnitude >= minimumSpeedToTrigger)
            StartCoroutine(other.GetComponent<Player>().Stun(other.GetComponent<Player>()));
    }

    // void Update()
    // {
    //     print(rb.velocity.magnitude);
    // }
}
