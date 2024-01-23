using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 direction;
    private Rigidbody rbPlayer;
    [SerializeField] private float speed = 1f, jumpForce = 1f, gravityForce = 1f;
    private float gravity;
    private RaycastHit groundHit;
    private Vector3 currentRotation;
    [SerializeField] private LayerMask layerGround;
    private bool IsGrounded => Physics.BoxCast(transform.position, new Vector3(0.5f, 0.1f, 0.5f), Vector3.down, out groundHit, transform.rotation, 0.5f, layerGround);




    private void Start()
    {
        rbPlayer = GetComponent<Rigidbody>();
    }

    public void Deplacement(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
        if (context.performed)
            currentRotation = new Vector3(direction.x, 0, direction.y);
    }

    private void FixedUpdate()
    {
        if (IsGrounded)
            gravity = 0f;
        else
            gravity += gravityForce;
        rbPlayer.velocity = new Vector3(direction.x * speed, rbPlayer.velocity.y - gravity, direction.y * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentRotation), 0.1f);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started || !IsGrounded) return;
        rbPlayer.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Porter(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.4f, transform.forward, out hit, 1f, layerGround))
        {
            if (hit.transform.GetComponent<ObjectPortable>())
                hit.transform.GetComponent<ObjectPortable>().Porter(this, true);
            if (hit.transform.GetComponent<Player>())
                SeFairePorter();
        }
    }

    private void SeFairePorter()
    {

    }
}
