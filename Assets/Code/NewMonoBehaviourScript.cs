using System.Collections;
using UnityEngine;

public class HealingZone : MonoBehaviour
{
    [Header("ตั้งค่าการฮีล")]
    public int healAmountPerTick = 10; // ฮีลทีละเท่าไหร่
    public float timeBetweenHeals = 1f; // ความเร็วในการฮีล (วินาที)

    private bool isHealing = false;

    // ฟังก์ชันนี้จะทำงานตลอดเวลาที่ผู้เล่นยืนอยู่ในอาณาเขต
    private void OnTriggerStay(Collider other)
    {
        // เช็คว่าคนที่มาเหยียบคือผู้เล่นใช่ไหม
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            // ถ้าเจอผู้เล่น, ผู้เล่นยังไม่ตาย, เลือดยังไม่เต็ม และไม่ได้กำลังฮีลอยู่
            if (player != null && !player.isDead && player.HP < player.maxHP && !isHealing)
            {
                StartCoroutine(HealRoutine(player));
            }
        }
    }

    private IEnumerator HealRoutine(Player player)
    {
        isHealing = true;

        // เรียกใช้ฟังก์ชัน Heal ใน Player
        player.Heal(healAmountPerTick);

        // รอเวลาก่อนจะฮีลรอบต่อไป
        yield return new WaitForSeconds(timeBetweenHeals);

        isHealing = false;
    }
}