using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



///////////////////////////////////////////////////////////////////////////////////
//
//
///////////////////////////////////////////////////////////////////////////////////
public class ManagerGame : MonoBehaviour
{
    public GameObject panel;

    public GameObject plane;

    public GameObject prefab_ball;

    public MyCamera my_camera;
    public Text label_camera;

    const float CAMERA_DIST_MIN = 1.0f;
    const float CAMERA_DIST_MAX = 250.0f;
    float camera_dist;
    public Text label_camera_dist;

    public enum EnumDimension
    {
        TWO,
        THREE,
        MAX,
    }
    EnumDimension dimension;
	public Text label_dimension;

    bool is_coll_active;
    public Text label_coll;

    public enum EnumBorder
    {
        LOCK,
        WRAP,
        NONE,
        MAX,
    }
    EnumBorder border;
	public Text label_border;

    const int NUM_BALL_TYPES_MIN = 2;
    const int NUM_BALL_TYPES_MAX = 7;
    public Color[] list_ball_color = new Color[NUM_BALL_TYPES_MAX];
    int num_ball_types;
	public Text label_num_ball_types;
    
    const int NUM_BALLS_MIN = 1;
    int num_balls;
	public Text label_num_balls;

    public enum EnumAttraction
    {
        LOCAL,
        GLOBAL,
        MAX,
    }
    EnumAttraction attraction;
	public Text label_attraction;


    const float SPEED_MIN = 1.0f;
    const float SPEED_MAX = 1000.0f;
    float speed;
	public Text label_speed;

    public Text label_reset;

    public Text label_hide;


    float world_size;
    
	public List<Ball>[] list_ball;
	
    bool is_active;

    const float INPUT_DELAY_MAX = 0.05f;
    float input_delay;


	///////////////////////////////////////////////////////////////////////////////////
	//
	//
	///////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
		SiRandom.SetSeed(0);

        is_active = false;

        input_delay = 0.0f;
        world_size = 30.0f;

        camera_dist = 70.0f;
        dimension = EnumDimension.THREE;
        is_coll_active = true;
        border = EnumBorder.LOCK;
        num_ball_types = NUM_BALL_TYPES_MIN;
        num_balls = NUM_BALLS_MIN;
        attraction = EnumAttraction.LOCAL;
        speed = 5.0f;

        list_ball = null;
        createBalls();

        my_camera.setup(this, plane);

        updateText();

