using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skills : MonoBehaviour
{
    List<SkillsList> skillUsed;
    [SerializeField]
    List<Button> skillsButton;
    [SerializeField]
    Hand hand;
    [SerializeField]
    Text errorText;
    // Start is called before the first frame update
    void Start()
    {
        skillUsed = new List<SkillsList>();
    }

    public void UseSkill(int type)
    {
        UseSkill((SkillsList)type);
    }

    public void UseSkill(SkillsList skillType)
    {
        if (!WasSkillUsed(skillType))
        {
            bool skillSucceed = true;
            switch (skillType)
            {
                case SkillsList.Store:
                case SkillsList.Revive:
                    skillSucceed = hand.Store();
                    break;
                case SkillsList.Undo:
                    skillSucceed = hand.Undo();
                    break;
                case SkillsList.Shuffle:
                    GameManager.Instance.Shuffle();
                    break;
                default:
                    break;
            }
            if (skillSucceed)
            {
                skillsButton[(int)skillType].interactable = false;
                if (skillType == SkillsList.Revive)
                {
                    GameObject revButton = skillsButton[(int)skillType].gameObject;
                    revButton.SetActive(false);
                    //remove game over overlay
                    revButton.transform.parent.gameObject.SetActive(false);
                }
                skillUsed.Add(skillType);
            } else
            {
                if (skillType == SkillsList.Store)
                {
                    StartCoroutine(DisplayError("No tiles to store"));
                } else if (skillType == SkillsList.Undo)
                {
                    StartCoroutine(DisplayError("No valid undo To Perform"));
                }

            }
        }
    }

    IEnumerator DisplayError(string text)
    {
        errorText.text = text;
        errorText.enabled = true;
        yield return new WaitForSeconds(1.5f);
        errorText.enabled = false;
    }
    public bool WasSkillUsed(SkillsList skillType)
    {
        return skillUsed.Contains(skillType);
    }
}
