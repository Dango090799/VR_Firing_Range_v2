using UnityEngine;
using UnityEngine.UI;

public class Magazine : MonoBehaviour
{
    public int AmmoCapacity;
    public Text ammoText;
    public AudioClip[] dropSounds;
    public AudioClip[] reloadSounds;

    private int CurrentAmmo;

    // Start is called before the first frame update
    void Start()
    {
        CurrentAmmo = AmmoCapacity;
        ammoText.text = CurrentAmmo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        ammoText.text = CurrentAmmo.ToString();
    }

    public int Ammo
    {
        get { return CurrentAmmo; }
        set { CurrentAmmo = value; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject gun = collision.gameObject;
        if (gun.CompareTag("Pistol") && collision.collider.name == "Mag_Collider")
        {
            transform.Find("Crosshair").gameObject.SetActive(false);

            int index = Random.Range(0, reloadSounds.Length);
            GetComponent<AudioSource>().PlayOneShot(reloadSounds[index]);

            if (GetComponent<Magazine>().Ammo > 0)
            {
                gun.GetComponent<ShootIfGrabbed>().Bullets = gun.GetComponent<ShootIfGrabbed>().Bullets + GetComponent<Magazine>().Ammo;
            }
            gun.GetComponent<ShootIfGrabbed>().bulletText.text = gun.GetComponent<ShootIfGrabbed>().Bullets.ToString(); // updates the bullet text;

            GetComponent<OVRGrabbable>().GrabEnd(Vector3.zero, Vector3.zero);
            GetComponent<Rigidbody>().isKinematic = true;

            transform.parent = gun.transform;

            transform.position = transform.parent.position + new Vector3(0,0,0);
            transform.rotation = transform.parent.rotation;
            transform.Rotate(new Vector3(0, 90, 0)); // add on a bit of rotation

            gun.GetComponent<ShootIfGrabbed>().CurrentMag = gameObject;
        }
    }
}
