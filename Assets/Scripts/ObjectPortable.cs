using UnityEngine;

public class ObjectPortable : MonoBehaviour
{
    private bool porter;
    private Player _player;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Porter(Player player, bool isPorter)
    {
        if (isPorter)
        {
            porter = true;
            _player = player;
            rb.isKinematic = true;
        }
        else
            porter = false;
    }

    private void Update()
    {
        if (porter)
        {
            transform.position = _player.transform.position + Vector3.up * 1.1f;
            transform.rotation = _player.transform.rotation;
        }
    }
}
