using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genCube : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject cubePrefab;  // 立方体预制体
    public Transform spawnReference;  // 生成参考点（XR Rig）
    //public float spawnInterval = 5f;  // 生成间隔时间（秒）
    public float spawnDistance = 15f;  // 生成距离


    void Start()
    {
        // 如果未手动指定参考点，尝试自动查找
        if (spawnReference == null)
        {
            // 尝试按名称查找XR Rig
            spawnReference = GameObject.Find("[Building Block] PICO Controller Tracking XR Origin (XR Rig).Camera Offset.Main Camera")?.transform;

            // 如果找不到精确名称，尝试查找包含"XR Rig"的对象
            if (spawnReference == null)
            {
                Debug.LogWarning("11111111111");
                spawnReference = GameObject.FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.XRRig>()?.transform;
            }

            // 如果还是找不到，使用主摄像机
            if (spawnReference == null)
            {
                Debug.LogWarning("2222222222222222");
                spawnReference = Camera.main?.transform;
            }
        }

        // 检查预制体是否已分配
        if (cubePrefab == null)
        {
            Debug.LogWarning("未分配立方体预制体！请在Inspector窗口中分配。");
        }

        // 检查参考点是否找到
        if (spawnReference == null)
        {
            Debug.LogWarning("未找到生成参考点！请手动分配或在场景中添加XR Rig。");
        }
    }

    /*void Update()
    {
        // 更新计时器
        timer += Time.deltaTime;

        // 如果达到生成间隔时间
        if (timer >= spawnInterval)
        {
            SpawnCube();
            timer = 0f;  // 重置计时器
        }
    }*/

    // 生成立方体的方法
    void SpawnCube()
    {
        // 检查预制体和参考点是否有效
        if (cubePrefab == null || spawnReference == null)
        {
            Debug.LogWarning("无法生成立方体：预制体或参考点未设置！");
            return;
        }



        // 计算生成位置：在参考点前方指定距离
        Vector3 spawnPosition = spawnReference.position + spawnReference.forward * spawnDistance;

        // 生成立方体
        GameObject newCube = Instantiate(cubePrefab, spawnPosition, spawnReference.rotation);
        

        // 可选：为生成的立方体命名（便于调试）
        newCube.name = "GeneratedCube_" + Time.time;
        my_cube cubeScript = newCube.GetComponent<my_cube>();
        cubeScript.Initialize(spawnPosition,spawnReference.position);


        // 可选：输出调试信息
        Debug.Log($"在位置 {spawnPosition} 生成立方体");
        // 在10秒后销毁这个立方体
        Destroy(newCube, 10f);  // 10秒后销毁
    }

    // 手动调用生成立方体（可选，可用于测试）
    public void SpawnCubeManually()
    {
        SpawnCube();
    }
}