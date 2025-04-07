using UnityEngine;
using UnityEngine.UI;

public class ImagePanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel; // 要显示的Panel
    [SerializeField] private Camera mainCamera; // 主相机
    [SerializeField] private float transitionSpeed = 2f; // 过渡速度
    [SerializeField] private float zoomFactor = 2f; // 放大倍数
    
    private Vector3 mainCameraPosition;
    private Quaternion mainCameraRotation;
    private float mainCameraFieldOfView;
    private bool isTransitioning = false;
    private float transitionProgress = 0f;
    
    private void Start()
    {
        // 保存主相机的初始状态
        mainCameraPosition = mainCamera.transform.position;
        mainCameraRotation = mainCamera.transform.rotation;
        mainCameraFieldOfView = mainCamera.fieldOfView;
        
        // 确保Panel初始状态是隐藏的
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
    
    private void Update()
    {
        // 检测ESC键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainView();
        }
        
        if (isTransitioning)
        {
            HandleTransition();
        }
    }
    
    public void OnImageClick()
    {
        // 显示Panel
        if (panel != null)
        {
            panel.SetActive(true);
        }
        
        // 开始相机过渡
        StartTransition();
    }
    
    private void StartTransition()
    {
        isTransitioning = true;
        transitionProgress = 0f;
        
        // 计算目标位置（在物体前方）
        Vector3 targetPosition = transform.position - transform.forward * 2f;
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - targetPosition);
        float targetFieldOfView = mainCameraFieldOfView / zoomFactor;
        
        // 平滑过渡
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, transitionProgress);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRotation, transitionProgress);
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFieldOfView, transitionProgress);
    }
    
    private void ReturnToMainView()
    {
        // 隐藏Panel
        if (panel != null)
        {
            panel.SetActive(false);
        }
        
        // 重置相机
        mainCamera.transform.position = mainCameraPosition;
        mainCamera.transform.rotation = mainCameraRotation;
        mainCamera.fieldOfView = mainCameraFieldOfView;
    }
    
    private void HandleTransition()
    {
        transitionProgress += Time.deltaTime * transitionSpeed;
        
        if (transitionProgress >= 1f)
        {
            transitionProgress = 1f;
            isTransitioning = false;
        }
    }
} 