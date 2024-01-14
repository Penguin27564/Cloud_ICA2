using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public class Skill
{
    public string name;
    public int level;
    public Skill(string _name, int _level)
    {
        name = _name;
        level = _level;
    }
}

public class SkillBox : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _skillName;
    
    [SerializeField]
    private Slider _skillLevelSlider;

    [SerializeField]
    private TMP_Text _skillLevelText;

    public Skill ReturnClass()
    {
        return new Skill(_skillName.text, (int)_skillLevelSlider.value);
    }

    public void SetUI(Skill skill)
    {
        _skillName.text = skill.name;
        _skillLevelSlider.value = skill.level;
    }

    public void SliderChangeUpdate(float num)
    {
        _skillLevelText.text = _skillLevelSlider.value.ToString();
    }
}