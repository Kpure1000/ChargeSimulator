using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSetting : MonoBehaviour
{
    public Dropdown dropdown;

    public Text optionText;

    public Text ScreenText;

    Resolution[] resolutions;

    private void Awake()
    {
        resolutions = Screen.resolutions;

        Screen.SetResolution(resolutions[resolutions.Length - 1].height * 4 / 3, resolutions[resolutions.Length - 1].height, true);

        ScreenText.text = "当前分辨率:\r\n" + resolutions[resolutions.Length - 1];

        foreach (var item in resolutions)
        {
            dropdown.options.Add(new Dropdown.OptionData(item.ToString()));
        }
    }

    public void OnDropDownChange()
    {
        foreach (var item in resolutions)
        {
            if(optionText.text.Equals(item.ToString()))
            {
                if(item.Equals(resolutions[resolutions.Length-1]))
                {
                    Screen.SetResolution(item.height * 4 / 3, item.height, true);
                }
                else
                {
                    Screen.SetResolution(item.height * 4 / 3, item.height, false);
                }
                ScreenText.text = "当前分辨率:\r\n" + item.ToString();
                break;
            }
        }
    }

}
