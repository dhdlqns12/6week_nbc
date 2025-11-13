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
        try
        {
            if (nameText == null || descriptionText == null || uiPanel == null)
            {
                throw new System.NullReferenceException("UI 요소가 할당되지 않았습니다!");
            }

            if (string.IsNullOrEmpty(obj.ObjectName))
            {
                throw new System.ArgumentException("오브젝트 이름이 비어있습니다!");
            }

            nameText.text = obj.ObjectName;
            descriptionText.text = obj.Description ?? "설명 없음";
            uiPanel.SetActive(true);
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError($"UI 초기화 오류: {e.Message}");
            enabled = false;
        }
        catch (System.ArgumentException e)
        {
            Debug.LogWarning($"오브젝트 데이터 오류: {e.Message}");
        }
    }

    void CloseObjectInfo()
    {
        if (currentObject != null)
        {
            currentObject = null;
            uiPanel.SetActive(false);
        }
    }

    #region 디버그용
    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayDistance);
        }
    }
    #endregion
}
