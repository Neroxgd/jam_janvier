using UnityEngine;
using DG.Tweening;

public class Humain : MonoBehaviour
{
    public void EndGame()
    {
        transform.DOMove(transform.position + new Vector3(1f, 0, -1f), 0.2f);
    }
}
