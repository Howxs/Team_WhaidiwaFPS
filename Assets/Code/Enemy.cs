using System.Collections; // ต้องมีบรรทัดนี้เพื่อใช้ระบบนับเวลา (Coroutine)
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;

    private NavMeshAgent navAgent;

    public bool isDead;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damageAmout)
    {
        // ถ้าตายไปแล้ว ไม่ต้องทำอะไรต่อ (ป้องกันบัคโดนยิงซ้ำตอนล้ม)
        if (isDead) return;

        HP -= damageAmout;

        if (HP <= 0)
        {
            int randomValue = Random.Range(0, 2); // 0 or 1

            if (randomValue == 0)
            {
                animator.SetTrigger("DIE1");
            }
            else
            {
                animator.SetTrigger("DIE2");
            }

            isDead = true;

            // Dead Sound
            SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieDeath);

            // --- สิ่งที่เพิ่มเข้ามา ---
            // 1. ปิด NavMeshAgent เพื่อให้ซอมบี้หยุดเดิน
            if (navAgent != null) navAgent.enabled = false;

            // 2. ปิด Collider เพื่อไม่ให้กลายเป็นกำแพงล่องหนขวางทางเดิน
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // 3. เริ่มนับเวลาเพื่อทำลาย Object ทิ้ง
            StartCoroutine(DestroyAfterAnimation());
            // ------------------------
        }
        else
        {
            animator.SetTrigger("DAMAGE");

            // Hurt Sound
            SoundManager.Instance.zombieChannel2.PlayOneShot(SoundManager.Instance.zombieHurt);
        }
    }

    // ฟังก์ชันนับเวลาถอยหลังก่อนลบศัตรูทิ้ง
    private IEnumerator DestroyAfterAnimation()
    {
        // หน่วงเวลา 3 วินาที (คุณสามารถเปลี่ยนตัวเลข 3f ให้ตรงกับความยาวแอนิเมชันตายของคุณได้เลยครับ)
        yield return new WaitForSeconds(3f);

        // ลบศัตรูออกจากฉาก
        Destroy(gameObject);
    }

    private void OnDrawGizmos() // แก้คำผิดจาก OnDrawGizmas เป็น OnDrawGizmos ด้วยครับ จะได้แสดงผลเส้นขอบเขตได้ถูกต้อง
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.5f); // Attacking // Stop Attacking

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 18f); // Detection

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 21f); // Stop Chasing
    }
}