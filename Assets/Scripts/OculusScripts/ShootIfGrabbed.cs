using OculusSampleFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShootIfGrabbed : MonoBehaviour
{
    //public int maxBullets = 10;
    public Text bulletText;
    public GameObject player;
    public GameManager gameManager;

    [Space(10)]
    [Header("Input Buttons")]
    public OVRInput.Button shootingButton;
    public OVRInput.Button reloadButton;

    [Space(10)]
    [Header("Sounds")]
    public AudioClip pistolShot;
    public AudioClip emptyMagSound;
    public AudioClip[] reloadSounds;
    public AudioClip[] dropSounds;

    private SimpleShoot simpleShoot;
    private OVRGrabbable ovrGrabbable;
    private GameObject currentMag;
    private int currentBullets;
    private OVRInput.Controller currentHand;


    // Start is called before the first frame update
    void Start()
    {
        simpleShoot = GetComponent<SimpleShoot>();
        ovrGrabbable = GetComponent<OVRGrabbable>();

        currentMag = null;
        currentBullets = 0;
        bulletText.text = currentBullets.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (ovrGrabbable.isGrabbed)
        {
            currentHand = ovrGrabbable.grabbedBy.GetController();
            gameManager.CurrentHand = currentHand;
        }

        // Constantly ignore collisions with current magazine collider
        if (currentMag != null)
        {
            currentMag.GetComponent<Collider>().enabled = false;
            currentMag.transform.Find("Crosshair").gameObject.SetActive(false);
        }

        // Shooting the pistol
        if (ovrGrabbable.isGrabbed && OVRInput.GetDown(shootingButton, ovrGrabbable.grabbedBy.GetController()) && currentBullets > 0)
        {
            VibrationManager.vibManager.TriggerVibration(40, 2, 255, ovrGrabbable.grabbedBy.GetController());
            GetComponent<AudioSource>().PlayOneShot(pistolShot);
            simpleShoot.TriggerShoot();

            currentBullets--;

            if(currentMag != null)
            {
                currentMag.GetComponent<Magazine>().Ammo = currentBullets;
            }
        }
        if (ovrGrabbable.isGrabbed && OVRInput.GetDown(shootingButton, ovrGrabbable.grabbedBy.GetController()) && currentBullets <= 0)
        {
            GetComponent<AudioSource>().PlayOneShot(emptyMagSound);
        }

        if (ovrGrabbable.isGrabbed && OVRInput.GetDown(reloadButton, ovrGrabbable.grabbedBy.GetController())) // Check if gun is held and holding hand has reload button pressed
        {
            if (currentMag != null)
            {
                if (currentMag.GetComponent<Magazine>().Ammo > 0)
                {
                    currentMag.GetComponent<Magazine>().Ammo = currentBullets - 1;
                }
                if (currentBullets > 0)
                {
                    currentBullets = 1; // 1 left in the chamber when the magazine is dropped
                }

                currentMag.GetComponent<Rigidbody>().isKinematic = false;
                currentMag.transform.parent = null;
                currentMag.transform.Find("Crosshair").gameObject.SetActive(true);

                StartCoroutine(WaitTimeForMagazine(0.2f, currentMag.GetComponent<Collider>()));
            }
        }

        bulletText.text = currentBullets.ToString();
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (!collision.gameObject.CompareTag("PistolMagazine") && !collision.gameObject.CompareTag("Bullet"))
        //{
        //    int index = Random.Range(0, dropSounds.Length);
        //    GetComponent<AudioSource>().PlayOneShot(dropSounds[index]);
        //}
    }

    IEnumerator WaitTimeForMagazine(float time, Collider collider)
    {
        yield return new WaitForSeconds(time);
        currentMag.GetComponent<Collider>().enabled = true;
        currentMag = null;
    }

    public OVRGrabbable Grabber
    {
        get { return ovrGrabbable; }
    }

    public OVRInput.Controller CurrentController
    {
        get { return currentHand; }
    }

    public int Bullets
    {
        get { return currentBullets; }
        set { currentBullets = value; }
    }

    public GameObject CurrentMag
    {
        get { return currentMag; }
        set { currentMag = value; }
    }
}
