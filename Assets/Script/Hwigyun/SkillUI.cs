using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public bool isEnforce;
    public Image profile;
    public Sprite[] profilesImage;

    [Header("스킬 1 설정")]
    public Button skillButton1;
    public Image skillCooldownOverlay1;
    public float skill1CooldownTime = 3f;
    private bool isSkill1OnCooldown = false;

    [Header("스킬 2 설정")]
    public Button skillButton2;
    public Image skillCooldownOverlay2;
    public float skill2CooldownTime = 3f;
    private bool isSkill2OnCooldown = false;

    private void OnEnable()
    {
        if (InputManager.Instance != null && InputManager.Instance.PlayerInputActions != null)
        {
            InputManager.Instance.PlayerInputActions.UI.Skill1.performed += OnSkill1InputPerformed;
            InputManager.Instance.PlayerInputActions.UI.Skill2.performed += OnSkill2InputPerformed;
            Debug.Log("스킬 UI 인풋액션 성공적");
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.PlayerInputActions != null)
        {
            InputManager.Instance.PlayerInputActions.UI.Skill1.performed -= OnSkill1InputPerformed;
            InputManager.Instance.PlayerInputActions.UI.Skill2.performed -= OnSkill2InputPerformed;
            Debug.Log("스킬 UI 인풋액션 실패적");
        }
    }

    private void Start()
    {
        skillCooldownOverlay1.fillAmount = 0;
        skillCooldownOverlay2.fillAmount = 0;
        isSkill1OnCooldown = false;
        isSkill2OnCooldown = false;
    }

    private void FixedUpdate()
    {
        if(isEnforce)
        {
            profile.sprite = profilesImage[1];
            skillButton2.gameObject.SetActive(true);
        }
        else
        {
            profile.sprite = profilesImage[0];
            skillButton2.gameObject.SetActive(false);
        }
    }

    private void OnSkill1InputPerformed(InputAction.CallbackContext context)
    {
        TryUseSkill1();
    }

    private void OnSkill2InputPerformed(InputAction.CallbackContext context)
    {
        TryUseSkill2();
    }

    private void TryUseSkill1()
    {
        if (!isSkill1OnCooldown)
        {
            Debug.Log("스킬1 사용");
            StartCoroutine(StartCooldown(skillCooldownOverlay1, skill1CooldownTime, skillButton1, 1));
        }
    }

    private void TryUseSkill2()
    {
        if (!isSkill2OnCooldown)
        {
            Debug.Log("스킬2 사용");
            StartCoroutine(StartCooldown(skillCooldownOverlay2, skill1CooldownTime, skillButton2, 2));
        }
    }

    IEnumerator StartCooldown(Image cooldownOverlay, float cooldownTime, Button skillButton, int skillIndex)
    {
        if (skillIndex == 1)
        {
            isSkill1OnCooldown = true;
        }
        else if (skillIndex == 2)
        {
            isSkill2OnCooldown = true;
        }

        skillButton.interactable = false;

        float timer = cooldownTime;
        cooldownOverlay.fillAmount = 1;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            cooldownOverlay.fillAmount = timer / cooldownTime;
            yield return null;
        }

        cooldownOverlay.fillAmount = 0;

        if (skillIndex == 1)
        {
            isSkill1OnCooldown = false;
        }
        else if (skillIndex == 2)
        {
            isSkill2OnCooldown = false;
        }

        skillButton.interactable = true;
    }

    public void ResetAllCooldowns()
    {
        if (skillCooldownOverlay1 != null) skillCooldownOverlay1.fillAmount = 0;
        if (skillCooldownOverlay2 != null) skillCooldownOverlay2.fillAmount = 0;
        isSkill1OnCooldown = false;
        isSkill2OnCooldown = false;
        if (skillButton1 != null) skillButton1.interactable = true;
        if (skillButton2 != null) skillButton2.interactable = true;
    }
}
