using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CastingMinigame : MonoBehaviour, IPowerMinigame
{
    public GameObject powerBarGameObject;
    public Image PowerBarMask;
    public float barChangeSpeed = 1;
    float maxPowerBarValue = 100;
    [SerializeField]
    public float currentPowerBarValue;
    bool powerIsIncreasing;
    bool PowerBarOn;

    public event Action<float> OnPowerChanged;

    private void OnEnable()
    {
        ResetState();
    }

    private void ResetState()
    {
        currentPowerBarValue = 0;
        powerIsIncreasing = true;
        PowerBarOn = true;
    }

    public float GetPowerValue()
    {
        return currentPowerBarValue / maxPowerBarValue;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        ResetState();
        StartCoroutine(UpdatePowerBar());
    }

    public void Deactivate()
    {
        PowerBarOn = false;
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    
    IEnumerator  UpdatePowerBar()
    {
        while (PowerBarOn)
        {
            if (!powerIsIncreasing)
            {
                currentPowerBarValue -= barChangeSpeed;
                if (currentPowerBarValue <= 0 ) 
                {
                    powerIsIncreasing = true;
                }
            }
            if (powerIsIncreasing)
            {
                currentPowerBarValue += barChangeSpeed;
                if (currentPowerBarValue >= maxPowerBarValue)
                {
                    powerIsIncreasing = false;
                }
            }

            float nomalizedValue = currentPowerBarValue / maxPowerBarValue;
            PowerBarMask.fillAmount = nomalizedValue;


            OnPowerChanged?.Invoke(nomalizedValue);

            yield return new WaitForSeconds(0.02f);
        }
    }
}
