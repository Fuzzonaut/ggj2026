using UnityEngine;
using UnityEngine.UI;

public class InsanityManager : MonoBehaviour
{
    public float insanity = 0f;
    public float maxInsanity = 100f;
    public float increaseRate = 0.7f;

    public Slider insanitySlider;

    void Start()
    {
        insanitySlider.minValue = 0;
        insanitySlider.maxValue = maxInsanity;
        insanitySlider.value = insanity;
    }

    void Update()
    {
        insanity += increaseRate * Time.deltaTime;
        insanity = Mathf.Clamp(insanity, 0, maxInsanity);

        insanitySlider.value = insanity;
    }
    public void ReduceInsanity(float amount)
    {
        insanity -= amount;
        
        
        if (insanity < 0) insanity = 0;
        if (insanitySlider != null)
    {
        insanitySlider.value = insanity;
    }
}
    }

