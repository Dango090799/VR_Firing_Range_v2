using UnityEngine;

public class Advanced_Recoil : MonoBehaviour
{

    [Header("Advanced Recoil")]
    public bool Advanced;
    [Space(10)]
    [Header("Advanced Recoil Settings")]
    public float PositionDampTime;
    public float RotationDampTime;
    [Space(10)]
    public float Recoil1;
    public float Recoil2;
    public float Recoil3;
    public float Recoil4;
    [Space(10)]
    public Vector3 RecoilRotation;
    public Vector3 RecoilKickBack;
    [Space(10)]
    [Header("Basic Recoil Settings")]
    public float RecoilYRotation;

    private Transform RecoilTransform;
    [Space(10)]
    private Vector3 CurrentRecoil1;
    private Vector3 CurrentRecoil2;
    private Vector3 CurrentRecoil3;
    private Vector3 CurrentRecoil4;
    [Space(10)]
    private Vector3 RotationOutput;
    private Transform currentHand, secondaryHand;
    private string currentPrimaryHand;

    public Transform leftShoulder, rightShoulder;
    public Transform headPosition;

    void FixedUpdate()
    {
        CurrentRecoil1 = Vector3.Lerp(CurrentRecoil1, Vector3.zero, Recoil1 * Time.deltaTime);
        CurrentRecoil2 = Vector3.Lerp(CurrentRecoil2, CurrentRecoil1, Recoil2 * Time.deltaTime);
        CurrentRecoil3 = Vector3.Lerp(CurrentRecoil3, Vector3.zero, Recoil3 * Time.deltaTime);
        CurrentRecoil4 = Vector3.Lerp(CurrentRecoil4, CurrentRecoil3, Recoil4 * Time.deltaTime);

        if(RecoilTransform != null && currentHand != null)
        {
            RecoilTransform.localPosition = Vector3.Slerp(RecoilTransform.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
            RotationOutput = Vector3.Slerp(RotationOutput, CurrentRecoil1, RotationDampTime * Time.fixedDeltaTime);
            RecoilTransform.localRotation = Quaternion.Euler(RotationOutput);

            currentHand.localPosition = Vector3.Slerp(currentHand.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
            currentHand.localRotation = Quaternion.Euler(RotationOutput);

            if(secondaryHand != null)
            {
                secondaryHand.localPosition = Vector3.Slerp(secondaryHand.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
                secondaryHand.localRotation = Quaternion.Euler(RotationOutput);
            }

            // Adds recoil to shoulder
            if(currentPrimaryHand == "DistanceGrabHandLeft")
            {
                leftShoulder.localPosition = Vector3.Slerp(leftShoulder.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
                leftShoulder.localRotation = Quaternion.Euler(RotationOutput);
            }else if(currentPrimaryHand == "DistanceGrabHandRight")
            {
                rightShoulder.localPosition = Vector3.Slerp(rightShoulder.localPosition, CurrentRecoil3, PositionDampTime * Time.fixedDeltaTime);
                rightShoulder.localRotation = Quaternion.Euler(RotationOutput);
            }
        }
    }
    public void Fire(GameObject grabber)
    {
        secondaryHand = null;
        // Determine transforms for hand gun is held in
        if (grabber.name == "DistanceGrabHandLeft")
        {
            currentPrimaryHand = grabber.name;
            currentHand = grabber.transform.Find("gripTrans");
            RecoilTransform = grabber.transform.Find("l_hand_skeletal_lowres");
        }
        else if (grabber.name == "DistanceGrabHandRight")
        {
            currentPrimaryHand = grabber.name;
            currentHand = grabber.transform.Find("gripTrans");
            RecoilTransform = grabber.transform.Find("r_hand_skeletal_lowres");
        }

        // Determine type of recoil to be applied
        if (Advanced)
        {
            CurrentRecoil1 += new Vector3(RecoilRotation.x, Random.Range(0, RecoilRotation.y), Random.Range(-RecoilRotation.z, RecoilRotation.z));
            CurrentRecoil3 += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
        }else
        {
            CurrentRecoil1 += new Vector3(0, Random.Range(0, RecoilYRotation), 0);
        }
    }

    public void Fire(GameObject leftHand, GameObject rightHand)
    {
        // need to determine which hand is the primary hand
        secondaryHand = rightHand.transform.Find("gripTrans");
    }
}