using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Vector3 blackCamPosition;
    [SerializeField] private Vector3 blackCamRotation;

    private Vector3 whiteCamPosition;
    private Vector3 whiteCamRotation;

    private bool isTurned;

    private void Awake()
    {
        whiteCamPosition = Camera.main.transform.position;
        whiteCamRotation = Camera.main.transform.localRotation.eulerAngles;
    }

    public void TurnBoard() 
    {
        var pos = (isTurned) ? whiteCamPosition : blackCamPosition;
        var rot = (isTurned) ? whiteCamRotation : blackCamRotation;

        Camera.main.transform.position = pos;
        Camera.main.transform.rotation = Quaternion.Euler(rot);

        isTurned = !isTurned;
    }
}
