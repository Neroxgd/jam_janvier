using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    private Vector2 direction;
    private Rigidbody rbPlayer;
    [SerializeField] private float speed = 1f, speedPorteObject = 0.5f, speedGuilli = 0.6f, jumpForce = 1f, gravityForce = 1f, throwForce = 3f, chatouillePower = 1f, chatouilleCalme = 0.1f, stunTimeObject = 1f;
    private bool porter;
    private float currentSpeed;
    public float StunTimeObj => stunTimeObject;
    private RaycastHit groundHit;
    private Vector3 currentRotation;
    [SerializeField] private LayerMask layerGround;
    [SerializeField] private Image chatouilleBarre; public Image ChatouilleBarre { get { return chatouilleBarre; } set { chatouilleBarre = value; } }
    private bool notThrow;
    [SerializeField] private GameObject winScreen, grabUI;
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
    }

    public void Deplacement(InputAction.CallbackContext context)
    {
        if (porter) return;
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
                hitChatouille.transform.GetComponent<Player>().ChatouilleBarre.fillAmount += chatouillePower / 10f * Time.deltaTime;
                if (hitChatouille.transform.GetComponent<Player>().ChatouilleBarre.fillAmount >= 1f)
                {
                    hitChatouille.transform.GetComponent<Player>().enabled = false;
                    winScreen.SetActive(true);
                    this.enabled = false;
                }
            }
        }
        chatouilleBarre.fillAmount -= chatouilleCalme / 10f * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        rbPlayer.velocity = new Vector3(direction.x * currentSpeed, rbPlayer.velocity.y, direction.y * currentSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentRotation), 0.2f);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started || !IsGrounded || porter) return;
        rbPlayer.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Porter(InputAction.CallbackContext context)
    {
        if (!context.started || currentObjectPorted != null || porter || currentSpeed == speedGuilli) return;
        notThrow = true;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.25f, transform.forward, out hit, 1f))
        {
            if (hit.transform.GetComponent<ObjectPortable>())
            {
                StartCoroutine(WaitPorter(hit));
            }
            // if (hit.transform.GetComponent<Player>() && hit.transform != this)
            // {
            // currentSpeed = speedPorteObject;
            //     StartCoroutine(Wait1Frame());
            //     currentObjectPorted = hit.transform;
            //     currentObjectPorted.transform.parent = transform;
            //     currentObjectPorted.GetComponent<Player>().SeFairePorter(this);
            // }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(transform.position + transform.forward * 0.75f, 0.1f);
    }

    public void Jeter(InputAction.CallbackContext context)
    {
        if (!context.started || currentObjectPorted == null || notThrow || porter) return;
        currentSpeed = speed;
        Rigidbody rbCurrentObjectPorted = currentObjectPorted.GetComponent<Rigidbody>();
        StartCoroutine(Stun(currentObjectPorted.GetComponent<Player>(), 1f));
        rbCurrentObjectPorted.isKinematic = false;
        currentObjectPorted.transform.parent = null;
        rbCurrentObjectPorted.AddForce(new Vector3(transform.forward.x, transform.forward.y, transform.forward.z) * throwForce * rbCurrentObjectPorted.mass, ForceMode.Impulse);
        currentObjectPorted = null;
    }

    public void Chatouiller(InputAction.CallbackContext context)
    {
        if (currentSpeed == speedPorteObject) return;
        if (context.started)
        {
            currentSpeed = speedGuilli;
            isChatouilling = true;
        }
        else if (context.canceled)
        {
            currentSpeed = speed;
            isChatouilling = false;
        }
    }

    private void SeFairePorter(Player player)
    {
        transform.DOMove(player.transform.position + Vector3.up * 1.6f, 0.1f);
        rbPlayer.isKinematic = true;
    }

    private IEnumerator WaitPorter(RaycastHit hit)
    {
        StartCoroutine(Stun(this, 0.5f));
        animator.SetTrigger("lift");
        yield return new WaitForSeconds(0.5f);
        notThrow = false;
        currentSpeed = speedPorteObject;
        currentObjectPorted = hit.transform;
        hit.transform.GetComponent<ObjectPortable>().Porter(this);
    }

    public IEnumerator Stun(Player player, float timeStun)
    {
        if (player == null) yield break;
        player.enabled = false;
        yield return new WaitForSeconds(timeStun);
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
}
