using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public int maxHP = 100;
    public GameObject bloodyScreen;

    public TextMeshProUGUI playerHelthUI;
    public GameObject gameOverUI;
    public Button Retry;
    public Button Exit;

    public bool isDead;

    private void Start()
    {
        playerHelthUI.text = $"HP:{HP}";
    }

    public void Heal(int healAmount)
    {
        if (isDead) return; // ถ้าตายแล้วไม่ต้องฮีล

        HP += healAmount; // เพิ่มเลือด

        // ป้องกันไม่ให้เลือดทะลุหลอด
        if (HP > maxHP)
        {
            HP = maxHP;
        }

        // อัปเดต UI 
        playerHelthUI.text = $"HP:{HP}";

        // (ถ้ามีเสียงตอนฮีล สามารถสั่งเล่นเสียงตรงนี้ได้ครับ)
    }

    public void TakeDamage(int damageAmout)
    {
        HP -= damageAmout;

        if (HP <= 0)
        {
            print("Player Dead");
            PlayerDead();
            isDead = true;
        }
        else
        {
            print("Player Hit");
            StartCoroutine(BloodyScreenEffect());
            playerHelthUI.text = $"HP : {HP}";
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);
        }
    }

    private void PlayerDead()
    {
        SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDie);

        GetComponent<MouseMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        if (SoundManager.Instance.zombieChannel2 != null)
        {
            SoundManager.Instance.zombieChannel2.Stop(); 
            SoundManager.Instance.zombieChannel2.mute = true; 
            SoundManager.Instance.zombieChannel.Stop(); 
            SoundManager.Instance.zombieChannel.mute = true; 
        }

        // dying animation


        playerHelthUI.gameObject.SetActive(false);

        GetComponent<ScreenFader>().StartFade();
        StartCoroutine(ShowGameOverUI());
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.gameObject.SetActive(true);
        Retry.gameObject.SetActive(true);
        Exit.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;                  
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(true);
        }

        var image = bloodyScreen.GetComponentInChildren<Image>();

        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;

            yield return null; ; // Wait for the next frame.
        }

        if (bloodyScreen.activeInHierarchy == false)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieHand"))
        {
            if (isDead == false)
            {
                TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
            }
        }
    }
}
