using Antymology.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antymology.Terrain
{
    public class WorldManager : Singleton<WorldManager>
    {

        #region Fields

        /// <summary>
        /// The prefab containing the ant.
        /// </summary>
        public GameObject antPrefab;

        /// <summary>
        /// The material used for eech block.
        /// </summary>
        public Material blockMaterial;

        //number of ants at the start
        public int startingAnts;


        /// <summary>
        /// The raw data of the underlying world structure.
        /// </summary>
        private AbstractBlock[,,] Blocks;

        /// <summary>
        /// Reference to the geometry data of the chunks.
        /// </summary>
        private Chunk[,,] Chunks;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private System.Random RNG;

        public int AliveAnts;

        public int NestBlocks;

        private int Iteration;

        private int individual;

        public bool Active;
        public int MutationChance;
        public int IndividualTrainingTimeinSeconds;
        // number of generations per training round
        public int generationsPerRound;
        public int genrations;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private SimplexNoise SimplexNoise;
        //refrence to an array of ant populations
        public List<GameObject>[,,] Positions;

        public enum views
        {
            MainMenu,
            Train,
            CurrentPopulation,
        }
        public views view;

        #endregion

        #region Initialization

        /// <summary>
        /// Awake is called before any start method is called.
        /// </summary>
        void Awake()
        {
            // Generate new random number generator
            RNG = new System.Random(ConfigurationManager.Instance.Seed);

            // Generate new simplex noise generator
            SimplexNoise = new SimplexNoise(ConfigurationManager.Instance.Seed);

            // Initialize a new 3D array of blocks with size of the number of chunks times the size of each chunk
            Blocks = new AbstractBlock[
                ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter,
                ConfigurationManager.Instance.World_Height * ConfigurationManager.Instance.Chunk_Diameter,
                ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter];

            // Initialize a new 3D array of chunks with size of the number of chunks
            Chunks = new Chunk[
                ConfigurationManager.Instance.World_Diameter,
                ConfigurationManager.Instance.World_Height,
                ConfigurationManager.Instance.World_Diameter];

             Active = true;
        }

        /// <summary>
        /// Called after every awake has been called.
        /// </summary>
        private void Start()
        {
            NestBlocks = 0;
            AliveAnts = 1000;
            view = views.MainMenu;
        }

        /// <summary>
        /// TO BE IMPLEMENTED BY YOU
        /// </summary>
        private SaveFile GenerateAnts()
        {
            Positions = new List<GameObject>[Blocks.GetLength(0), Blocks.GetLength(1), Blocks.GetLength(2)];
            for(int k = 0;k < Blocks.GetLength(0); k++)
            {
                for(int m = 0; m < Blocks.GetLength(1); m++)
                {
                    for(int n = 0; n < Blocks.GetLength(2); n++)
                    {
                        Positions[k, m, n] = new List<GameObject>();
                    }
                }
            }
            int health = 9999;
            int health_share_chance = RNG.Next(100); ;
            int health_share_with_queen_chance =  RNG.Next(100); ;
            int queen_health_share_chance =  RNG.Next(100); ;
            int eatChance = RNG.Next(100); 
            int digChance = RNG.Next(100);
            int createNestChance = RNG.Next(100);
            int xd = Blocks.GetLength(0);
            int yd = Blocks.GetLength(1);
            int zd = Blocks.GetLength(2);
            for (int i = 0; i < startingAnts; i++) {
                int x = RNG.Next(Blocks.GetLength(0));
                int z = RNG.Next(Blocks.GetLength(2));
                GameObject ant = (GameObject)Instantiate(antPrefab);
                for (int y = Blocks.GetLength(1)-1; y >= 0; y--) {
                    if (Blocks[x, y, z] as AirBlock != null) { 
                        ant.transform.position = new Vector3(x, y, z);
                        AntBehaviour a = ant.GetComponent<AntBehaviour>();
                        Positions[x, y, z].Add(ant);
                        if (i == 0) {
                            MeshRenderer me = ant.GetComponent<MeshRenderer>();
                            me.material = Resources.Load("Queen", typeof(Material)) as Material;
                            a.isqueen = true;

                        }
                        a.ID = i;
                        a.wm = this;
                        a.health = health;
                        a.health_share_chance = health_share_chance;
                        a.queen_health_share_chance = queen_health_share_chance;
                        a.eatChance = eatChance;
                        a.digChance = digChance;
                        a.xdimention = xd;
                        a.yimention = yd;
                        a.zdimention = zd;
                        a.createNestChance = createNestChance;

                    } 
                }


            }
            SaveFile s = new SaveFile();
            s.health_share_with_queen_chance = health_share_with_queen_chance;
            s.health_share_chance = health_share_chance;
            s.queen_health_share_chance = queen_health_share_chance;
            s.eatChance = eatChance;
            s.digChance = digChance;
            s.createNestChance = createNestChance;
            return s;
        }

        #endregion
        #region
        /// <summary>
        /// Retrieves an abstract block type at the desired world coordinates.
        /// </summary>
        public AbstractBlock GetBlock(int WorldXCoordinate, int WorldYCoordinate, int WorldZCoordinate)
        {
            if
            (
                WorldXCoordinate < 0 ||
                WorldYCoordinate < 0 ||
                WorldZCoordinate < 0 ||
                WorldXCoordinate >= Blocks.GetLength(0) ||
                WorldYCoordinate >= Blocks.GetLength(1) ||
                WorldZCoordinate >= Blocks.GetLength(2)
            )
                return new AirBlock();

            return Blocks[WorldXCoordinate, WorldYCoordinate, WorldZCoordinate];
        }

        /// <summary>
        /// Retrieves an abstract block type at the desired local coordinates within a chunk.
        /// </summary>
        public AbstractBlock GetBlock(
            int ChunkXCoordinate, int ChunkYCoordinate, int ChunkZCoordinate,
            int LocalXCoordinate, int LocalYCoordinate, int LocalZCoordinate)
        {
            if
            (
                LocalXCoordinate < 0 ||
                LocalYCoordinate < 0 ||
                LocalZCoordinate < 0 ||
                LocalXCoordinate >= Blocks.GetLength(0) ||
                LocalYCoordinate >= Blocks.GetLength(1) ||
                LocalZCoordinate >= Blocks.GetLength(2) ||
                ChunkXCoordinate < 0 ||
                ChunkYCoordinate < 0 ||
                ChunkZCoordinate < 0 ||
                ChunkXCoordinate >= Blocks.GetLength(0) ||
                ChunkYCoordinate >= Blocks.GetLength(1) ||
                ChunkZCoordinate >= Blocks.GetLength(2) 
            )
                return new AirBlock();

            return Blocks
            [
                ChunkXCoordinate * LocalXCoordinate,
                ChunkYCoordinate * LocalYCoordinate,
                ChunkZCoordinate * LocalZCoordinate
            ];
        }

        /// <summary>
        /// sets an abstract block type at the desired world coordinates.
        /// </summary>
        public void SetBlock(int WorldXCoordinate, int WorldYCoordinate, int WorldZCoordinate, AbstractBlock toSet)
        {
            if
            (
                WorldXCoordinate < 0 ||
                WorldYCoordinate < 0 ||
                WorldZCoordinate < 0 ||
                WorldXCoordinate > Blocks.GetLength(0) ||
                WorldYCoordinate > Blocks.GetLength(1) ||
                WorldZCoordinate > Blocks.GetLength(2)
            )
            {
                Debug.Log("Attempted to set a block which didn't exist");
                return;
            }

            Blocks[WorldXCoordinate, WorldYCoordinate, WorldZCoordinate] = toSet;

            SetChunkContainingBlockToUpdate
            (
                WorldXCoordinate,
                WorldYCoordinate,
                WorldZCoordinate
            );
        }

        /// <summary>
        /// sets an abstract block type at the desired local coordinates within a chunk.
        /// </summary>
        public void SetBlock(
            int ChunkXCoordinate, int ChunkYCoordinate, int ChunkZCoordinate,
            int LocalXCoordinate, int LocalYCoordinate, int LocalZCoordinate,
            AbstractBlock toSet)
        {
            if
            (
                LocalXCoordinate < 0 ||
                LocalYCoordinate < 0 ||
                LocalZCoordinate < 0 ||
                LocalXCoordinate > Blocks.GetLength(0) ||
                LocalYCoordinate > Blocks.GetLength(1) ||
                LocalZCoordinate > Blocks.GetLength(2) ||
                ChunkXCoordinate < 0 ||
                ChunkYCoordinate < 0 ||
                ChunkZCoordinate < 0 ||
                ChunkXCoordinate > Blocks.GetLength(0) ||
                ChunkYCoordinate > Blocks.GetLength(1) ||
                ChunkZCoordinate > Blocks.GetLength(2)
            )
            {
                Debug.Log("Attempted to set a block which didn't exist");
                return;
            }
            Blocks
            [
                ChunkXCoordinate * LocalXCoordinate,
                ChunkYCoordinate * LocalYCoordinate,
                ChunkZCoordinate * LocalZCoordinate
            ] = toSet;

            SetChunkContainingBlockToUpdate
            (
                ChunkXCoordinate * LocalXCoordinate,
                ChunkYCoordinate * LocalYCoordinate,
                ChunkZCoordinate * LocalZCoordinate
            );
        }

        #endregion

        #region Helpers

        #region Blocks

        /// <summary>
        /// Is responsible for generating the base, acid, and spheres.
        /// </summary>
        private void GenerateData()
        {
            GeneratePreliminaryWorld();
            GenerateAcidicRegions();
            GenerateSphericalContainers();
        }

        /// <summary>
        /// Generates the preliminary world data based on perlin noise.
        /// </summary>
        private void GeneratePreliminaryWorld()
        {
            for (int x = 0; x < Blocks.GetLength(0); x++)
                for (int z = 0; z < Blocks.GetLength(2); z++)
                {
                    /**
                     * These numbers have been fine-tuned and tweaked through trial and error.
                     * Altering these numbers may produce weird looking worlds.
                     **/
                    int stoneCeiling = SimplexNoise.GetPerlinNoise(x, 0, z, 10, 3, 1.2) +
                                       SimplexNoise.GetPerlinNoise(x, 300, z, 20, 4, 0) +
                                       10;
                    int grassHeight = SimplexNoise.GetPerlinNoise(x, 100, z, 30, 10, 0);
                    int foodHeight = SimplexNoise.GetPerlinNoise(x, 200, z, 20, 5, 1.5);

                    for (int y = 0; y < Blocks.GetLength(1); y++)
                    {
                        if (y <= stoneCeiling)
                        {
                            Blocks[x, y, z] = new StoneBlock();
                        }
                        else if (y <= stoneCeiling + grassHeight)
                        {
                            Blocks[x, y, z] = new GrassBlock();
                        }
                        else if (y <= stoneCeiling + grassHeight + foodHeight)
                        {
                            Blocks[x, y, z] = new MulchBlock();
                        }
                        else
                        {
                            Blocks[x, y, z] = new AirBlock();
                        }
                        if
                        (
                            x == 0 ||
                            x >= Blocks.GetLength(0) - 1 ||
                            z == 0 ||
                            z >= Blocks.GetLength(2) - 1 ||
                            y == 0
                        )
                            Blocks[x, y, z] = new ContainerBlock();
                    }
                }
        }

        /// <summary>
        /// Alters a pre-generated map so that acid blocks exist.
        /// </summary>
        private void GenerateAcidicRegions()
        {
            for (int i = 0; i < ConfigurationManager.Instance.Number_Of_Acidic_Regions; i++)
            {
                int xCoord = RNG.Next(0, Blocks.GetLength(0));
                int zCoord = RNG.Next(0, Blocks.GetLength(2));
                int yCoord = -1;
                for (int j = Blocks.GetLength(1) - 1; j >= 0; j--)
                {
                    if (Blocks[xCoord, j, zCoord] as AirBlock == null)
                    {
                        yCoord = j;
                        break;
                    }
                }

                //Generate a sphere around this point overriding non-air blocks
                for (int HX = xCoord - ConfigurationManager.Instance.Acidic_Region_Radius; HX < xCoord + ConfigurationManager.Instance.Acidic_Region_Radius; HX++)
                {
                    for (int HZ = zCoord - ConfigurationManager.Instance.Acidic_Region_Radius; HZ < zCoord + ConfigurationManager.Instance.Acidic_Region_Radius; HZ++)
                    {
                        for (int HY = yCoord - ConfigurationManager.Instance.Acidic_Region_Radius; HY < yCoord + ConfigurationManager.Instance.Acidic_Region_Radius; HY++)
                        {
                            float xSquare = (xCoord - HX) * (xCoord - HX);
                            float ySquare = (yCoord - HY) * (yCoord - HY);
                            float zSquare = (zCoord - HZ) * (zCoord - HZ);
                            float Dist = Mathf.Sqrt(xSquare + ySquare + zSquare);
                            if (Dist <= ConfigurationManager.Instance.Acidic_Region_Radius)
                            {
                                int CX, CY, CZ;
                                CX = Mathf.Clamp(HX, 1, Blocks.GetLength(0) - 2);
                                CZ = Mathf.Clamp(HZ, 1, Blocks.GetLength(2) - 2);
                                CY = Mathf.Clamp(HY, 1, Blocks.GetLength(1) - 2);
                                if (Blocks[CX, CY, CZ] as AirBlock != null)
                                    Blocks[CX, CY, CZ] = new AcidicBlock();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Alters a pre-generated map so that obstructions exist within the map.
        /// </summary>
        private void GenerateSphericalContainers()
        {

            //Generate hazards
            for (int i = 0; i < ConfigurationManager.Instance.Number_Of_Conatiner_Spheres; i++)
            {
                int xCoord = RNG.Next(0, Blocks.GetLength(0));
                int zCoord = RNG.Next(0, Blocks.GetLength(2));
                int yCoord = RNG.Next(0, Blocks.GetLength(1));


                //Generate a sphere around this point overriding non-air blocks
                for (int HX = xCoord - ConfigurationManager.Instance.Conatiner_Sphere_Radius; HX < xCoord + ConfigurationManager.Instance.Conatiner_Sphere_Radius; HX++)
                {
                    for (int HZ = zCoord - ConfigurationManager.Instance.Conatiner_Sphere_Radius; HZ < zCoord + ConfigurationManager.Instance.Conatiner_Sphere_Radius; HZ++)
                    {
                        for (int HY = yCoord - ConfigurationManager.Instance.Conatiner_Sphere_Radius; HY < yCoord + ConfigurationManager.Instance.Conatiner_Sphere_Radius; HY++)
                        {
                            float xSquare = (xCoord - HX) * (xCoord - HX);
                            float ySquare = (yCoord - HY) * (yCoord - HY);
                            float zSquare = (zCoord - HZ) * (zCoord - HZ);
                            float Dist = Mathf.Sqrt(xSquare + ySquare + zSquare);
                            if (Dist <= ConfigurationManager.Instance.Conatiner_Sphere_Radius)
                            {
                                int CX, CY, CZ;
                                CX = Mathf.Clamp(HX, 1, Blocks.GetLength(0) - 2);
                                CZ = Mathf.Clamp(HZ, 1, Blocks.GetLength(2) - 2);
                                CY = Mathf.Clamp(HY, 1, Blocks.GetLength(1) - 2);
                                Blocks[CX, CY, CZ] = new ContainerBlock();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given a world coordinate, tells the chunk holding that coordinate to update.
        /// Also tells all 4 neighbours to update (as an altered block might exist on the
        /// edge of a chunk).
        /// </summary>
        /// <param name="worldXCoordinate"></param>
        /// <param name="worldYCoordinate"></param>
        /// <param name="worldZCoordinate"></param>
        private void SetChunkContainingBlockToUpdate(int worldXCoordinate, int worldYCoordinate, int worldZCoordinate)
        {
            //Updates the chunk containing this block
            int updateX = Mathf.FloorToInt(worldXCoordinate / ConfigurationManager.Instance.Chunk_Diameter);
            int updateY = Mathf.FloorToInt(worldYCoordinate / ConfigurationManager.Instance.Chunk_Diameter);
            int updateZ = Mathf.FloorToInt(worldZCoordinate / ConfigurationManager.Instance.Chunk_Diameter);
            Chunks[updateX, updateY, updateZ].updateNeeded = true;
            
            // Also flag all 6 neighbours for update as well
            if(updateX - 1 >= 0)
                Chunks[updateX - 1, updateY, updateZ].updateNeeded = true;
            if (updateX + 1 < Chunks.GetLength(0))
                Chunks[updateX + 1, updateY, updateZ].updateNeeded = true;

            if (updateY - 1 >= 0)
                Chunks[updateX, updateY - 1, updateZ].updateNeeded = true;
            if (updateY + 1 < Chunks.GetLength(1))
                Chunks[updateX, updateY + 1, updateZ].updateNeeded = true;

            if (updateZ - 1 >= 0)
                Chunks[updateX, updateY, updateZ - 1].updateNeeded = true;
            if (updateZ + 1 < Chunks.GetLength(2))
                Chunks[updateX, updateY, updateZ + 1].updateNeeded = true;
        }

        #endregion

        #region Chunks

        /// <summary>
        /// Takes the world data and generates the associated chunk objects.
        /// </summary>
        private void GenerateChunks()
        {
            GameObject chunkObg = new GameObject("Chunks");

            for (int x = 0; x < Chunks.GetLength(0); x++)
                for (int z = 0; z < Chunks.GetLength(2); z++)
                    for (int y = 0; y < Chunks.GetLength(1); y++)
                    {
                        GameObject temp = new GameObject();
                        temp.transform.parent = chunkObg.transform;
                        temp.transform.position = new Vector3
                        (
                            x * ConfigurationManager.Instance.Chunk_Diameter - 0.5f,
                            y * ConfigurationManager.Instance.Chunk_Diameter + 0.5f,
                            z * ConfigurationManager.Instance.Chunk_Diameter - 0.5f
                        );
                        Chunk chunkScript = temp.AddComponent<Chunk>();
                        chunkScript.x = x * ConfigurationManager.Instance.Chunk_Diameter;
                        chunkScript.y = y * ConfigurationManager.Instance.Chunk_Diameter;
                        chunkScript.z = z * ConfigurationManager.Instance.Chunk_Diameter;
                        chunkScript.Init(blockMaterial);
                        chunkScript.GenerateMesh();
                        Chunks[x, y, z] = chunkScript;
                    }
        }
        public void clearWorld()
        {
            for(int x = 0;x< Positions.GetLength(0); x++)
            {
                for(int y = 0; y < Positions.GetLength(1); y++)
                {
                    for(int z = 0;z < Positions.GetLength(2); z++)
                    {
                        Blocks[x, y, z] = null;
                        for(int k = 0; k < Positions[x,y,z].Count;k++)
                        if(Positions[x,y,z] != null)
                        {
                            Destroy(Positions[x, y, z][k]);
                            Positions[x, y, z][k] = null;
                        }
                    }
                }
            }
            Blocks = null;
            Positions = null;
            for (int x = 0; x < Chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Chunks.GetLength(1); y++)
                {
                    for (int z = 0; z < Chunks.GetLength(2); z++)
                    {
                        Destroy(Chunks[x, y, z]);
                        Chunks[x, y, z] = null;
                    }
                }
            }
            Chunks = null;
            Destroy(GameObject.Find("Chunks"));

        }
        SaveFile GenerateAntsWithSpecifiedStats(int health_share_chance, int health_share_with_queen_chance, int queen_health_share_chance, int eatChance, int digChance, int createNestChance)
        {
            Positions = new List<GameObject>[Blocks.GetLength(0), Blocks.GetLength(1), Blocks.GetLength(2)];
            for (int k = 0; k < Blocks.GetLength(0); k++)
            {
                for (int m = 0; m < Blocks.GetLength(1); m++)
                {
                    for (int n = 0; n < Blocks.GetLength(2); n++)
                    {
                        Positions[k, m, n] = new List<GameObject>();
                    }
                }
            }
            int health = 9999; 
            int xd = Blocks.GetLength(0);
            int yd = Blocks.GetLength(1);
            int zd = Blocks.GetLength(2);
            for (int i = 0; i < startingAnts; i++)
            {
                int x = RNG.Next(Blocks.GetLength(0));
                int z = RNG.Next(Blocks.GetLength(2));
                GameObject ant = (GameObject)Instantiate(antPrefab);
                for (int y = Blocks.GetLength(1) - 1; y >= 0; y--)
                {
                    if (Blocks[x, y, z] as AirBlock != null)
                    {
                        ant.transform.position = new Vector3(x, y, z);
                        AntBehaviour a = ant.GetComponent<AntBehaviour>();
                        Positions[x, y, z].Add(ant);
                        if (i == 0)
                        {
                            MeshRenderer me = ant.GetComponent<MeshRenderer>();
                            me.material = Resources.Load("Queen", typeof(Material)) as Material;
                            a.isqueen = true;

                        }
                        a.ID = i;
                        a.wm = this;
                        a.health = health;
                        a.health_share_chance = health_share_chance;
                        a.queen_health_share_chance = queen_health_share_chance;
                        a.eatChance = eatChance;
                        a.digChance = digChance;
                        a.xdimention = xd;
                        a.yimention = yd;
                        a.zdimention = zd;
                        a.createNestChance = createNestChance;

                    }
                }


            }
            SaveFile s = new SaveFile();
            s.health_share_with_queen_chance = health_share_with_queen_chance;
            s.health_share_chance = health_share_chance;
            s.queen_health_share_chance = queen_health_share_chance;
            s.eatChance = eatChance;
            s.digChance = digChance;
            s.createNestChance = createNestChance;
            return s;

        }
        public SaveFile generateAntsFromParents(SaveFile parent1,SaveFile parent2)
        {
            Positions = new List<GameObject>[Blocks.GetLength(0), Blocks.GetLength(1), Blocks.GetLength(2)];
            for (int k = 0; k < Blocks.GetLength(0); k++)
            {
                for (int m = 0; m < Blocks.GetLength(1); m++)
                {
                    for (int n = 0; n < Blocks.GetLength(2); n++)
                    {
                        Positions[k, m, n] = new List<GameObject>();
                    }
                }
            }
            int health = 9999;
            int health_share_chance = CrossOverAndMutate(parent1.health_share_chance, parent2.health_share_chance);
            int health_share_with_queen_chance = CrossOverAndMutate(parent1.health_share_with_queen_chance, parent2.health_share_with_queen_chance);
            int queen_health_share_chance = CrossOverAndMutate(parent1.queen_health_share_chance, parent2.queen_health_share_chance);
            int eatChance = CrossOverAndMutate(parent1.eatChance, parent2.eatChance);
            int digChance = CrossOverAndMutate(parent1.digChance, parent2.digChance);
            int createNestChance = CrossOverAndMutate(parent1.createNestChance, parent2.createNestChance);
            int xd = Blocks.GetLength(0);
            int yd = Blocks.GetLength(1);
            int zd = Blocks.GetLength(2);
            for (int i = 0; i < startingAnts; i++)
            {
                int x = RNG.Next(Blocks.GetLength(0));
                int z = RNG.Next(Blocks.GetLength(2));
                GameObject ant = (GameObject)Instantiate(antPrefab);
                for (int y = Blocks.GetLength(1) - 1; y >= 0; y--)
                {
                    if (Blocks[x, y, z] as AirBlock != null)
                    {
                        ant.transform.position = new Vector3(x, y, z);
                        AntBehaviour a = ant.GetComponent<AntBehaviour>();
                        Positions[x, y, z].Add(ant);
                        if (i == 0)
                        {
                            MeshRenderer me = ant.GetComponent<MeshRenderer>();
                            me.material = Resources.Load("Queen", typeof(Material)) as Material;
                            a.isqueen = true;

                        }
                        a.ID = i;
                        a.wm = this;
                        a.health = health;
                        a.health_share_chance = health_share_chance;
                        a.queen_health_share_chance = queen_health_share_chance;
                        a.eatChance = eatChance;
                        a.digChance = digChance;
                        a.xdimention = xd;
                        a.yimention = yd;
                        a.zdimention = zd;
                        a.createNestChance = createNestChance;

                    }
                }


            }
            return GenerateAntsWithSpecifiedStats(health_share_chance, health_share_with_queen_chance, queen_health_share_chance, eatChance, digChance, createNestChance);
        }

        private int CrossOverAndMutate(int gene1, int gene2)
        {
            int rand = RNG.Next(2);
            int val;
            if(rand == 0)
            {
                val = gene1;
            }
            {
                val = gene2;
            }
            rand = RNG.Next(100);
            if(rand < MutationChance)
            {
                int mutation = RNG.Next(-10, 11);
                val += mutation;
                val = Math.Max(val, 0);
                return val;
            }
            else
            {
                return val;
            }
        }

        class Comparer : IComparer
        {

            public int Compare(object x, object y)
            {
                SaveFile m = x as SaveFile;
                SaveFile n = y as SaveFile;
                if (m.NestSize >= n.NestSize)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }
        IEnumerator Train()
        {
            SaveFile[] saves = new SaveFile[generationsPerRound];
            Debug.Log("Training started");
            for (int i = 0; i < genrations; i++)
            {
                Debug.Log(" Training Genereation " + i);
                Iteration = i;
                //first Generation is random
                if (i == 0)
                {             
                    for (int x = 0; x < generationsPerRound; x++)
                    {
                        Debug.Log(" Training Individual" + x);
                        Active = true;
                        individual = x;
                        NestBlocks = 0;
                        AliveAnts = 1000;
                        Awake();
                        GenerateData();
                        GenerateChunks();
                        Camera.main.transform.position = new Vector3(0 / 2, Blocks.GetLength(1), 0);
                        Camera.main.transform.LookAt(new Vector3(Blocks.GetLength(0), 0, Blocks.GetLength(2)));
                        saves[x] = GenerateAnts();
                        yield return new WaitForSeconds(IndividualTrainingTimeinSeconds);
                        saves[x].NestSize = NestBlocks;
                        Debug.Log("NestSize for individual " + x + " is " + saves[x].NestSize);
                        Active = false;
                        try
                        {
                            clearWorld();
                        }
                        catch
                        {

                        }

                    }
                    Array.Sort(saves,new Comparer());
                    SaveFile.Save(saves[0].health_share_with_queen_chance, saves[0].queen_health_share_chance, saves[0].eatChance, saves[0].digChance, saves[0].NestSize, saves[0].createNestChance, saves[0].health_share_chance);
                    Debug.Log("Best NestSize for generation " + i + " is " + saves[0].NestSize);
                }
                else
                {
                    for (int x = 0; x < generationsPerRound; x++)
                    {
                        if(x < (generationsPerRound / 2))
                        {
                            Debug.Log(" Training Individual" + x);
                            Active = true;
                            individual = x;
                            NestBlocks = 0;
                            AliveAnts = 1000;
                            Awake();
                            GenerateData();
                            GenerateChunks();
                            Camera.main.transform.position = new Vector3(0 / 2, Blocks.GetLength(1), 0);
                            Camera.main.transform.LookAt(new Vector3(Blocks.GetLength(0), 0, Blocks.GetLength(2)));
                            saves[x] = GenerateAntsWithSpecifiedStats(saves[x].health_share_chance, saves[x].health_share_with_queen_chance, saves[x].queen_health_share_chance, saves[x].eatChance, saves[x].digChance, saves[x].createNestChance);
                            yield return new WaitForSeconds(IndividualTrainingTimeinSeconds);
                            saves[x].NestSize = NestBlocks;
                            Debug.Log("NestSize for individual " + x + " is " + saves[x].NestSize);
                            Active = false;
                            try
                            {
                                clearWorld();
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            Debug.Log(" Training Individual" + x);
                            Active = true;
                            individual = x;
                            NestBlocks = 0;
                            AliveAnts = 1000;
                            Awake();
                            GenerateData();
                            GenerateChunks();
                            Camera.main.transform.position = new Vector3(0 / 2, Blocks.GetLength(1), 0);
                            Camera.main.transform.LookAt(new Vector3(Blocks.GetLength(0), 0, Blocks.GetLength(2)));
                            int parent1 = RNG.Next(generationsPerRound / 2);
                            int parent2 = RNG.Next(generationsPerRound / 2);
                            saves[x] = generateAntsFromParents(saves[parent1],saves[parent2]);
                            yield return new WaitForSeconds(IndividualTrainingTimeinSeconds);
                            saves[x].NestSize = NestBlocks;
                            Debug.Log("NestSize for individual " + x + " is " + saves[x].NestSize);
                            Active = false;
                            try
                            {
                                clearWorld();
                            }
                            catch
                            {

                            }
                        }

                    }
                    Array.Sort(saves, new Comparer());
                    SaveFile.Save(saves[0].health_share_with_queen_chance, saves[0].queen_health_share_chance, saves[0].eatChance, saves[0].digChance, saves[0].NestSize, saves[0].createNestChance, saves[0].health_share_chance);
                    Debug.Log("Best NestSize for generation " + i + " is " + saves[0].NestSize);
                }

            }
            try
            {
                for (int x = 0; x < Positions.GetLength(0); x++)
                {
                    for (int y = 0; y < Positions.GetLength(1); y++)
                    {
                        for (int z = 0; z < Positions.GetLength(2); z++)
                        {
                            if (Positions[x, y, z] != null)
                            {
                                for (int k = 0; k < Positions[x, y, z].Count; k++)
                                {
                                    Destroy(Positions[x, y, z][k]);
                                    Positions[x, y, z][k] = null;
                                }
                            }

                        }
                    }
                }
            }
            catch
            {

            }

        }
        void OnGUI()
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 20;
            switch (view)
            {
                case views.MainMenu:
                    if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 40, 150, 30), "Best Population"))
                    {
                        NestBlocks = 0;
                        AliveAnts = 1000;
                        Awake();
                        GenerateData();
                        GenerateChunks();
                        Camera.main.transform.position = new Vector3(0 / 2, Blocks.GetLength(1), 0);
                        Camera.main.transform.LookAt(new Vector3(Blocks.GetLength(0), 0, Blocks.GetLength(2)));
                        SaveFile s = SaveFile.load();
                        if (s != null)
                        {
                            Debug.Log("Best Population Loaded");
                            GenerateAntsWithSpecifiedStats(s.health_share_chance, s.health_share_with_queen_chance, s.queen_health_share_chance, s.eatChance, s.digChance, s.createNestChance);
                        }
                        else
                        {
                            Debug.Log("Best Population not found");
                            GenerateAnts();
                        }

                        view = views.CurrentPopulation;
                    }
                    if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 80, 150, 30), "Train"))
                    {
                        view = views.Train;
                        Active = true;
                        StartCoroutine(Train());

                    }
                    break;
                case views.CurrentPopulation:
                    string a = "Alive Ants " + AliveAnts;
                    string b = "Nest Blocks " + NestBlocks;
                    GUI.Label(new Rect(10, 10, 150, 20), a);
                    GUI.Label(new Rect(Screen.width - 200, 10, 150, 20), b);
                    if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 40, 150, 30), "MainMenu"))
                    {
                        Active = false;
                        try
                        {
                            clearWorld();
                        }
                        catch
                        {

                        }
                        view = views.MainMenu;
                    }
                    break;
                case views.Train:
                    string a2 = "Alive Ants " + AliveAnts;
                    string b2 = "Nest Blocks " + NestBlocks;
                    string c = "Iteration " + Iteration;
                    string d = "Individual " + individual;
                    GUI.Label(new Rect(10, 10, 150, 20), a2);
                    GUI.Label(new Rect(Screen.width - 200, 10, 150, 20), b2);
                    GUI.Label(new Rect(10, 50, 150, 20), c);
                    GUI.Label(new Rect(Screen.width - 200, 50, 150, 20), d);
                    if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 40, 150, 30), "MainMenu"))
                    {
                        Active = false;
                        try
                        {
                            clearWorld();
                        }
                        catch
                        {

                        }

                        view = views.MainMenu;
                    }
                    break;

            }
        }
        #endregion

        #endregion
    }

}
