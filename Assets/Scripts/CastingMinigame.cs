using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CastingMinigame : MonoBehaviour
{
    public GameObject powerBarGameObject;
    public Image PowerBarMask;
    public float barChangeSpeed = 1;
    float maxPowerBarValue = 100;
    float currentPowerBarValue;
    bool powerIsIncreasing;
    bool PowerBarOn;

    private void Start()
    {
        currentPowerBarValue = 0;
        powerIsIncreasing = true;
        PowerBarOn = true;
        StartCoroutine(UpdatePowerBar());
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
            
            float fill = currentPowerBarValue / maxPowerBarValue;
            PowerBarMask.fillAmount = fill;
            yield return new WaitForSeconds(0.02f);

            if (Input.GetKeyUp(KeyCode.Space))
            {
                PowerBarOn = false;
                castLure(currentPowerBarValue);
            }
        }
        yield return null;
    }

    public void castLure(float castFoce)
    {
        
        Debug.Log($"Casting with force {castFoce}");
    }

    

}
