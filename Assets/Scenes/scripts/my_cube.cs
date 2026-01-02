// CubeMovement.cs - 附加到Cube预制体上或动态添加
using UnityEngine;

public class my_cube : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 2f;  // 移动速度
    [SerializeField] private bool useFixedDirection = true;  // 是否使用固定方向
        [Header("生成设置")]
    public GameObject cubePrefab;  // 立方体预制体

    public  Vector3 targetPosition;  // 目标位置
    private  Vector3 moveDirection;   // 移动方向
    private bool isMoving = true;    // 是否正在移动

    // 初始化移动参数
    public void Initialize(Vector3 startPos, Vector3 targetPos, float speed = 2f)
    {
        targetPosition = targetPos;
        moveSpeed = speed;

        if (useFixedDirection)
        {
            // 计算生成时的固定方向（从生成位置指向目标位置）
            moveDirection = (targetPosition - startPos).normalized;
        }
        else
        {
            // 或者每帧重新计算方向（会跟随目标位置变化）
            // 这里使用固定方向
            moveDirection = (targetPosition - startPos).normalized;
        }

        isMoving = true;
        Rigidbody  rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        if (!isMoving) return;

        if (useFixedDirection)
        {
            // 方法1：使用固定方向移动
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 方法2：每帧重新计算方向（会追踪移动的目标）
            Vector3 currentDirection = (targetPosition - transform.position).normalized;
            transform.position += currentDirection * moveSpeed * Time.deltaTime;
        }

        // 检查是否到达目标位置附近
        Transform spawnReference = Camera.main?.transform;

        float distanceToTarget = Vector3.Distance(transform.position, spawnReference.position);
        if (distanceToTarget < 0.01f)
        {
            // 到达目标，停止移动或销毁
            OnReachTarget();
        }
    }

    void OnReachTarget()
    {
        isMoving = false;
        // 可以在这里添加到达目标后的处理，比如销毁

        // 或者改变颜色表示到达
        //GetComponent<MeshRenderer>().material.color = Color.red;
        ScoreManager.Instance.SubtractScore(10);
        CreateFragments();
        Destroy(gameObject,2f);
    }

    void CreateFragments()
    {
        // 隐藏原物体
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // 创建碎片
        int fragmentCount = Random.Range(10, 20);
        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fragment.transform.position = transform.position + Random.insideUnitSphere * 0.5f;
            fragment.transform.localScale = Vector3.one * Random.Range(0.03f, 0.06f);

            // 添加刚体和力
            Rigidbody rb = fragment.AddComponent<Rigidbody>();
            rb.AddExplosionForce(200f, transform.position, 3f);

            //材质
            Material usedMaterial = GetComponent<MeshRenderer>().material;
            if (usedMaterial == null)
            {
                // 创建默认材质
                usedMaterial = new Material(Shader.Find("Standard"));
                usedMaterial.color = Color.white;
            }

            // 设置材质
            fragment.GetComponent<MeshRenderer>().material = usedMaterial;

            // 销毁碎片
            Destroy(fragment, Random.Range(1f, 3f));
        }
    }


    // 可选的：在编辑器中可视化移动方向
    void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, moveDirection * 2f);
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
        }
    }
}