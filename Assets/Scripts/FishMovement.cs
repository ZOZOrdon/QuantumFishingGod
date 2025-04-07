using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("运动参数")]
    public float moveSpeed = 2f;        // 移动速度
    public float swimRange = 5f;        // 游动范围
    public float turnSpeed = 2f;        // 转向速度
    
    [Header("摆动参数")]
    public float swayAmount = 0.5f;     // 上下摆动幅度
    public float swaySpeed = 2f;        // 上下摆动速度
    public float ySwayAmount = 0.3f;    // 上下摆动幅度
    public float ySwaySpeed = 1.5f;     // 上下摆动速度
    public float rotationSwayAmount = 5f; // 旋转摆动幅度
    public float rotationSwaySpeed = 1.8f; // 旋转摆动速度
    
    private Vector3 startPosition;      // 初始位置
    private float direction = 1f;       // 移动方向
    private float targetRotation;       // 目标旋转角度
    private float swayTime;             // 摆动时间
    private float ySwayTime;            // 上下摆动时间
    private float rotationSwayTime;     // 旋转摆动时间
    
    void Start()
    {
        startPosition = transform.position;
        // 根据初始朝向设置目标旋转
        targetRotation = transform.eulerAngles.y;
        // 根据初始朝向设置移动方向（取反，使方向与贴图一致）
        direction = -Mathf.Sign(Mathf.Cos(targetRotation * Mathf.Deg2Rad));
        swayTime = 0f;
        ySwayTime = 0f;
        rotationSwayTime = 0f;
    }
    
    void Update()
    {
        // 计算新位置
        float newX = transform.position.x + moveSpeed * direction * Time.deltaTime;
        
        // 检查是否超出范围
        if (Mathf.Abs(newX - startPosition.x) > swimRange)
        {
            direction *= -1; // 改变方向
            // 根据移动方向设置目标旋转（180度表示向右，0度表示向左）
            targetRotation = direction > 0 ? 180f : 0f;
        }
        
        // 计算旋转摆动
        rotationSwayTime += Time.deltaTime * rotationSwaySpeed;
        float rotationSwayOffset = Mathf.Sin(rotationSwayTime) * rotationSwayAmount;
        
        // 平滑旋转（添加摆动效果）
        float currentRotation = transform.eulerAngles.y;
        float newRotation = Mathf.LerpAngle(currentRotation, targetRotation + rotationSwayOffset, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, newRotation, 0);
        
        // 计算摆动
        swayTime += Time.deltaTime * swaySpeed;
        float swayOffset = Mathf.Sin(swayTime) * swayAmount;
        
        // 计算上下摆动
        ySwayTime += Time.deltaTime * ySwaySpeed;
        float ySwayOffset = Mathf.Sin(ySwayTime) * ySwayAmount;
        
        // 更新位置（添加摆动效果）
        transform.position = new Vector3(newX, 
                                       startPosition.y + swayOffset + ySwayOffset, 
                                       transform.position.z);
    }
} 