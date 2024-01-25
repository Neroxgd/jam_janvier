using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    private Vector2 direction;
    private Rigidbody rbPlayer;
    [SerializeField] private float speed = 1f, speedPorteObject = 0.5f, speedGuilli = 0.9f, jumpForce = 1f, gravityForce = 1f, throwForce = 3f, chatouillePower = 1f, chatouilleCalme = 0.1f, stunTimeObject = 1f;
    private bool stun;
    [SerializeField] private uint nombreDebattre = 10;
    private int compteurNbDebattre;
    private float currentSpeed;
    public float StunTimeObj => stunTimeObject;
    private RaycastHit groundHit;
    private Vector3 currentRotation;
    [SerializeField] private LayerMask layerGround;
    [SerializeField] private Image chatouilleBarre;
    private bool notThrow;
    [SerializeField] private GameObject winScreen, grabUI;
    private Humain humain;
    private Animator animator;
    private bool isChatouilling;
    private Transform currentObjectPorted;
    private bool IsGrounded => Physics.BoxCast(transform.position, new Vector3(0.25f, 0.1f, 0.25f), Vector3.down, out groundHit, transform.rotation, 0.75f, layerGround);

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        currentSpeed = speed;
        rbPlayer = GetComponent<Rigidbody>();
        Physics.gravity = Vector3.down * gravityForce;
        humain = FindObjectOfType(typeof(Humain)).GetComponent<Humain>();
    }

    public void Deplacement(InputAction.CallbackContext context)
    {
        if (stun) return;
        direction = context.ReadValue<Vector2>();
        animator.SetBool("course", true);
        if (context.performed)
            currentRotation = new Vector3(direction.x, 0, direction.y);
        else if (context.canceled)
            animator.SetBool("course", false);

    }

    private void Update()
    {
        GrabUI();
        if (isChatouilling)
        {
            RaycastHit hitChatouille;
            if (Physics.SphereCast(transform.position, 0.25f, transform.forward, out hitChatouille, 1f) && hitChatouille.transform.GetComponent<Player>() && hitChatouille.transform != this)
            {
                hitChatouille.transform.GetComponent<Player>().chatouilleBarre.fillAmount += chatouillePower / 10f * Time.deltaTime;
                if (hitChatouille.transform.GetComponent<Player>().chatouilleBarre.fillAmount >= 1f)
                {
                    this.enabled = false;
                    StartCoroutine(WaitEndGame(hitChatouille));
                }
            }
        }
        chatouilleBarre.fillAmount -= chatouilleCalme / 10f * Time.deltaTime;
    }

    private IEnumerator WaitEndGame(RaycastHit hitChatouille)
    {
        humain.EndGame();
        yield return new WaitForSeconds(2f);
        hitChatouille.transform.GetComponent<Player>().enabled = false;
        winScreen.SetActive(true);
    }

    private void FixedUpdate()
    {
        rbPlayer.velocity = new Vector3(direction.x * currentSpeed, rbPlayer.velocity.y, direction.y * currentSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentRotation), 0.2f);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started || !IsGrounded || stun) return;
        rbPlayer.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Porter(InputAction.CallbackContext context)
    {
        if (!context.started || currentObjectPorted != null || stun || currentSpeed == speedGuilli) return;
        notThrow = true;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.25f, transform.forward, out hit, 1f))
        {
            if (hit.transform.GetComponent<ObjectPortable>())
                StartCoroutine(WaitPorter(hit, false));
            else if (hit.transform.GetComponent<Player>() && hit.transform != this)
                StartCoroutine(WaitPorter(hit, true));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(transform.position + transform.forward * 0.75f, 0.1f);
    }

    public void Jeter(InputAction.CallbackContext context)
    {
        if (!context.started || currentObjectPorted == null || notThrow || stun) return;
        currentSpeed = speed;
        Rigidbody rbCurrentObjectPorted = currentObjectPorted.GetComponent<Rigidbody>();
        StartCoroutine(Stun(currentObjectPorted.GetComponent<Player>(), 1f, false));
        rbCurrentObjectPorted.isKinematic = false;
        currentObjectPorted.transform.parent = null;
        rbCurrentObjectPorted.AddForce(new Vector3(transform.forward.x, transform.forward.y, transform.forward.z) * throwForce * rbCurrentObjectPorted.mass, ForceMode.Impulse);
        if (currentObjectPorted.GetComponent<Player>())
            currentObjectPorted.GetComponent<Player>().animator.SetBool("idle", true);
        currentObjectPorted = null;
        animator.SetTrigger("porter");
    }

    public void Chatouiller(InputAction.CallbackContext context)
    {
        if (currentSpeed == speedPorteObject) return;
        if (context.started)
        {
            currentSpeed = speedGuilli;
            isChatouilling = true;
            animator.SetBool("guilli", true);
        }
        else if (context.canceled)
        {
            currentSpeed = speed;
            isChatouilling = false;
            animator.SetBool("guilli", false);
        }
    }

    private void SeFairePorter(Player player)
    {
        transform.DOMove(player.transform.position + Vector3.up * 1.6f, 0.1f)
        .OnComplete(() => transform.position = player.transform.position + Vector3.up * 1.6f);
        rbPlayer.isKinematic = true;
        compteurNbDebattre = (int)nombreDebattre;
        transform.parent = player.transform;
        animator.SetBool("idle", false);
        animator.SetBool("stunPorter", true);
    }

    private IEnumerator WaitPorter(RaycastHit hit, bool ifPlayer)
    {
        StartCoroutine(Stun(this, 0.3f, false));
        notThrow = false;
        if (ifPlayer)
            hit.transform.GetComponent<Player>().SeFairePorter(this);
        else
            hit.transform.GetComponent<ObjectPortable>().Porter(this);
        animator.SetTrigger("porter");
        yield return 0;
        currentObjectPorted = hit.transform;
        currentSpeed = speedPorteObject;
    }

    public IEnumerator Stun(Player player, float timeStun, bool isStun)
    {
        if (player == null) yield break;
        if (currentSpeed == speedPorteObject)
        {
            currentObjectPorted.transform.parent = null;
            currentObjectPorted.GetComponent<Rigidbody>().isKinematic = false;
            currentObjectPorted = null;
            currentSpeed = speed;
        }
        if (isStun)
        {
            stun = true;
            animator.SetTrigger("stun");
            animator.SetBool("course", false);
        }
        player.enabled = false;
        yield return new WaitForSeconds(timeStun);
        if (isStun)
            stun = false;
        player.enabled = true;
    }

    private void GrabUI()
    {
        RaycastHit hit;
        if (currentSpeed != speedPorteObject && Physics.SphereCast(transform.position, 0.25f, transform.forward, out hit, 1f) && hit.transform.GetComponent<ObjectPortable>())
        {
            grabUI.SetActive(true);
            grabUI.GetComponentInChildren<TextMeshProUGUI>().text = "Grab";
            return;
        }
        if (currentSpeed == speedPorteObject)
        {
            grabUI.SetActive(true);
            grabUI.GetComponentInChildren<TextMeshProUGUI>().text = "Throw";
            return;
        }
        grabUI.SetActive(false);
    }

    public void SeDebattre(InputAction.CallbackContext context)
    {
        if (!context.started || !rbPlayer.isKinematic) return;
        compteurNbDebattre--;
        transform.DOShakePosition(0.2f, 0.2f, 50, 180f, false, false, ShakeRandomnessMode.Harmonic).SetId("shake");
        if (compteurNbDebattre <= 0)
        {
            DOTween.Kill("shake");
            transform.parent = null;
            rbPlayer.isKinematic = false;
            animator.SetBool("idle", true);
            animator.SetBool("stunPorter", false);
        }
    }
}
