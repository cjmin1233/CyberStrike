using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public static float GetRandomNormalDistribution(float mean, float standard)
    {
        var x1 = Random.Range(0f, 1f);
        var x2 = Random.Range(0f, 1f);
        return mean + standard * (Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2));
    }
    public enum GunState
    {
        Ready,
        Empty,
        Reloading
    }
    public GunState gunState { get; private set; }

    private LineRenderer bulletLineRenderer;
    [SerializeField] Transform firePoint;
    public Transform leftGrabPoint;
    public float damage = 20f;
    public float fireDistance = 100f;

    RaycastHit hit;

    public float timeBetFire;
    public float reloadTime;

    [Range(0f, 10f)] public float maxSpread;
    [Range(1f, 10f)] public float stability;
    [Range(0.01f, 3f)] public float restoreFromRecoilSpeed;

    float currentSpread;
    float currenSpreadVelocity;

    float lastFireTime;

    #region Effects
    AudioSource gunAudioPlayer;
    [SerializeField] ParticleSystem muzzleFlashEffect;
    [SerializeField] AudioClip shotClip;
    #endregion
    private void Awake()
    {
        bulletLineRenderer = GetComponent<LineRenderer>();
        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;

        gunAudioPlayer = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        currentSpread = 0f;

        gunState = GunState.Ready;
        lastFireTime = -100f;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void Fire(Vector3 aimPoint)
    {
        if(gunState==GunState.Ready&&
            Time.time >= lastFireTime + timeBetFire)
        {
            var xError = GetRandomNormalDistribution(0f, currentSpread);
            var yError = GetRandomNormalDistribution(0f, currentSpread);

            Vector3 fireDirection = aimPoint - firePoint.position;
            fireDirection = Quaternion.AngleAxis(yError, Vector3.up) * fireDirection;
            fireDirection = Quaternion.AngleAxis(xError, Vector3.right) * fireDirection;

            currentSpread += 1f / stability;

            lastFireTime = Time.time;

            Shot(fireDirection);
        }
    }
    public void Shot(Vector3 fireDirection)
    {
        //var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));

        //if (Physics.Raycast(ray, out hit, fireDistance))
        //{
        //    bulletLineRenderer.positionCount = 2;

        //    bulletLineRenderer.SetPosition(0, firePoint.position);
        //    bulletLineRenderer.SetPosition(1, hit.point);
        //}
        //else bulletLineRenderer.positionCount = 0;
        var hitPosition = Vector3.zero;

        if(Physics.Raycast(firePoint.position, fireDirection, out hit, fireDistance))
        {
            var target = hit.collider.GetComponent<IDamagable>();

            if (target != null)
            {
                DamageMessage damageMessage;

                damageMessage.damager = PlayerControllerFPS.instance.gameObject;
                damageMessage.damage = this.damage;
                damageMessage.hitPoint = hit.point;
                damageMessage.hitNormal = hit.normal;

                target.TakeDamage(damageMessage);
            }
            else
            {
                // 지형물 이펙트
            }
            hitPosition = hit.point;
        }
        else
        {
            hitPosition = firePoint.position + fireDirection * fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));
    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        gunAudioPlayer.PlayOneShot(shotClip);

        bulletLineRenderer.SetPosition(0, firePoint.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;

        //yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(Time.fixedDeltaTime);

        bulletLineRenderer.enabled = false;
    }

    private void Update()
    {
        currentSpread = Mathf.SmoothDamp(currentSpread, 0f, ref currenSpreadVelocity, 1f / restoreFromRecoilSpeed);
        currentSpread = Mathf.Clamp(currentSpread, 0f, maxSpread);
    }
}
