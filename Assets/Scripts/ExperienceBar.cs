using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] private Image experienceBarFill;
    [SerializeField] private Text experienceText;  // Optional, if you want to display the numeric value
    private int maxExperience = 10000;  // You can change this according to your leveling system

    public void SetExperience(float experiencePercentage)
    {
        experienceBarFill.fillAmount = experiencePercentage;

        // If you have an experience text
        if (experienceText != null)
        {
            experienceText.text = $"{experiencePercentage * 100}%";
        }
    }
}
