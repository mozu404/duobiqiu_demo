using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class genCube : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject cubePrefab;  // 立方体预制体
    public Transform spawnReference;  // 生成参考点（XR Rig）
    public float spawnIntervalMin = 2f;  // 生成间隔时间（秒）
    public float spawnIntervalMax = 8f;  // 生成间隔时间（秒）
    public float spawnDistance = 15f;  // 生成距离
    private float currentSpawnInterval = 1f;//开始的时候1s生成
    private float timer = 0f;
    public TMP_Text changemodeText;


    public bool is_chuangguan = false;

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

    void Update()
    {

        if (!is_chuangguan) {
            return;
        }
        // 更新计时器
        timer += Time.deltaTime;

        // 如果达到当前生成间隔时间
        if (timer >= currentSpawnInterval)
        {
            SpawnCube();
            timer = 0f;                     // 重置计时器
            SetRandomSpawnInterval();       // 重新设置随机间隔
        }
    }
    // 设置随机生成间隔
    void SetRandomSpawnInterval()
    {
        currentSpawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

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
        Vector3 baseDirection = spawnReference.position + spawnReference.forward * spawnDistance;
        // 在0.2立方体内随机偏移
        Vector3 randomOffset = new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f)
        );

        // 计算最终位置
        Vector3 spawnPosition = baseDirection + randomOffset;

        // 生成立方体
        GameObject newCube = Instantiate(cubePrefab, spawnPosition, spawnReference.rotation);
        

        // 可选：为生成的立方体命名（便于调试）
        newCube.name = "GeneratedCube_" + Time.time;
        my_cube cubeScript = newCube.GetComponent<my_cube>();
        cubeScript.Initialize(spawnPosition,spawnReference.position);


        // 可选：输出调试信息
        Debug.Log($"在位置 {spawnPosition} 生成立方体");
        // 在10秒后销毁这个立方体
        Destroy(newCube, 7f);  // 7秒后销毁
    }

    // 手动调用生成立方体（可选，可用于测试）
    public void SpawnCubeManually()
    {
        SpawnCube();
    }

    // 开启/关闭闯关模式
    public void start_game()
    {
        // 切换闯关状态
        is_chuangguan = !is_chuangguan;
        if (is_chuangguan)
        {
            // 开启闯关
            StartChallenge();
        }
        else
        {
            // 关闭闯关
            StopChallenge();
        }
        // 更新按钮文字
        UpdatechangemodeText();
    }


    void StartChallenge()
    {
        ScoreManager.Instance.SetScore(50);

    }

    void StopChallenge()
    {
        return;
    }

    // 更新按钮文字
    void UpdatechangemodeText()
    {
        if (changemodeText != null)
        {
            changemodeText.text = is_chuangguan ? "生成陨石中..." : "开始闯关";
        }
    }

}