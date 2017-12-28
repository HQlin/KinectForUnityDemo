//放在摄像头上，实现旋转、缩放观察物体
using UnityEngine;

public class BaseCam : MonoBehaviour
{

    public Vector3 mousePos1;                           //记录鼠标点下去瞬间的位置
    public Vector3 mousePos2;                           //记录鼠标任何时刻的位置
    public Quaternion start_qua;                        //角度使用四元数
    public Vector3 start_pos;                           //位置坐标

    // Use this for initialization
    void Start()
    {
        //初始化上下角度状态
        transform.rotation = new Quaternion(0.3f, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        transform.position = new Vector3(0, 1, -17);
        //记录相机开始的角度与位置
        start_qua = transform.rotation;
        start_pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //记录鼠标实时移动的点
        mousePos2 = Input.mousePosition;
    }

    void OnGUI()
    {
        //鼠标左键
        if (Input.GetMouseButton(0))
        {
            mousePos1 = Input.mousePosition;            //记录鼠标点击瞬间的点
            Vector3 offset = mousePos1 - mousePos2;     //记录鼠标移动的距离

            //上下与左右 旋转分开，绝对值比较
            if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
            {
                //以物体上方为旋转轴（Vector3.up == new Vector3(0, 1.0f, 0)），物体左右旋转角度与鼠标横向移动距离相关，变化速率2f
                transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, Time.deltaTime * offset.x * 2f);
            }
            else
            {
                //以世界坐标右方为旋转轴（transform.right，是会变化的量），物体上下旋转角度与鼠标纵向移动距离相关，变化速率2f
                //transform.RotateAround(new Vector3(0, 0, 0), transform.right, -Time.deltaTime * offset.y * 2f);
            }

            //打印数据transform.right变量
            Debug.Log("pos: " + transform.right);
        }
        //鼠标中键，物体恢复原来的角度与位置
        if (Input.GetMouseButton(2))
        {
            transform.rotation = start_qua;
            transform.position = start_pos;
        }
        //鼠标中键滑动，物体缩放，摄像头前后移动距离范围在2f~5f，变化速率3f
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && Vector3.Distance(transform.position, new Vector3(0, 0, 0)) > 2f)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel") * 5f);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && Vector3.Distance(transform.position, new Vector3(0, 0, 0)) < 50f)
        {
            transform.Translate(Vector3.back * Time.deltaTime * (-Input.GetAxis("Mouse ScrollWheel")) * 5f);
        }
    }
}
