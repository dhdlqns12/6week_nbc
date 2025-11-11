using UnityEngine;
using UnityEngine.UI;

public class SpBar : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Slider spSlider;
    [SerializeField] private Image fillImage;

    #region 유니티  callback함수
    private void OnEnable()
    {
        EventBus.OnSpChanged += UpdateSpBar;
    }

    private void OnDisable()
    {
        EventBus.OnSpChanged -= UpdateSpBar;
    }

    private void Start()
    {
        UpdateSpBar();
    }
    #endregion

    private void UpdateSpBar()
    {
        float normalizedSp = player.curSp / 100;
        spSlider.value = normalizedSp;
    }
}