        is_active = true;
    }




	///////////////////////////////////////////////////////////////////////////////////
	//
	//
	///////////////////////////////////////////////////////////////////////////////////
    void createBalls()
    {
        int i;
        int j;
        Ball b;
        List<Ball> list;
        GameObject gobj;
        int list_idx;
	    int list_idx_attracted_to;
	    int list_idx_repelling_from;


        is_active = false;

        //destroy existing balls
        if (list_ball != null)
        {
            for (i=0;i<list_ball.Length;i++)
            {
                list = list_ball[i];
                while(list.Count > 0)
                {
                    Destroy(list[0].gameObject);
                    list.RemoveAt(0);
                }

                list_ball[i] = null;
            }
        }

        //create new balls
        list_ball = new List<Ball>[num_ball_types];

        for (i=0;i<list_ball.Length;i++)
        {
            list_ball[i] = new List<Ball>();
            list = list_ball[i];

            list_idx = i;
            
            if (list_ball.Length == 2)
            {
                if (i == 0)
                {
                    list_idx_attracted_to = 1;
                    list_idx_repelling_from = 0;
                }
                else
                {
                    list_idx_attracted_to = 0;
                    list_idx_repelling_from = 1;
                }
            }
            else
            {
                list_idx_attracted_to = SiRandom.GetInt(num_ball_types);
                if (list_idx_attracted_to == i)
                    list_idx_attracted_to = 0;

                list_idx_repelling_from = SiRandom.GetInt(num_ball_types);
                if (list_idx_repelling_from == i)
                    list_idx_repelling_from = 0;

            }

            for(j=0;j<num_balls;j++)
            {                
                gobj = Instantiate(prefab_ball);
                gobj.GetComponent<MeshRenderer>().material.color = list_ball_color[i];
            
                b = gobj.GetComponent<Ball>();

                b.setup(this, list_idx, list_idx_attracted_to, list_idx_repelling_from);
                b.transform.SetParent(transform);
                list.Add(b);
            }
        }

        is_active = true;

    }



    ///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    public void Update()
    {

        int i;
        int j;


        if (!is_active)
            return;

        //camera
        if (Input.GetKeyUp("n"))
        {
            my_camera.nextMode();
            updateText();
        }

        //dimensions
        if (Input.GetKeyUp("d"))
        {
            dimension++;
            if (dimension >= EnumDimension.MAX)
                dimension = 0;

            updateText();
        }


        //collisions
        if (Input.GetKeyUp("c"))
        {
            is_coll_active = !is_coll_active;

            for(i=LayerInfo.COLL_BALL_TYPE_0; i<=LayerInfo.COLL_BALL_TYPE_6; i++)
            {
                for(j=LayerInfo.COLL_BALL_TYPE_0; j<=LayerInfo.COLL_BALL_TYPE_6; j++)
                   Physics.IgnoreLayerCollision(i, j, !is_coll_active);
            }
            updateText();
        }


        //border
        if (Input.GetKeyUp("b"))
        {
            border++;
            if (border >= EnumBorder.MAX)
                border = 0;

            updateText();
        }


        //num ball types
        if (Input.GetKeyUp("a"))
        {
            num_ball_types -= 1;
            if (num_ball_types < NUM_BALL_TYPES_MIN)
                num_ball_types = NUM_BALL_TYPES_MIN;

            createBalls();
            updateText();
        }
        else if (Input.GetKeyUp("s"))   
        {
            num_ball_types += 1;
            if (num_ball_types > NUM_BALL_TYPES_MAX)
                num_ball_types = NUM_BALL_TYPES_MAX;

            createBalls();
            updateText();
        }


        //num balls (per side)
        if (Input.GetKeyUp("f"))
        {
            num_balls -= NUM_BALLS_MIN;
            if (num_balls < NUM_BALLS_MIN)
                num_balls = NUM_BALLS_MIN;

            createBalls();
            updateText();
        }
        else if (Input.GetKeyUp("g"))    
        {
            num_balls++;
            createBalls();
            updateText();
        }


        //attraction
        if (Input.GetKeyUp("k"))
        {
            attraction++;
            if (attraction >= EnumAttraction.MAX)
                attraction = 0;
            updateText();
        }


        input_delay -= Time.deltaTime;
        if (input_delay < 0.0f)
        {
            //camera dist
            if (Input.GetKey("w"))
            {
                camera_dist -= 1.0f;
                if (camera_dist < CAMERA_DIST_MIN)
                    camera_dist = CAMERA_DIST_MIN;

                updateText();
                input_delay = INPUT_DELAY_MAX;
            }
            else if (Input.GetKey("e"))
            {
                camera_dist += 1.0f;
                if (camera_dist > CAMERA_DIST_MAX)
                    camera_dist = CAMERA_DIST_MAX;

                updateText();
                input_delay = INPUT_DELAY_MAX;
            }


            //speed
            if (Input.GetKey("o"))
            {
                speed -= 1.0f;
                if (speed < SPEED_MIN)
                    speed = SPEED_MIN;

                updateText();
                input_delay = INPUT_DELAY_MAX;
            }
            else if (Input.GetKey("p"))
            {
                speed += 1.0f;
                if (speed > SPEED_MAX)
                    speed = SPEED_MAX;

                updateText();
                input_delay = INPUT_DELAY_MAX;
            }

        }


        //reset
        if (Input.GetKeyUp("r"))
        {
            createBalls();
            updateText();
        }


        //hide/show
        if (Input.GetKeyUp("h"))
        {
            if (panel.activeSelf)
                panel.SetActive(false);
            else
                panel.SetActive(true);

            updateText();
        }

    }



	///////////////////////////////////////////////////////////////////////////////////
	//
	//
	///////////////////////////////////////////////////////////////////////////////////
    void FixedUpdate()
    {
        int i;
        int j;
        List<Ball> list;


        if (!is_active)
            return;

        if (list_ball != null)
        {
            for (i=0;i<list_ball.Length;i++)
            {
                list = list_ball[i];

                for(j=0;j<list.Count;j++)
                    list[j].updateMe();
            }
        }
    }




	///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    void updateText()
    {

        label_camera.text = "" + my_camera.getMode();

        label_camera_dist.text = camera_dist.ToString();

        if (dimension == EnumDimension.TWO)
           label_dimension.text = "2d";
        else
           label_dimension.text = "3d";

        label_coll.text = is_coll_active.ToString();

        label_border.text = "" + border;

        label_num_ball_types.text = num_ball_types.ToString();
        
        label_num_balls.text = num_balls.ToString();

        label_attraction.text = "" + attraction;

        label_speed.text = speed.ToString();

        label_reset.text = "";
        label_hide.text = "";

    }


	///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    public float getAttractionDist()
    {
        if (attraction == EnumAttraction.LOCAL)
            return 650.0f; 
        else
            return 99999999.0f;
    }


    ///////////////////////////////////////////////////////////////////////////////////
	//
	///////////////////////////////////////////////////////////////////////////////////
    public float getWorldSize() { return world_size; }
    public EnumDimension getDimension() { return dimension; }
    public EnumBorder getBorder() { return border; }
    public float getSpeed() { return speed; }
    public float getSpeedMax() { return SPEED_MAX; }
    public float getCameraDist() { return camera_dist; }
 

}