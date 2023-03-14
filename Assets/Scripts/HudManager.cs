using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudManager : MonoBehaviour
{
    public TextMeshProUGUI scoreLabel;

    // Show player stats in the HUD
    public void Refresh()
    {
        scoreLabel.text = "Rings: " + GameManager.instance.rings;
    }
}