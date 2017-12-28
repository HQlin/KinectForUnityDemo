using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeManager : MonoBehaviour {

	public GameObject DepthView;								//深度数据对象
	public GameObject Dropdown;									//下拉列表

	private const int height = 53;								//图像高度
	private const int width = 64;								//图像宽度
	private const float localScaleSize = 0.5f;					//方块大小
	private Transform[,] objs = new Transform[width,height];	//方块数组存储对象
	private DepthSourceView _DepthSourceView;					//获取深度数据

	public float[] spectrumData;								//将音频样本数据传送至samples数组，数组大小必须为2的n次方,最小64，最大8192
	public AudioSource thisAudioSource;							//音乐文件
	private int offsetSize = 2;									//频率间隔大小

	private int x = 32;											//中点横坐标
	private int y = 26;											//中点纵坐标


	// Use this for initialization
	void Start () {
		_DepthSourceView = DepthView.GetComponent<DepthSourceView>();
		CreateCubes();
		//StartCoroutine(Timer());
		spectrumData= new float[64];
		//GetComponent<TestDropdown>().Txt_CurrentNode;
	}
	
	// Update is called once per frame
	void Update () {		
		if (_DepthSourceView.ViewMode == DepthViewMode.MultiSourceReader) {
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					objs [x, y].GetComponent<CubeShader> ().h = 0f;
				}
			}
			thisAudioSource.GetSpectrumData (spectrumData, 0, FFTWindow.BlackmanHarris);

			switch (Dropdown.GetComponent<Dropdown> ().value) 
			{
			case 0:
				{
					//显示正方形频率
					int index = 0;
					for (int i = 0; i < y; i++) {
						float h = GetAvg (spectrumData, index, offsetSize) * 10;
						index = index + offsetSize;
						//左上角
						objs [x - i, y - i].GetComponent<CubeShader> ().h = h;
						//右下角
						objs [x + i, y + i].GetComponent<CubeShader> ().h = h;
						//右上角
						objs [x + i, y - i].GetComponent<CubeShader> ().h = h;
						//左下角
						objs [x - i, y + i].GetComponent<CubeShader> ().h = h;
						for (int j = 0; j < i; j++) {
							//左上角
							objs [x - i, y - j].GetComponent<CubeShader> ().h = h;
							objs [x - j, y - i].GetComponent<CubeShader> ().h = h;
							//右下角
							objs [x + i, y + j].GetComponent<CubeShader> ().h = h;
							objs [x + j, y + i].GetComponent<CubeShader> ().h = h;
							//右上角
							objs [x + i, y - j].GetComponent<CubeShader> ().h = h;
							objs [x + j, y - i].GetComponent<CubeShader> ().h = h;
							//左下角
							objs [x - i, y + j].GetComponent<CubeShader> ().h = h;
							objs [x - j, y + i].GetComponent<CubeShader> ().h = h;
						}
					}
				}
				break;
			case 1:
				{
					//显示金字塔频率
					int index = 0;
					for (int i = 0; i < y; i++) {
						float h = GetAvg (spectrumData, index, offsetSize) * 10;
						index = index + offsetSize;
						try{
							//左上角
							objs [x - i, y - i].GetComponent<CubeShader> ().h = h;
							for (int j = 0; j <= i; j++) {
								objs [x - i - j, y - i + j].GetComponent<CubeShader> ().h = h;
								objs [x - i + j, y - i - j].GetComponent<CubeShader> ().h = h;
							}
						}catch{}

						try{
							//右下角
							objs [x + i, y + i].GetComponent<CubeShader> ().h = h;
							for (int j = 0; j <= i; j++) {
								objs [x + i + j, y + i - j].GetComponent<CubeShader> ().h = h;
								objs [x + i - j, y + i + j].GetComponent<CubeShader> ().h = h;
							}
						}catch{}

						try{
							//右上角
							objs [x + i, y - i].GetComponent<CubeShader> ().h = h;
							for (int j = 0; j <= i; j++) {
								objs [x + i + j, y - i + j].GetComponent<CubeShader> ().h = h;
								objs [x + i - j, y - i - j].GetComponent<CubeShader> ().h = h;
							}
						}catch{}

						try{
							//左下角
							objs [x - i, y + i].GetComponent<CubeShader> ().h = h;
							for (int j = 0; j <= i; j++) {
								objs [x - i - j, y + i - j].GetComponent<CubeShader> ().h = h;
								objs [x - i + j, y + i + j].GetComponent<CubeShader> ().h = h;
							}
						}catch{}									
					}
				}
				break;
			case 2:
				{
					//显示圆形频率，中心散
					int index = 0;
					int r = 1;
					try{
						for (; r < y; r++) {
							float h = GetAvg (spectrumData, index, offsetSize) * 10;
							index = index + offsetSize;
							Bresenham_Circle(x , y , r, h);
						}
					}catch{
						Debug.Log(index);
					}
				}
				break;
			case 3:
				{
					//显示圆形频率，外收
					int index = 0;
					int r = y-1;
					try{
						for (; r > 0; r--) {
							float h = GetAvg (spectrumData, index, offsetSize) * 10;
							index = index + offsetSize;
							Bresenham_Circle(x , y , r, h);
						}
					}catch{
						Debug.Log(index);
					}
				}
				break;
			}
		} 
		else 
		{
			int index = 0;
			float minValue = 0;
			float maxValue = 0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					float h = 1 - _DepthSourceView._Vertices [index].z;
					maxValue = Mathf.Max(maxValue,  _DepthSourceView._Vertices [index].z);
					minValue = Mathf.Min(minValue,  _DepthSourceView._Vertices [index].z);
					index++;
					objs [x, y].GetComponent<CubeShader> ().h = h;
				}
			}
		}

		if (Input.GetButtonDown("Fire2"))
		{
			if(_DepthSourceView.ViewMode == DepthViewMode.MultiSourceReader)
			{
				_DepthSourceView.ViewMode = DepthViewMode.SeparateSourceReaders;
			}
			else
			{
				_DepthSourceView.ViewMode = DepthViewMode.MultiSourceReader;
			}
		}
	}

	//求平均值
	private float GetAvg (float[] spectrumData, int index, int size)
	{
		float sum = 0;
		for (int i = 0; i < size; i++) 
		{
			sum += spectrumData [index + i];
		}
		return sum/size;
	}

	private void CreateCubes()
	{
		/*
		GameObject sphereObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphereObj.transform.parent = transform;
		sphereObj.transform.localPosition = new Vector3(16, 7, -13.5f);
		sphereObj.transform.localScale = new Vector3(7, 7, 7);
		sphereObj.AddComponent<Rigidbody> ();
		sphereObj.GetComponent<Renderer>().material.color = new Color(1f, 0.2f, 0f);
		*/
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				GameObject cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
				cubeObj.name = "[" + x + "," + y + "]";
				cubeObj.transform.parent = transform;
				cubeObj.transform.localPosition = new Vector3(x*localScaleSize, 0, -y*localScaleSize);
				cubeObj.AddComponent<CubeShader> ();
				objs[x, y] = cubeObj.GetComponent<Transform>();
			}
		}
	}

	//协程 随机插入数据
	IEnumerator Timer()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			int index = 0;
			float minValue = 0;
			float maxValue = 0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					float h = 1 - _DepthSourceView._Vertices [index].z;
					maxValue = Mathf.Max(maxValue,  _DepthSourceView._Vertices [index].z);
					minValue = Mathf.Min(minValue,  _DepthSourceView._Vertices [index].z);
					index++;
					objs [x, y].GetComponent<CubeShader> ().h = h;
				}
			}
			//Debug.Log(maxValue);
		}
	}

	//中点画圆法－Bresenham算法
	void Bresenham_Circle(int xc , int yc , int r, float h)
	{
	    int x, y, d;

		x = 0;
		y = r;
		d = 3 - 2 * r;
		CirclePlot(xc , yc , x , y , h);
		while(x < y)
		{
			if(d < 0)
			{
		       d = d + 4 * x + 6;
			}
			else
			{
				d = d + 4 * ( x - y ) + 10;
				y--;
  			}
 			x++;
			CirclePlot(xc , yc , x , y , h);
		}
	}

	//CirclePlot()函数是参照圆的八分对称性完成八个点的位置计算的辅助函数
	void CirclePlot(int xc, int yc, int x, int y, float h)
	{
		objs [x + xc, y + yc].GetComponent<CubeShader> ().h = h;
		objs [y + xc, x + yc].GetComponent<CubeShader> ().h = h;
		objs [-x + xc, y + yc].GetComponent<CubeShader> ().h = h;
		objs [y + xc, -x + yc].GetComponent<CubeShader> ().h = h;
		objs [x + xc, -y + yc].GetComponent<CubeShader> ().h = h;
		objs [-y + xc, x + yc].GetComponent<CubeShader> ().h = h;
		objs [-x + xc, -y + yc].GetComponent<CubeShader> ().h = h;
		objs [-y + xc, -x + yc].GetComponent<CubeShader> ().h = h;
	}
}
