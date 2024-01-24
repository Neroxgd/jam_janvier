using UnityEngine;
using DG.Tweening;

public class ObjectPortable : MonoBehaviour
{
    private Player _player;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Porter(Player player)
    {
        _player = player;
        transform.DOMove(_player.transform.position + Vector3.up * 1.6f, 0.1f)
        .OnComplete(() => transform.position = _player.transform.position + Vector3.up * 1.6f);
        rb.isKinematic = true;
        transform.parent = player.transform;
    }
}
