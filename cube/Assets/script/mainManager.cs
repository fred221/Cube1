using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mainManager : MonoBehaviour {

	enum slideEnum
	{
		nullslide,
		up,
		down,
		left,
		right,
	};
	public  GameObject originCube_;

	private slideEnum currSlide = slideEnum.nullslide;
	private Vector3 centerPoint = new Vector3 (1.0f, 1.0f, 1.0f);

	private float offsetTime = 0.05f ;
	private float offSetAngle = 90.0f;

	private Vector2 currentPos = Vector2.zero; //next pos
	private Vector2 lastPos = Vector3.zero ;
	private float curSetAngle = 0.0f;
	private float times = 0.0f;

	private List<GameObject> cubeDictMap_ = new List<GameObject>();
	private List<GameObject> cubeMoveList = new List<GameObject>();
	private Vector3 cubeMoveDir = Vector3.zero ;

	// Use this for initialization
	void Start () {
		
		CreateCube ();
	}

	void OnGUI()
	{
		if (Event.current.type == EventType.mouseDown)
		{
			times = 0.0f;
			lastPos    = Input.mousePosition;//Event.current.mousePosition;
			currentPos = Input.mousePosition;//Event.current.mousePosition;
		}
		if (Event.current.type == EventType.MouseDrag)
		{
			if (currSlide != slideEnum.nullslide)
			{
				return;
			}
			currentPos = Input.mousePosition;
			times += Time.deltaTime;
			if (times > offsetTime)
			{
				MoveTo ();
				times = 0.0f;
				lastPos = currentPos;
			}
		}

		if(Event.current.type == EventType.MouseUp)
		{
			currSlide = slideEnum.nullslide;
		}
	}
	// Update is called once per frame
	void Update ()
	{
		MoveCube ();
	}
		
	void MoveTo()
	{
		if (cubeMoveList.Count > 0)
		{
			return;
		}

		float absX = Mathf.Abs (currentPos.x - lastPos.x);
		float absY = Mathf.Abs (currentPos.y - lastPos.y);

		if (absX - absY > 0) {
			if (absX < 15)
			{
				return;
			}
			// left
			if (currentPos.x < lastPos.x) {
				if (currSlide == slideEnum.left) {
					return;
				}

				MoveByCube (slideEnum.left);
				currSlide = slideEnum.left;
			}
			//right
			if (currentPos.x > lastPos.x) {
				if (currSlide == slideEnum.right) {
					return;
				}

				MoveByCube (slideEnum.right);
				currSlide = slideEnum.right;
			}
		} 
		else 
		{
			if (absY < 15)
			{
				return;
			}
			//up
			if (currentPos.y > lastPos.y)
			{
				if (currSlide == slideEnum.up) 
				{
					return;
				}

				MoveByCube (slideEnum.up);
				currSlide = slideEnum.up;
			}
			//down
			if (currentPos.y < lastPos.y)
			{
				if (currSlide == slideEnum.up) 
				{
					return;
				}

				MoveByCube (slideEnum.down);
				currSlide = slideEnum.down;
			}
		}
	}
	void CreateCube()
	{
		for (int m = 0; m < 3; ++m) 
		{
			for (int j = 0; j < 3; ++j)
			{
				for (int i = 0; i < 3; ++i)
				{
					Vector3 pos = originCube_.transform.localPosition;
					pos.Set (pos.x + (i * 1), pos.y + (j * 1), pos.z + (m *1));
					GameObject cubeObj = (GameObject)Instantiate (originCube_, pos, originCube_.transform.localRotation);
					cubeObj.name = i.ToString () + j.ToString () + m.ToString ();
					cubeDictMap_.Add(cubeObj);
				}
			}
		}
	}

	void MoveByCube(slideEnum slideType)
	{
		if (slideType == slideEnum.nullslide)
		{
			return;
		}
	
		Ray ray = Camera.main.ScreenPointToRay (lastPos);
		RaycastHit hitRay;
		if (!Physics.Raycast (ray, out hitRay)) 
		{
			return;
		}
		foreach (GameObject cubeObj in cubeDictMap_)
		{
			if (slideType == slideEnum.left) 
			{
				float lastPosWorldY = GetLastPosWorldY ();
				if (lastPosWorldY < 3.2f)
				{
					if (Mathf.Abs(cubeObj.transform.position.y - hitRay.collider.transform.position.y) > 1e-5) 
					{
						continue;
					}
					AddListToMove (cubeObj, Vector3.up);
				} 
				else
				{
					if (currentPos.y > lastPos.y)
					{
						if (Mathf.Abs (cubeObj.transform.position.z - hitRay.collider.transform.position.z) > 1e-5)
						{
							continue;
						}	
						AddListToMove (cubeObj, Vector3.back);
					}
					else 
					{
						if (Mathf.Abs(cubeObj.transform.position.x - hitRay.collider.transform.position.x) > 1e-5)
						{
							continue;
						}
						AddListToMove (cubeObj,Vector3.right);
					}
				}
			}
			if (slideType == slideEnum.right) 
			{
				float lastPosWorldY = GetLastPosWorldY ();
				if (lastPosWorldY < 3.2f)
				{
					if (Mathf.Abs (cubeObj.transform.position.y - hitRay.collider.transform.position.y) > 1e-5) {
						continue;
					}
					AddListToMove (cubeObj, Vector3.down);
				}
				else
				{
					if (currentPos.y < lastPos.y) 
					{
						if (Mathf.Abs (cubeObj.transform.position.z - hitRay.collider.transform.position.z) > 1e-5) {
							continue;
						}	
						AddListToMove (cubeObj, Vector3.forward);
					} 

					else 
					{
						if (Mathf.Abs(cubeObj.transform.position.x - hitRay.collider.transform.position.x) > 1e-5)
						{
							continue;
						}
						AddListToMove (cubeObj,Vector3.left);
					}
				}
			}
			if (slideType == slideEnum.up) 
			{
				// which 6 face of cube
				if (lastPos.x < 315)
				{
					if (Mathf.Abs(cubeObj.transform.position.x - hitRay.collider.transform.position.x) > 1e-5)
					{
						continue;
					}
					AddListToMove (cubeObj,Vector3.left);
				}
				else
				{
					if (Mathf.Abs(cubeObj.transform.position.z - hitRay.collider.transform.position.z) > 1e-5)
					{
						continue;
					}
					AddListToMove (cubeObj, Vector3.back);
				}
			}
			if (slideType == slideEnum.down) 
			{
				// which 6 face of cube
				if (lastPos.x < 315)
				{
					if (Mathf.Abs(cubeObj.transform.position.x - hitRay.collider.transform.position.x) > 1e-5)
					{
						continue;
					}
					AddListToMove (cubeObj,Vector3.right);
				}
				else
				{
					if (Mathf.Abs(cubeObj.transform.position.z - hitRay.collider.transform.position.z) > 1e-5)
					{
						continue;
					}	
					AddListToMove (cubeObj, Vector3.forward);
				}
			}
		}
	}
	float GetLastPosWorldY()
	{
		GameObject mainCameraObj = GameObject.Find("Main Camera"); 
		Vector3 wordCenterPos    = mainCameraObj.GetComponent<Camera>().WorldToScreenPoint(centerPoint);
		Vector3 mousePos         = new Vector3 (lastPos.x, lastPos.y, centerPoint.z);
		Vector3 wordLastPos      = mainCameraObj.GetComponent<Camera>().ScreenToWorldPoint(mousePos);
		return wordLastPos.y;
	}
	void AddListToMove(GameObject cube,Vector3  dir)
	{
		cubeMoveList.Add (cube);
		cubeMoveDir = dir;
	}
	void MoveCube()
	{
		if (cubeMoveList.Count < 9) 
		{
			cubeMoveList.Clear();
			cubeMoveDir = Vector3.zero;
			curSetAngle = 0.0f;
			return;
		}
			
		foreach(GameObject cubeObj in cubeMoveList)
		{
			cubeObj.transform.RotateAround (centerPoint,cubeMoveDir,10);
		}

		curSetAngle += 10;

		if (curSetAngle >= offSetAngle)
		{
			cubeMoveList.Clear();
			cubeMoveDir = Vector3.zero;
			curSetAngle = 0.0f;
		}
	}
}
