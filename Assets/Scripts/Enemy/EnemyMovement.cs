using UnityEngine;
using UnityEngine.AI; // จำเป็นต้องใช้สำหรับ NavMesh

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public SpriteRenderer enemySprite;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // สำหรับเกม 2.5D/3D ที่ใช้ Sprite:
        // ปิดการหมุนตัวของ Agent เพราะเราจะใช้การ Flip Sprite แทน
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        if (agent == null) return;

        // สั่งให้ Agent เดินไปที่เป้าหมาย (มันจะหาทางอ้อมสิ่งกีดขวางเอง)
        agent.SetDestination(targetPosition);

        // การหันหน้า Sprite (เช็คจากความเร็วปัจจุบันของ Agent)
        if (agent.velocity.x > 0.01f)
            enemySprite.flipX = false;
        else if (agent.velocity.x < -0.01f)
            enemySprite.flipX = true;
    }

    public void Stop()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
    }

    public void Resume()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }
}