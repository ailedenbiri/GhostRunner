using UnityEngine;
using System;
using System.Collections;   

[RequireComponent(typeof(PlayerMovement))]
public class PlayerScaleMorph : MonoBehaviour
{
    [Header("Scale Ayarları")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 minAirScale = new Vector3(0.6f, 0.6f, 0.6f);

    [Tooltip("Havaya çıktığında ne kadar hızlı küçülsün")]
    public float shrinkSpeed = 5f;

    [Tooltip("Zemine değince ne kadar hızlı eski haline dönsün")]
    public float restoreSpeed = 10f;

    private PlayerMovement _movement;
    private Coroutine _scaleRoutine;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        normalScale = transform.localScale;
    }

    private void OnEnable()
    {
        _movement.JumpStarted += OnJumpStarted;
        _movement.Landed += OnLanded;
    }

    private void OnDisable()
    {
        _movement.JumpStarted -= OnJumpStarted;
        _movement.Landed -= OnLanded;
    }

    private void OnJumpStarted()
    {
        StartScaleRoutine(minAirScale, shrinkSpeed);
    }

    private void OnLanded()
    {
        StartScaleRoutine(normalScale, restoreSpeed);
    }

    private void StartScaleRoutine(Vector3 target, float speed)
    {
        if (_scaleRoutine != null)
            StopCoroutine(_scaleRoutine);

        _scaleRoutine = StartCoroutine(ScaleRoutine(target, speed));
    }

    private IEnumerator ScaleRoutine(Vector3 target, float speed)
    {
        while ((transform.localScale - target).sqrMagnitude > 0.0001f)
        {
            transform.localScale =
                Vector3.Lerp(transform.localScale, target, Time.deltaTime * speed);
            yield return null;
        }

        transform.localScale = target;
    }
}

