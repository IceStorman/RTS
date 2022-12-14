using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public SkillData skill;
    private GameObject _source;
    private Button _button;
    private bool _ready;

    public void Initialize(SkillData skill, GameObject source)
    {
        this.skill = skill;
        _source = source;
    }

    public void Trigger(GameObject target = null)
    {
        if (!_ready) return;
        StartCoroutine(WrappedTrigger(target));
    }

    private IEnumerator WrappedTrigger(GameObject target)
    {
        yield return new WaitForSeconds(skill.castTime);
        skill.Trigger(_source, target);
        SetReady(false);
        yield return new WaitForSeconds(skill.cooldown);
        SetReady(true);
    }

    private void SetReady(bool ready)
    {
        _ready = ready;
        if (_button != null) _button.interactable = ready;
    }

    public void SetButton(Button button)
    {
        _button = button;
        SetReady(true);
    }
}
