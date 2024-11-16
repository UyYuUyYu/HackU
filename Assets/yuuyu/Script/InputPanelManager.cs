using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject inputImagePanle,inputNamePanel,checkPanel,checkTextAndImagePanel,imageUI;

    public void OK()
    {
        checkPanel.SetActive(false);
        inputNamePanel.SetActive(false);
        inputImagePanle.SetActive(true);
    }
    public void Cansel()
    {
        checkPanel.SetActive(false);
        checkTextAndImagePanel.SetActive(false);
        imageUI.SetActive(true);
    }

    public void Kettei()
    {
        checkPanel.SetActive(true);
    }

    public void LastCheck()
    {
        checkTextAndImagePanel.SetActive(true);
    }
}
