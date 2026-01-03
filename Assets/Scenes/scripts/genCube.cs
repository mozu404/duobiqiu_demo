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
    public float spawnIntervalMax = 10f;  // 生成间隔时间（秒）
    public float originalSpawnIntervalMax = 10f; //原始最大间隔时间，和上面的max同步设置

    public float spawnDistance = 12f;  // 生成距离
    private float currentSpawnInterval = 1f;//开始的时候1s生成
    private float timer = 0f;
    public TMP_Text changemodeText;
    public TMP_Text gameTime; //游戏时间

    private float start_timer = 0f; //游戏开始时间
 
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
        // 更新陨石生成计时器
        timer += Time.deltaTime;

        // 如果达到当前生成间隔时间
        if (timer >= currentSpawnInterval)
        {
            SpawnCube();
            timer = 0f;                     // 重置计时器
            SetRandomSpawnInterval();       // 重新设置随机间隔
        }

        //更新游戏时间
        start_timer += Time.deltaTime;

        // 计算应该减少的次数
        int reductionCount = Mathf.FloorToInt(start_timer / 5f);
        // 确保不低于最小值
        if (spawnIntervalMax > spawnIntervalMin)
        {
            float newInterval = originalSpawnIntervalMax - (reductionCount * 0.5f);
            spawnIntervalMax = Mathf.Max(spawnIntervalMin, newInterval);
        }


        // 更新显示
        UpdateGameTimeDisplay();

        if (ScoreManager.Instance.GetCurrentScore() < 0)
        {
            StopChallenge();
        }

    }


    // 更新游戏时间显示
    void UpdateGameTimeDisplay()
    {
        if (gameTime != null)
        {
            gameTime.text = "时间：" + start_timer.ToString("F0") + "s";  // 保留0位小数
        }
    }

    // 获取当前游戏时间（如果需要用到）
    public float GetGameTime()
    {
        return start_timer;
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

        Vector3 targetPos = spawnReference.position;
        Vector3 randomOffset2 = new Vector3(
            Random.Range(-0.01f, 0.01f),
            Random.Range(-0.01f, 0.01f),
            Random.Range(-0.01f, 0.01f)
        );
        targetPos = targetPos + randomOffset2;

        cubeScript.Initialize(spawnPosition,targetPos);


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
        start_timer = 0f;

    }

    void StopChallenge()
    {
        // 停止计时和其他游戏逻辑
        is_chuangguan = false;

        // 将坚持的时间转换为整数（四舍五入）
        int timePlayed = Mathf.RoundToInt(start_timer);

        // 在摄像机前显示坚持了多久（使用UI显示）
        ShowTimePlayedMessage(timePlayed);

        // 可选：播放音效或触发其他结束事件
        Debug.Log($"闯关结束，坚持了 {timePlayed} 秒");
    }

    void ShowTimePlayedMessage(int seconds)
    {
        // 创建UI文本显示（使用世界空间UI）
        GameObject timeMessage = new GameObject("TimeMessage");

        // 将UI放置在摄像机前
        Transform cameraTransform = Camera.main.transform;
        timeMessage.transform.position = cameraTransform.position + cameraTransform.forward * 2f; // 距离摄像机2个单位

        // 始终面向摄像机
        timeMessage.transform.LookAt(cameraTransform);
        timeMessage.transform.Rotate(0, 180f, 0); // 让文字正确朝向摄像机

        // 添加TextMeshPro组件
        TextMeshPro tmp = timeMessage.AddComponent<TextMeshPro>();

        // 关键修改：设置字体为changemodeText的字体
        if (changemodeText != null)
        {
            tmp.font = changemodeText.font; // 设置相同字体
            tmp.fontStyle = changemodeText.fontStyle; // 可选：设置相同字体样式
            tmp.color = changemodeText.color; // 可选：设置相同颜色
        }

        tmp.text = $"坚持了 {seconds} 秒!";
        tmp.fontSize = 5;
        tmp.color = Color.yellow;
        tmp.alignment = TextAlignmentOptions.Center;

        // 添加淡出效果（可选）
        StartCoroutine(FadeOutAndDestroy(tmp, 3f));
    }

    // 淡出并销毁的协程
    IEnumerator FadeOutAndDestroy(TextMeshPro tmp, float duration)
    {
        float elapsed = 0f;
        Color startColor = tmp.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            tmp.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(tmp.gameObject);
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