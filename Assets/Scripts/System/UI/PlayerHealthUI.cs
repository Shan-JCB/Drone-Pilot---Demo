using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Small HUD helper to show player's health as text and optional fill image.
/// Attach to a Canvas GameObject. Assign a TextMeshProUGUI and, optionally, an Image for a bar.
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image healthFillImage; // optional - expects Image type with Fill Method set to Horizontal

    public void UpdateHealth(float current, float max)
    {
        if (healthText != null)
        {
            healthText.SetText($"HP: {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}");
        }

        if (healthFillImage != null && max > 0f)
        {
            healthFillImage.fillAmount = Mathf.Clamp01(current / max);
        }
    }
}
