using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private float zoomFactor = 2f;
    
    private Vector3 mainCameraPosition;
    private Quaternion mainCameraRotation;
    private float mainCameraFieldOfView;
    
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float targetFieldOfView;
    
    private bool isTransitioning = false;
    private float transitionProgress = 0f;
    
    private void Start()
    {
        // 保存主相机的初始状态
        mainCameraPosition = mainCamera.transform.position;
        mainCameraRotation = mainCamera.transform.rotation;
        mainCameraFieldOfView = mainCamera.fieldOfView;
    }
    
    private void Update()
    {
        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == "fish" || hit.collider.gameObject.name == "frog")
                {
                    StartTransition(hit.collider.gameObject);
                }
            }
        }
        
        // 检测回退键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainView();
        }
        
        if (isTransitioning)
        {
            HandleTransition();
        }
    }
    
    private void StartTransition(GameObject target)
    {
        isTransitioning = true;
        transitionProgress = 0f;
        
        // 计算目标位置（在目标物体前方）
        targetPosition = target.transform.position - target.transform.forward * 2f;
        targetRotation = Quaternion.LookRotation(target.transform.position - targetPosition);
        targetFieldOfView = mainCameraFieldOfView / zoomFactor;
    }
    
    private void ReturnToMainView()
    {
        isTransitioning = true;
        transitionProgress = 0f;
        
        targetPosition = mainCameraPosition;
        targetRotation = mainCameraRotation;
        targetFieldOfView = mainCameraFieldOfView;
    }
    
    private void HandleTransition()
    {
        transitionProgress += Time.deltaTime * transitionSpeed;
        
        if (transitionProgress >= 1f)
        {
            // 过渡完成
            transitionProgress = 1f;
            isTransitioning = false;
        }
        
        // 使用平滑插值
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, transitionProgress);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRotation, transitionProgress);
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFieldOfView, transitionProgress);
    }
} 