using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float FireRate = 1f;
    public Transform BulletFirePosition;
    public GameObject BulletPrefab;

    public event Action OnFired;

    private float _current;

    public bool CanFire => _current >= FireRate;

    void Start()
    {
        _current = FireRate;
    }

    public void Fire()
    {
        if (!CanFire) return;

        _current = 0f;

        if (BulletPrefab != null)
        {
            const float spread = 5f;
            for (var i = 0; i < 6; i++)
            {
                var rotationOffset = Quaternion.Euler(
                    UnityEngine.Random.Range(-spread, spread),
                    UnityEngine.Random.Range(-spread, spread),
                    0
                );

                Instantiate(BulletPrefab, BulletFirePosition.position, BulletFirePosition.rotation * rotationOffset);
            }
        }

        OnFired?.Invoke();
    }

    void Update()
    {
        _current += Time.deltaTime;
        if (_current > FireRate)
            _current = FireRate;

        if (Input.GetMouseButton(0) && CanFire) Fire();
    }
}