using UnityEngine;

public class ObjectPortable : MonoBehaviour
{
    private bool porter;
    private Player _player;

    public void Porter(Player player, bool isPorter)
    {
        if (isPorter)
        {
            porter = true;
            _player = player;
        }
        else
            porter = false;
    }

    private void Update()
    {
        if (porter)
            transform.position = _player.transform.position + Vector3.up;
    }
}
