using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage;

    public void SetHealthPercent(float percent)
    {
        fillImage.fillAmount = Mathf.Clamp01(percent);
    }
}
