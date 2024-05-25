using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float initialHorizontalSpeed = 5f; // 初始横向速度
    public float verticalSpeed = -2f; // 纵向速度
    public float increaseFactor = 2f; // 长按鼠标时横向速度增加的倍数
    public float maxHorizontalSpeed = 10f; // 最大横向速度
    public float accelerationRate = 2f; // 加速度

    private bool isPressed = false;
    private float currentHorizontalSpeed; // 当前横向速度

    void Start()
    {
        currentHorizontalSpeed = initialHorizontalSpeed;
    }

    void Update()
    {
        // 获取当前物体的位置
        Vector3 currentPosition = transform.position;

        // 检测鼠标点击事件
        if (Input.GetMouseButtonDown(0))
        {
            // 点击鼠标时，横向速度逐渐变为负方向
            currentHorizontalSpeed = -Mathf.Abs(initialHorizontalSpeed);
        }
        else if (Input.GetMouseButton(0))
        {
            // 长按鼠标时，横向速度逐渐增加，方向不变
            isPressed = true;
            currentHorizontalSpeed = Mathf.Min(currentHorizontalSpeed + accelerationRate * Time.deltaTime, maxHorizontalSpeed);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 松开鼠标时，横向速度逐渐恢复到初始值
            isPressed = false;
            currentHorizontalSpeed = Mathf.Max(currentHorizontalSpeed - accelerationRate * Time.deltaTime, initialHorizontalSpeed);
        }

        // 根据速度更新物体的位置
        float newX = currentPosition.x + currentHorizontalSpeed * Time.deltaTime;
        float newY = currentPosition.y + verticalSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, newY, currentPosition.z);
    }
}