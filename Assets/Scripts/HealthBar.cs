using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healtbar;
    public int maxHP;
    public float currHP;

    void Update()
    {
        healtbar.fillAmount = currHP / maxHP;
    }
}
