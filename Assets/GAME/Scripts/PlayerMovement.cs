using UnityEngine;
using System;
using System.Collections;   


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
      [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 7f;

    [Tooltip("Zemin olarak kabul edeceğin layer'lar")]
    public LayerMask groundLayer;

    // Diğer scriptler için event'ler
    public event Action JumpStarted;
    public event Action Landed;

    private Rigidbody _rb;
    private bool _isGrounded;

    public bool IsGrounded => _isGrounded;   // istersen başka yerden de okuyabilirsin

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        // 2.5D için Z ve rotasyon kilidi
        _rb.constraints = RigidbodyConstraints.FreezePositionZ |
                          RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        // Eski Input sistemi
        float inputX = Input.GetAxisRaw("Horizontal");

        Vector3 vel = _rb.linearVelocity;
        vel.x = inputX * moveSpeed;
        _rb.linearVelocity = vel;
    }

    private void HandleJump()
    {
        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            // Daha temiz zıplama
            Vector3 v = _rb.linearVelocity;
            v.y = 0;
            _rb.linearVelocity = v;

            _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            _isGrounded = false;          // havaya çıktık

            JumpStarted?.Invoke();
        }
    }

    // --- GROUND ALGILAMA BURADA ---

    private void OnCollisionEnter(Collision other)
    {
        if (IsInGroundLayer(other.gameObject.layer))
        {
            bool wasGrounded = _isGrounded;
            _isGrounded = true;

            // sadece havadayken yere döndüğümüzde event yolla
            if (!wasGrounded)
            {
                // Debug.Log("LANDED");
                Landed?.Invoke();
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (IsInGroundLayer(other.gameObject.layer))
        {
            _isGrounded = false;
        }
    }

    private bool IsInGroundLayer(int layer)
    {
        // verilen LayerMask içinde mi kontrol ediyoruz
        return (groundLayer.value & (1 << layer)) != 0;
    }
}