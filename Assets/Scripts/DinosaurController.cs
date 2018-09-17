using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DinosaurController : MonoBehaviour {

    public float idleTimer;
    public Eatable WhichFoodDoesItEat;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    private Animator animator;
    private bool wanderRandomly;
    private List<int> foodAlreadyTried;

    private Food food = null;
    private SceneController gameController;
    private GameObject ground;

    void Start () {
        wanderRandomly = true;
        agent = GetComponent<NavMeshAgent>();
        timer = idleTimer;
        animator = GetComponent<Animator>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>();
        foodAlreadyTried = new List<int>();
        ground = GameObject.FindGameObjectWithTag("Ground");
    }
	
	void Update () {
        
        if (wanderRandomly)
        {
            if (!agent.hasPath)
            {
                timer += Time.deltaTime;
                if (timer >= idleTimer)
                {
                    Vector3 newPosition = RandomPosition();
                    agent.SetDestination(newPosition);
                    timer = 0;
                    //set walk animation
                }
                else
                {
                    //set idle animation
                }
            }
        }
        else
        {
            if (!agent.hasPath)
            {
                animator.SetBool("Eat", true);      //no animation yet
                if (food.WhichFoodIsIt == WhichFoodDoesItEat)
                {
                    Destroy(food.gameObject);
                }
                else
                {
                    if (!foodAlreadyTried.Contains(food.GetInstanceID()))
                    {
                        foodAlreadyTried.Add(food.GetInstanceID());
                    }
                }
                animator.SetBool("Eat", false);
                wanderRandomly = true;
            }
        }
    }


    public Vector3 RandomPosition()
    {
        float groundsizeX = ground.transform.localScale.x;
        float groundSizeZ = ground.transform.localScale.z;

        Vector3 randomPoint = new Vector3(Random.Range(-groundsizeX * 5, groundsizeX * 5), 0, Random.Range(-groundSizeZ * 5, groundSizeZ * 5));

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint, out navHit, 2, NavMesh.AllAreas);

        return navHit.position;
    }

    //go to food, check if it's the right type -> eat/go away
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Eatable"))
        {
            if(food == null)
            {
                food = other.gameObject.GetComponent<Food>();
            }

            if (!food.inOtherWorld)
            {
                return;
            }
            wanderRandomly = false;
            agent.SetDestination(other.transform.position);
        }
    }

    //check if food is coming from real world to other world while dinosaurs are in range of food...
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Eatable"))
        {
            if (!wanderRandomly)
            {
                return;
            }

            //test if food was already tried before but not eaten
            if(food != null)
            {
                for (int i = 0; i < foodAlreadyTried.Count; i++)
                {
                    if(food.GetInstanceID() == foodAlreadyTried[i])
                    {
                        return;
                    }
                }
            }

            //if food comes to other world -> trigger eating
            if (food.inOtherWorld)
            {
                OnTriggerEnter(other);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Eatable"))
        {
            if (!food.inOtherWorld)
            {
                return;
            }
            wanderRandomly = true;
            food = null;
        }
    }
}
