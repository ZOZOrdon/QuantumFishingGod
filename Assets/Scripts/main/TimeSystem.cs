using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TimeSystem : MonoBehaviour
{
    [Header("时间设置")]
    [Range(0, 24)] public float startHour = 6f;           // 开始时间（小时）
    public float timeScale = 1f;                          // 时间流速（1代表现实时间）
    public float fadeDuration = 2.0f; // 交叉淡入淡出持续时间
    
    [Header("背景图片设置")]
    public Image imageA; // 第一个背景图片组件
    public Image imageB; // 第二个背景图片组件 (确保在 ImageA 上方或下方)
    public Sprite morningSprite;                          // 早晨背景
    public Sprite daySprite;                              // 白天背景
    public Sprite eveningSprite;                          // 傍晚背景
    public Sprite nightSprite;                            // 夜晚背景
    
    [Header("时间段设置")]
    [Range(0, 24)] public float morningStart = 5f;        // 早晨开始时间
    [Range(0, 24)] public float dayStart = 8f;            // 白天开始时间
    [Range(0, 24)] public float eveningStart = 17f;       // 傍晚开始时间
    [Range(0, 24)] public float nightStart = 20f;         // 夜晚开始时间
    
    private float currentHour;                            // 当前小时
    private TimeOfDay currentTimeOfDay;                   // 当前时间段
    private Coroutine fadeCoroutine;
    private Image currentActiveImage; // 当前显示（或正在淡出）的图片
    private Image nextImage;          // 下一个将要淡入的图片
    
    // 时间段枚举
    private enum TimeOfDay
    {
        Morning,
        Day,
        Evening,
        Night
    }
    
    void Start()
    {
        // *** 确保两个 Image 都已设置 ***
        if (imageA == null || imageB == null)
        {
            Debug.LogError("请在 Inspector 中设置 imageA 和 imageB！");
            enabled = false; // 禁用脚本以防出错
            return;
        }

        // 初始化时间
        currentHour = startHour;
        UpdateTimeOfDay();

        // *** 初始化图片状态 ***
        currentActiveImage = imageA;
        nextImage = imageB;
        currentActiveImage.sprite = GetSpriteForTimeOfDay(currentTimeOfDay);
        SetImageAlpha(currentActiveImage, 1f); // 当前图片完全不透明
        SetImageAlpha(nextImage, 0f);         // 下一个图片完全透明
        nextImage.sprite = null; // 开始时下一个图片不需要sprite
    }
    
    void Update()
    {
        // 更新时间
        currentHour += Time.deltaTime * timeScale / 60f; // 每分钟真实时间代表游戏内的timeScale小时
        
        // 处理24小时循环
        if (currentHour >= 24f)
        {
            currentHour -= 24f;
        }
        
        // 检查并更新时间段
        TimeOfDay previousTimeOfDay = currentTimeOfDay;
        UpdateTimeOfDay();
        
        // 只有在时间段变化时才更新背景
        if (previousTimeOfDay != currentTimeOfDay)
        {
            StartCrossfadeTransition();
        }
    }
    
    // 更新当前时间段
    private void UpdateTimeOfDay()
    {
        if (currentHour >= morningStart && currentHour < dayStart)
        {
            currentTimeOfDay = TimeOfDay.Morning;
        }
        else if (currentHour >= dayStart && currentHour < eveningStart)
        {
            currentTimeOfDay = TimeOfDay.Day;
        }
        else if (currentHour >= eveningStart && currentHour < nightStart)
        {
            currentTimeOfDay = TimeOfDay.Evening;
        }
        else
        {
            currentTimeOfDay = TimeOfDay.Night;
        }
    }
    
    private Sprite GetSpriteForTimeOfDay(TimeOfDay timeOfDay)
    {
        switch (timeOfDay)
        {
            case TimeOfDay.Morning: return morningSprite;
            case TimeOfDay.Day:     return daySprite;
            case TimeOfDay.Evening: return eveningSprite;
            case TimeOfDay.Night:   return nightSprite;
            default:                return null;
        }
    }
    
    // *** 开始交叉淡入淡出过渡 ***
    private void StartCrossfadeTransition()
    {
        // 1. 获取新时间段对应的 Sprite (调用了包含 switch 逻辑的函数)
        Sprite spriteToFadeIn = GetSpriteForTimeOfDay(currentTimeOfDay);

        // 如果目标sprite无效或与当前活动图像的sprite相同，则不进行切换
        if (spriteToFadeIn == null || (currentActiveImage.sprite == spriteToFadeIn && GetImageAlpha(currentActiveImage) == 1f))
        {
            return;
        }
        
        // 如果当前有正在运行的协程，先停止它，并立即完成上一个过渡（避免奇怪状态）
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
             // 立即设置上一个过渡的最终状态
            SetImageAlpha(currentActiveImage, 0f);
            SetImageAlpha(nextImage, 1f);
             // 交换角色
            Image temp = currentActiveImage;
            currentActiveImage = nextImage;
            nextImage = temp;
        }

        // 5. 将获取到的正确 Sprite 设置给即将淡入的 Image
        nextImage.sprite = spriteToFadeIn;

        // 6. 启动协程，执行透明度变化
        fadeCoroutine = StartCoroutine(CrossfadeTransition());
    }

    // *** 交叉淡入淡出协程 ***
    private IEnumerator CrossfadeTransition()
    {
        float timer = 0f;

        // 确保初始状态正确
        SetImageAlpha(currentActiveImage, 1f);
        SetImageAlpha(nextImage, 0f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            // 同时修改两个图片的 alpha 值
            SetImageAlpha(currentActiveImage, 1f - progress); // 当前图片淡出
            SetImageAlpha(nextImage, progress);          // 下一个图片淡入

            yield return null; // 等待下一帧
        }

        // 确保最终状态精确
        SetImageAlpha(currentActiveImage, 0f);
        SetImageAlpha(nextImage, 1f);

        // *** 切换角色：现在淡入的图片成为活动图片 ***
        Image previouslyActive = currentActiveImage;
        currentActiveImage = nextImage;
        nextImage = previouslyActive; // 旧的活动图片现在是下一个要用的（透明的）

        fadeCoroutine = null; // 标记协程结束
    }
    
    // 辅助方法：设置Image的Alpha值
    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = Mathf.Clamp01(alpha); // 限制 alpha 在 0 到 1 之间
        image.color = color;
    }
    
    // 辅助方法：获取Image的Alpha值
    private float GetImageAlpha(Image image)
    {
        return image.color.a;
    }
    
    // 获取格式化的当前时间字符串 (HH:MM)
    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(currentHour);
        int minutes = Mathf.FloorToInt((currentHour - hours) * 60f);
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }
    
    // 获取当前时间（小时，0-24范围内的浮点数）
    public float GetCurrentHour()
    {
        return currentHour;
    }
}

