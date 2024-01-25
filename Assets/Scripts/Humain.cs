using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using Aurinaxtailer;

public class Humain : MonoBehaviour
{
    [SerializeField] private GameObject normalLights, redLights, chat, chien;
    [SerializeField] private TextMeshProUGUI compteurStart;
    [SerializeField] private AudioClip musiqueDecompte, inGameLoop;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(musiqueDecompte);
        StartCoroutine(CompteurStart());
        // EndGame();
    }

    public void EndGame()
    {
        transform.DOMove(transform.position + new Vector3(2f, 0, -2f), 0.2f);
        normalLights.SetActive(false);
        redLights.SetActive(true);
    }

    private IEnumerator CompteurStart()
    {
        compteurStart.text = "";
        yield return new WaitForSeconds(0.2f);
        compteurStart.text = "3";
        yield return new WaitForSeconds(1.1f);
        compteurStart.text = "2";
        yield return new WaitForSeconds(1.1f);
        compteurStart.text = "1";
        yield return new WaitForSeconds(1.3f);
        compteurStart.text = "FIGHT !";
        yield return new WaitForSeconds(0.9f);
        AudioManager.Instance.PlayMusic(inGameLoop);
        chat.GetComponent<PlayerInput>().enabled = true;
        chien.GetComponent<PlayerInput>().enabled = true;
        compteurStart.gameObject.SetActive(false);
    }
}
