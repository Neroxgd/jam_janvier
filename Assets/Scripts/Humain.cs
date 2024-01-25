using UnityEngine;
using DG.Tweening;

public class Humain : MonoBehaviour
{
    [SerializeField] private GameObject normalLights, redLights;

    // void Start()
    // {
    //     EndGame();
    // }
    
    public void EndGame()
    {
        transform.DOMove(transform.position + new Vector3(2f, 0, -2f), 0.2f);
        normalLights.SetActive(false);
        redLights.SetActive(true);
    }
}
