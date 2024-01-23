using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Player : MonoBehaviour
{
    private Vector2 direction;
    private Rigidbody rbPlayer;
    [SerializeField] private float speed = 1f, jumpForce = 1f, gravityForce = 1f, throwForce = 3f;
    private bool porter;
    private RaycastHit groundHit;
    private Vector3 currentRotation;
    [SerializeField] private LayerMask layerGround;
    private bool notThrow;
    private Transform currentObjectPorted;
    private bool IsGrounded => Physics.BoxCast(transform.position, new Vector3(0.5f, 0.1f, 0.5f), Vector3.down, out groundHit, transform.rotation, 0.5f, layerGround);

    private void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();
        Physics.gravity = Vector3.down * gravityForce;
    }

    public void Deplacement(InputAction.CallbackContext context)
    {
        if (porter) return;
        direction = context.ReadValue<Vector2>();
        if (context.performed)
            currentRotation = new Vector3(direction.x, 0, direction.y);
    }

    private void FixedUpdate()
    {
        rbPlayer.velocity = new Vector3(direction.x * speed, rbPlayer.velocity.y, direction.y * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentRotation), 0.2f);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started || !IsGrounded || porter) return;
        rbPlayer.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Porter(InputAction.CallbackContext context)
    {
        if (!context.started || currentObjectPorted != null || porter) return;
        notThrow = true;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.4f, transform.forward, out hit, 1f))
        {
            if (hit.transform.GetComponent<ObjectPortable>())
            {
                StartCoroutine(Wait1Frame());
                currentObjectPorted = hit.transform;
                hit.transform.GetComponent<ObjectPortable>().Porter(this);
            }
            if (hit.transform.GetComponent<Player>() && hit.transform != this)
            {
                StartCoroutine(Wait1Frame());
                currentObjectPorted = hit.transform;
                currentObjectPorted.transform.parent = transform;
                currentObjectPorted.GetComponent<Player>().SeFairePorter(this);
            }
        }
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.DrawSphere(transform.position + transform.forward / 2f, 0.4f);
    //     Gizmos.DrawSphere(transform.position + transform.forward * 1.5f, 0.4f);
    // }

    public void Jeter(InputAction.CallbackContext context)
    {
        if (!context.started || currentObjectPorted == null || notThrow || porter) return;
        Rigidbody rbCurrentObjectPorted = currentObjectPorted.GetComponent<Rigidbody>();
        StartCoroutine(Stun(currentObjectPorted.GetComponent<Player>()));
        rbCurrentObjectPorted.isKinematic = false;
        currentObjectPorted.transform.parent = null;
        rbCurrentObjectPorted.AddForce(new Vector3(transform.forward.x, transform.forward.y + 2f, transform.forward.z) * throwForce * rbCurrentObjectPorted.mass, ForceMode.Impulse);
        currentObjectPorted = null;
    }

    private void SeFairePorter(Player player)
    {
        transform.DOMove(player.transform.position + Vector3.up * 1.1f, 0.1f);
        rbPlayer.isKinematic = true;
    }

    private IEnumerator Wait1Frame()
    {
        yield return 0;
        notThrow = false;
    }

    private IEnumerator Stun(Player player)
    {
        if (player == null) yield break;
        player.enabled = false;
        yield return new WaitForSeconds(1f);
        player.enabled = true;
    }
}

