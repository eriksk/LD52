using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float FireRate = 1f;
    public Transform BulletFirePosition;
    public GameObject BulletPrefab;

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
            var bullet = Instantiate(BulletPrefab, BulletFirePosition.position, BulletFirePosition.rotation);
        }
    }

    void Update()
    {
        _current += Time.deltaTime;
        if (_current > FireRate)
            _current = FireRate;

        if (Input.GetMouseButton(0) && CanFire) Fire();
    }
}