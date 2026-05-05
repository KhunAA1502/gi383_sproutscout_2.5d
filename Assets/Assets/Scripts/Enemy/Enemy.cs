using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : Character 
{
    public enum State { Idle, Chasing, Attacking }
    [Header("Target Settings")]
    public Transform target; // ลาก Player มาใส่ที่นี่
    
    private EnemyMovement movement;

    protected override void Awake()
    {
      base.Awake();
    movement = GetComponent<EnemyMovement>();

    // ถ้า Target ว่างอยู่ ให้พยายามหา Object ที่มี Tag ว่า "Player" ในฉากอัตโนมัติ
    if (target == null)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }
    }

   void Update()
{
   if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy)
   {
    movement.MoveTowards(PlayerController.instance.transform.position);
   }
   
}
}