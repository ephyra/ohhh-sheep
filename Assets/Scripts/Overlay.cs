using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlay : MonoBehaviour
{
    //insert go in order of overlay enum (pause, gameover, victory)
    [SerializeField]
    List<GameObject> overlayGOs;
    [SerializeField]
    Skills skills;
    public void OpenOverlay(int type)
    {
        OpenOverlay((OverlayType)type);
    }
    public void OpenOverlay(OverlayType type)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < overlayGOs.Count; i++)
        {
            overlayGOs[i].SetActive((int)type == i);
        }
        if (type == OverlayType.GameOver)
        {
            overlayGOs[(int)OverlayType.GameOver].SetActive(!skills.WasSkillUsed(SkillsList.Revive));
        }
    }

    public void CloseOverlay()
    {
        gameObject.SetActive(false);
    }

}
