using UnityEngine;
using DG.Tweening;

public class Humain : MonoBehaviour
{
    void Start()
    {
        EndGame();
    }

    public void EndGame()
    {
        transform.DOMove(transform.position + new Vector3(3f, 0, -1f), 0.2f);
    }
}
