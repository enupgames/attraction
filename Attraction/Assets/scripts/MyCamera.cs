using UnityEngine;



////////////////////////////////////////////////////////////////////////////
//
/////////////////////////////////////////////////////////////////////////////
public class MyCamera : MonoBehaviour
{
	ManagerGame manager_game;
	GameObject obj_to_follow;

	public enum EnumMode
	{
		SPIN,
		TOP,
		MAX,
	}
	EnumMode mode;

	const float SPIN_RATE = 15.0f;
	Vector3 spin_offset;
	Vector3 spin_rot;

	Vector3 top_offset;


	////////////////////////////////////////////////////////////////////////////
	//
	/////////////////////////////////////////////////////////////////////////////
	public void Start()
	{
	}


	////////////////////////////////////////////////////////////////////////////
	//
	/////////////////////////////////////////////////////////////////////////////
	public void setup(ManagerGame manager_game, GameObject obj_to_follow)
	{
		this.manager_game = manager_game;
		this.obj_to_follow = obj_to_follow;

		spin_offset = new Vector3(0.0f, 35.0f, manager_game.getCameraDist());
		spin_rot = new Vector3(0,0,0);

		updateOffsetInfo();
		
		mode = EnumMode.MAX;
		nextMode();
	}


	////////////////////////////////////////////////////////////////////////////
	//
	/////////////////////////////////////////////////////////////////////////////
	void updateOffsetInfo()
	{	
		spin_offset.z = manager_game.getCameraDist();
		top_offset = new Vector3(0.0f, manager_game.getCameraDist(), 0.0f);
	}


	////////////////////////////////////////////////////////////////////////////
	//
	/////////////////////////////////////////////////////////////////////////////
	public void LateUpdate()
	{
		Vector3 pos;
		Vector3 vec;


		if (obj_to_follow == null)
			return;


		updateOffsetInfo();

		switch(mode)
		{
			case EnumMode.SPIN:

				spin_rot.y += SPIN_RATE * Time.deltaTime;

				vec = Quaternion.Euler(spin_rot) * spin_offset;
				pos = obj_to_follow.transform.position + vec;
				transform.position = pos;

				transform.LookAt(obj_to_follow.transform.position);
	
				break;

			case EnumMode.TOP:
				transform.position = top_offset;
				transform.LookAt(obj_to_follow.transform.position);

				break;

			default:
				SiLog.UnsupportedIdx((int)mode);
				break;

		}
			
	}


	////////////////////////////////////////////////////////////////////////////
	//
	/////////////////////////////////////////////////////////////////////////////
	public void nextMode()
	{
		if (obj_to_follow == null)
		{
			SiLog.Error("obj to follow is null");
			return;
		}

		mode++;
		if (mode >= EnumMode.MAX)
			mode = 0;
	}


	////////////////////////////////////////////////////////////////////////////
	//
	/////////////////////////////////////////////////////////////////////////////
	public EnumMode getMode() { return mode; }

}



