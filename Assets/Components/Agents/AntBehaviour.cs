using System.Collections;
using System.Collections.Generic;
using Antymology.Terrain;
using UnityEngine;

public class AntBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public int health;
    public int health_share_chance;
    public int health_share_with_queen_chance;
    public int queen_health_share_chance;
    public int eatChance;
    public int digChance;
    public int xdimention;
    public int yimention;
    public int zdimention;
    public int ID;
    public bool isqueen;
    public WorldManager wm;
    private System.Random RNG;
    void Start()
    {
        RNG = new System.Random();
        wm = GameObject.FindObjectOfType<WorldManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if (wm.GetBlock((int)transform.position.x, (int)(transform.position.y - 1), (int)transform.position.z) as AcidicBlock != null)
        {
            //Debug.Log("Ant " + ID + " is standing on an acidic block and as thus has recieved double damage");
            health -= 4;
        }
        else {
            health -= 2;
        }
        if (health <= 0)
        {
            Debug.Log("Ant " + ID + " has died");
            wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            if (isqueen)
            {
                int x = (int)(transform.position.x + RNG.Next(-10, 11));
                int y = (int)(transform.position.y + RNG.Next(-10, 11));
                int z = (int)(transform.position.z + RNG.Next(-10, 11));
                if (x >= xdimention || x < 0)
                {
                    x = (int)transform.position.x;
                }
                if (y >= yimention || y < 0)
                {
                    y = (int)transform.position.y;
                }
                if (z >= xdimention || z < 0)
                {
                    z = (int)transform.position.z;
                }
                if (wm.GetBlock(x, y, z) as AirBlock != null && wm.GetBlock(x, y - 1, z) as AirBlock == null)
                {
                    Debug.Log("Ant " + ID + " has moved");
                    wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
                    Vector3 v = new Vector3(x, y, z);
                    transform.position = v;
                    wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Add(this.gameObject);
                }
                int antsInSamePosition = wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Count;
                if (wm.GetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z)) as MulchBlock != null)
                {
                    if (antsInSamePosition == 1)
                    {
                        int rand = RNG.Next(100);
                        if (rand < eatChance)
                        {
                            Debug.Log("Ant " + ID + " has consumed a mulch block");
                            wm.SetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z), ((new AirBlock()) as AbstractBlock));
                            wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
                            Vector3 v = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                            transform.position = v;
                            health += 3333;
                            wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Add(this.gameObject);
                        }
                    }

                }
                else if (wm.GetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z)) as ContainerBlock == null)
                {
                    int rand = RNG.Next(100);
                    if (rand < digChance)
                    {
                        Debug.Log("Ant " + ID + " has dug a block");
                        wm.SetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z), ((new AirBlock()) as AbstractBlock));
                        for (int m = 0; m < wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Count; m++)
                        {
                            GameObject ant = wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)][m];
                            if(ant != null)
                            {
                                AntBehaviour beh = ant.GetComponent<AntBehaviour>();
                                if (beh.ID != ID)
                                {
                                    wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(ant);
                                    Vector3 v2 = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                                    ant.transform.position = v2;
                                    wm.Positions[((int)transform.position.x), ((int)transform.position.y - 1), ((int)transform.position.z)].Add(ant);
                                }
                            }

                        }
                        wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
                        Vector3 v = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                        transform.position = v;
                        wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Add(this.gameObject);

                    }

                }
            }
            else
            {
                int x = (int)(transform.position.x + RNG.Next(-10, 11));
                int y = (int)(transform.position.y + RNG.Next(-10, 11));
                int z = (int)(transform.position.z + RNG.Next(-10, 11));
                if (x >= xdimention || x < 0)
                {
                    x = (int)transform.position.x;
                }
                if (y >= yimention || y < 0)
                {
                    y = (int)transform.position.y;
                }
                if (z >= xdimention || z < 0)
                {
                    z = (int)transform.position.z;
                }
                if (wm.GetBlock(x, y, z) as AirBlock != null && wm.GetBlock(x, y - 1, z) as AirBlock == null)
                {
                    Debug.Log("Ant " + ID + " has moved");
                    wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
                    Vector3 v = new Vector3(x, y, z);
                    transform.position = v;
                    wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Add(this.gameObject);
                }
                int antsInSamePosition = wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Count;
                if (wm.GetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z)) as MulchBlock != null)
                {
                    if (antsInSamePosition == 1)
                    {
                        int rand = RNG.Next(100);
                        if (rand < eatChance)
                        {
                            Debug.Log("Ant " + ID + " has consumed a mulch block");
                            wm.SetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z), ((new AirBlock()) as AbstractBlock));
                            wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
                            Vector3 v = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                            transform.position = v;
                            health += 3333;
                            wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Add(this.gameObject);
                        }
                    }

                }
                else if (wm.GetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z)) as ContainerBlock == null)
                {
                    int rand = RNG.Next(100);
                    if (rand < digChance)
                    {
                        Debug.Log("Ant " + ID + " has dug a block");
                        wm.SetBlock(((int)transform.position.x), ((int)transform.position.y) - 1, ((int)transform.position.z), ((new AirBlock()) as AbstractBlock));
                        for (int m = 0; m < wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Count; m++)
                        {
                            GameObject ant = wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)][m];
                            if(ant != null)
                            {
                                AntBehaviour beh = ant.GetComponent<AntBehaviour>();
                                if (beh.ID != ID)
                                {
                                    wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(ant);
                                    Vector3 v2 = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                                    ant.transform.position = v2;
                                    wm.Positions[((int)transform.position.x), ((int)transform.position.y - 1), ((int)transform.position.z)].Add(ant);
                                }
                            }

                        }
                        wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Remove(this.gameObject);
                        Vector3 v = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                        transform.position = v;
                        wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Add(this.gameObject);

                    }

                }
                antsInSamePosition = wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Count;
                if (antsInSamePosition >= 2)
                {
                    int rand = RNG.Next(100);
                    int m = 0;
                    bool queenFound = false;
                    for (;m < wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)].Count; m++)
                    {
                        GameObject ant = wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)][m];
                        if(ant != null)
                        {
                            AntBehaviour beh = ant.GetComponent<AntBehaviour>();
                            if (beh.isqueen)
                            {
                                queenFound = true;
                                break;
                            }
                        }
 
                    }
                    if (queenFound)
                    {
                        if(rand < health_share_with_queen_chance)
                        {
                            GameObject ant = wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)][m];
                            if(ant != null)
                            {
                                AntBehaviour beh = ant.GetComponent<AntBehaviour>();
                                int sharedHealth = RNG.Next(health);
                                beh.health += sharedHealth;
                                health -= sharedHealth;
                                Debug.Log("Ant " + ID + " has shared health with queen");
                            }

                        }
                    }
                    else
                    {
                        if (rand < health_share_chance)
                        {
                            GameObject ant = wm.Positions[((int)transform.position.x), ((int)transform.position.y), ((int)transform.position.z)][0];
                            if(ant != null)
                            {
                                AntBehaviour beh = ant.GetComponent<AntBehaviour>();
                                int sharedHealth = RNG.Next(health);
                                beh.health += sharedHealth;
                                health -= sharedHealth;
                                Debug.Log("Ant " + ID + " has shared health with Ant " + beh.ID);
                            }
                        }
                    }
                }
            }
        }
    }
}
