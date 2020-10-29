using System.Collections.Generic;
using UnityEngine;



///////////////////////////////////////////////////////////////////////////////////
//
//
///////////////////////////////////////////////////////////////////////////////////
public class Ball : MonoBehaviour
{
	public ManagerGame manager_game;

	public Rigidbody rb;
	Vector3 vel;

	public int list_idx;
	int list_idx_attracted_to;
	int list_idx_repelling_from;



	///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
		vel = new Vector3(0.0f, 0.0f, 0.0f);
    }


	///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    public void setup(ManagerGame manager_game, int list_idx, int list_idx_attracted_to, int list_idx_repelling_from)
    {
		Vector3 pos;

		this.manager_game = manager_game;

		this.list_idx = list_idx;
		this.list_idx_attracted_to = list_idx_attracted_to;
		this.list_idx_repelling_from = list_idx_repelling_from;

		pos.x = -manager_game.getWorldSize()/2.0f + (SiRandom.GetFloat() * manager_game.getWorldSize());

		if (manager_game.getDimension() == ManagerGame.EnumDimension.THREE)
			pos.y = 0.0f + (SiRandom.GetFloat() * manager_game.getWorldSize());
		else
			pos.y = 0.0f;

		pos.z = -manager_game.getWorldSize()/2.0f + (SiRandom.GetFloat() * manager_game.getWorldSize());

		transform.localPosition = pos;

		gameObject.layer = LayerInfo.COLL_BALL_TYPE_0 + list_idx;
	}


	

	///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    public void updateMe()
    {
		int i;
		Vector3 pos_this;
		List<Ball> list_ball_attracted_to;
		List<Ball> list_ball_repelling_from;
		List<Ball> list;
		Ball b;
		float speed;


		pos_this = transform.localPosition;

		list_ball_attracted_to = buildListOfNearbyBalls(pos_this, list_idx_attracted_to);
		list_ball_repelling_from = buildListOfNearbyBalls(pos_this, list_idx_repelling_from);

	
		vel.x = 0.0f;
		vel.y = 0.0f;
		vel.z = 0.0f;


	
		speed = manager_game.getSpeed();


		//move towards those you're attracted to
		list = list_ball_attracted_to;	
		for(i=0;i<list.Count;i++)
		{
			b = list[i];

			if (pos_this.x >= b.transform.localPosition.x)
				vel.x -= speed;
			else if (pos_this.x < b.transform.localPosition.x)
				vel.x += speed;

			if (manager_game.getDimension() == ManagerGame.EnumDimension.THREE)
			{
				if (pos_this.y >= b.transform.localPosition.y)
					vel.y-= speed;
				else if (pos_this.y < b.transform.localPosition.y)
					vel.y += speed;
			}

			if (pos_this.z >= b.transform.localPosition.z)
				vel.z -= speed;
			else if (pos_this.z < b.transform.localPosition.z)
				vel.z += speed;
		}

		//move away
		list = list_ball_repelling_from;	
		for(i=0;i<list.Count;i++)
		{
			b = list[i];

			if (pos_this.x >= b.transform.localPosition.x)
				vel.x += speed;
			else if (pos_this.x < b.transform.localPosition.x)
				vel.x -= speed;

			if (manager_game.getDimension() == ManagerGame.EnumDimension.THREE)
			{
				if (pos_this.y >= b.transform.localPosition.y)
					vel.y += speed;
				else if (pos_this.y < b.transform.localPosition.y)
					vel.y -= speed;
			}

			if (pos_this.z >= b.transform.localPosition.z)
				vel.z += speed;
			else if (pos_this.z < b.transform.localPosition.z)
				vel.z -= speed;
		}

		rb.AddForce(vel);


		processBounds();
		
    }



	///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    public List<Ball> buildListOfNearbyBalls(Vector3 pos_this, int list_idx_other)
    {
		int i;
		Ball b;
		List<Ball> list;
		List<Ball> list_other;
		float dist;


		list = new List<Ball>();
		list_other = manager_game.list_ball[list_idx_other];

		for(i=0; i<list_other.Count; i++)
		{
			b = list_other[i];

			if (b == this)
				continue;

			dist = SiMath.CalcDist(b.transform.localPosition, pos_this);
			if (dist < manager_game.getAttractionDist())
				list.Add(b);
		}

		return list;
	}


	///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    void processBounds()
	{
		Vector3 pos;



		pos = transform.localPosition;

		switch(manager_game.getBorder())
		{
			case ManagerGame.EnumBorder.WRAP:

				if (pos.x > manager_game.getWorldSize())
					pos.x = -manager_game.getWorldSize();
				else if (pos.x < -manager_game.getWorldSize())
					pos.x = manager_game.getWorldSize();

				if (manager_game.getDimension() == ManagerGame.EnumDimension.THREE)
				{
					if (pos.y > manager_game.getWorldSize())
						pos.y = 0.0f;
					else if (pos.y < 0.0f)
						pos.y = manager_game.getWorldSize();
				}
				else
				{
					pos.y = 0.0f;
				}

				if (pos.z > manager_game.getWorldSize())
					pos.z = -manager_game.getWorldSize();
				else if (pos.z < -manager_game.getWorldSize())
					pos.z = manager_game.getWorldSize();

				break;

			case ManagerGame.EnumBorder.LOCK:

				if (pos.x > manager_game.getWorldSize())
					pos.x = manager_game.getWorldSize();
				else if (pos.x < -manager_game.getWorldSize())
					pos.x = -manager_game.getWorldSize();

				if (manager_game.getDimension() == ManagerGame.EnumDimension.THREE)
				{
					if (pos.y > manager_game.getWorldSize())
						pos.y = manager_game.getWorldSize();
					else if (pos.y < 0.0f)
						pos.y = 0.0f;
				}
				else
				{
					pos.y = 0.0f;
				}

				if (pos.z > manager_game.getWorldSize())
					pos.z = manager_game.getWorldSize();
				else if (pos.z < -manager_game.getWorldSize())
					pos.z = -manager_game.getWorldSize();

				break;

			case ManagerGame.EnumBorder.NONE:

				if (manager_game.getDimension() == ManagerGame.EnumDimension.THREE)
				{
					if (pos.y < 0.0f)
						pos.y = 0.0f;
				}
				else
				{
					pos.y = 0.0f;
				}

				break;

			default:
				SiLog.UnsupportedIdx((int)manager_game.getBorder());
				break;

		}


		transform.localPosition = pos;
	}


}













