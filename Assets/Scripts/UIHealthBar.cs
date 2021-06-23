using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public static UIHealthBar Instance = null;
    [System.Serializable]
    public struct  EnergyBarsStruct
    {
        public Image mask;
        float originalSize;
    }

    public EnergyBarsStruct[] energyBarStructs;

    public enum EnergyBars { PlayerHealth, PlayerWeapon,PlayerItemHealth};

    [SerializeField] Sprite[] energyBarSprites;
    public enum EnergyBarTypes
    {
        PlayerLife,
        BigLifeHealth,
        BomBullt,
        FireRed,
    }
   

    private void Awake()
    {
        if(Instance ==null)
        {
            Instance = this;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (EnergyBars energyBars in Enum.GetValues(typeof(EnergyBars)))
        {
           // energyBarStructs[(int)energyBars];
        }
        // get the initial height of the mask
       // originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        // adjust the height of the mask to "hide" lost health bars
      //  mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
