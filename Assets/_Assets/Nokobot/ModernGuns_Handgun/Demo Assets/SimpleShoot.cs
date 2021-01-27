using UnityEngine;

public class SimpleShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public Transform barrelLocation;
    public Transform casingExitLocation;
    public Advanced_Recoil recoil;
    public float shotPower;
    public float InitialBulletVel { get; set; }

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;
    }

    public void TriggerShoot()
    {
        GetComponent<Animator>().SetTrigger("Fire");
    }

    void Shoot()
    {
        GameObject tempFlash;
        InitialBulletVel = Vector3.Magnitude(barrelLocation.forward * shotPower);
        Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
        tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

        recoil.Fire(GetComponent<ShootIfGrabbed>().Grabber.grabbedBy.gameObject); // need to change this to check if one or two hands is holding the gun
    }

    void CasingRelease()
    {
        GameObject casing;
        casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        casing.GetComponent<Rigidbody>().AddExplosionForce(550f, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        casing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(10f, 1000f)), ForceMode.Impulse);
    }
}
