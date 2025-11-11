using TMPro;
using UnityEngine;

public class ObjectRayDetector : MonoBehaviour
{
    [Header("레이캐스트 설정")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask detectLayer;

    [Header("UI 요소")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private IObject currentObject;

    void Update()
    {
        CheckObject();
    }

    void CheckObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, detectLayer))
        {
            IObject detectedObject = hit.collider.GetComponent<IObject>();

            if (detectedObject != null)
            {
                if (currentObject != detectedObject)
                {
                    currentObject = detectedObject;
                    ShowObjectInfo(currentObject);
                }
            }
            else
            {
                CloseObjectInfo();
            }
        }
        else
        {
            CloseObjectInfo();
        }
    }

    void ShowObjectInfo(IObject obj)
    {
        nameText.text = obj.ObjectName;
        descriptionText.text = obj.Description;
        uiPanel.SetActive(true);
    }

    void CloseObjectInfo()
    {
        if (currentObject != null)
        {
            currentObject = null;
            uiPanel.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayDistance);
        }
    }
}
