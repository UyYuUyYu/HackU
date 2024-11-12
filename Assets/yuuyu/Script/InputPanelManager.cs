using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject inputImagePanle,inputNamePanel,checkPanel,checkTextAndImagePanel;

    public void OK()
    {
        inputImagePanle.SetActive(true);
        checkPanel.SetActive(false);
        inputNamePanel.SetActive(false);

    }
    public void Cansel()
    {
        checkPanel.SetActive(false);
        checkTextAndImagePanel.SetActive(false);
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
