using UnityEngine;

public class StationaryTarget : MonoBehaviour
{
    public enum TargetState { Down, Up }

    public Transform containerTransform;
    public GameManager gameManager;
    public ParticleSystem sparks;

    public AudioClip hitSound;
    
    private float timeCount = 0.0f;
    private Quaternion rotateStart;
    private Quaternion targetRot;
    private Quaternion UpPosition, DownPosition;
    private TargetState state;

    // Start is called before the first frame update
    void Start()
    {
        UpPosition = containerTransform.rotation;
        DownPosition = Quaternion.Euler(containerTransform.rotation.eulerAngles.x + 90f,
                                        containerTransform.rotation.eulerAngles.y,
                                        containerTransform.rotation.eulerAngles.z);
        state = TargetState.Up;
    }

    // Update is called once per frame
    void Update()
    {
        containerTransform.rotation = Quaternion.Slerp(containerTransform.rotation, targetRot, timeCount);
        timeCount = timeCount + Time.deltaTime;

        if(targetRot == DownPosition)
        {
            state = TargetState.Down;
        }else if(targetRot == UpPosition)
        {
            state = TargetState.Up;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            GetComponent<AudioSource>().PlayOneShot(hitSound);

            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Instantiate(sparks, contact.point, rot);
            if (containerTransform.rotation == Quaternion.Euler(0f, 0f, 0f))
            {
                state = TargetState.Down;
                TargetDown();
                timeCount = 0.0f;
                gameManager.Score++;
                gameManager.TargetsHit++;
            }
        }
    }

    public void TargetDown()
    {
        targetRot = DownPosition;
        state = TargetState.Down;
        timeCount = 0.0f;
    }

    public void TargetUp()
    {
        targetRot = UpPosition;
        state = TargetState.Up;
        timeCount = 0.0f;
    }

    public Quaternion RotateTarget
    {
        get { return targetRot; }
        set { targetRot = value; }
    }

    public TargetState TargetStatus
    {
        get { return state; }
        set { state = value; }
    }
}