using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeShader : MonoBehaviour {

    //高度变化速率
    public float hspeed = 50f;
    //物体高度,归一化
	public float h;
	//物体高度最大值
	public float maxSize = 10;
    //物体高度队列
    public Queue hQ = new Queue();


    // Use this for initialization
    void Start () {
        //初始化物体大小
        transform.transform.localScale = new Vector3(0.48f, 0.48f, 0.48f);
        //StartCoroutine(Timer());
    }
	
	// Update is called once per frame
	void Update () {
		float temp = ((int)(h * maxSize * 100)) / 100f;
		if(temp < transform.localScale.y)
        {
            //缩短
            transform.localScale -= new Vector3(0, hspeed * Time.deltaTime, 0);
        }   
		else if (temp > transform.localScale.y)
        {
            //伸长
            transform.localScale += new Vector3(0, hspeed * Time.deltaTime, 0);
        
        }
        
        //到达临界值让变化禁止
		if(Mathf.Abs(transform.localScale.y - temp) < hspeed * Time.deltaTime)
        {
			transform.localScale = new Vector3(transform.transform.localScale.x, temp, transform.transform.localScale.z);
            if (hQ.Count > 0)
            {
                h = (float)hQ.Dequeue();
            }
        }

        //动态变化颜色
		int size = 16; //1~255
		double MeasureCount = size * h * 2 * 3.141592654 / 60;
		double MeasureCount1 = MeasureCount + 2.094395102;
		double MeasureCount2 = MeasureCount - 2.094395102;
		double MeasureR = (255 * (Mathf.Sin ((float)MeasureCount2) + 0.5)) < 0 ? 0 : ((255 * (Mathf.Sin  ((float)MeasureCount2) + 0.5)) > 255 ? 255 : (255 * (Mathf.Sin  ((float)MeasureCount2) + 0.5)));
		double MeasureG = (255 * (Mathf.Sin ((float)MeasureCount1) + 0.5)) < 0 ? 0 : ((255 * (Mathf.Sin ((float)MeasureCount1) + 0.5)) > 255 ? 255 : (255 * (Mathf.Sin ((float)MeasureCount1) + 0.5)));
		double MeasureB = (255 * (Mathf.Sin ((float)MeasureCount) + 0.5)) < 0 ? 0 : ((255 * (Mathf.Sin ((float)MeasureCount) + 0.5)) > 255 ? 255 : (255 * (Mathf.Sin ((float)MeasureCount) + 0.5)));

		GetComponent<Renderer>().material.color = new Color((float)MeasureR/255, (float)MeasureG/255, (float)MeasureB/255);
    }

    //协程
    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            h = Random.Range(0f, 10f);
            Debug.Log(string.Format("Timer2 is up !!! time=${0}", Time.deltaTime));
        }
    }
}
