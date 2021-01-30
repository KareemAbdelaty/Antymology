﻿using Antymology.Helpers;
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

        /// <summary>
        /// Random number generator.
        /// </summary>
        private SimplexNoise SimplexNoise;

        #endregion

        #region Initialization

        // Awake is called before any start method is called.
        void Awake()
        {
            // Generate new random number generator
            RNG = new System.Random(ConfigurationManager.Instance.Seed);

            // Generate new simplex noise generator
            SimplexNoise = new SimplexNoise(ConfigurationManager.Instance.Seed);

            // Initialize a new 3D array of blocks with size of the number of chunks times the size of each chunk
            Blocks = new AbstractBlock[
                ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter,
                ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter,
                ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter];

            // Initialize a new 3D array of chunks with size of the number of chunks
            Chunks = new Chunk[
                ConfigurationManager.Instance.World_Diameter,
                ConfigurationManager.Instance.World_Diameter,
                ConfigurationManager.Instance.World_Diameter];
        }

        /// <summary>
        /// Called after every awake has been called.
        /// </summary>
        private void Start()
        {
            GenerateData();
            GenerateChunks();
        }

        #endregion

        #region Methods

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
                WorldXCoordinate > ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter ||
                WorldYCoordinate > ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter ||
                WorldZCoordinate > ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter
            )
                throw new IndexOutOfRangeException();

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
                LocalXCoordinate > ConfigurationManager.Instance.Chunk_Diameter ||
                LocalYCoordinate > ConfigurationManager.Instance.Chunk_Diameter ||
                LocalZCoordinate > ConfigurationManager.Instance.Chunk_Diameter ||
                ChunkXCoordinate < 0 ||
                ChunkYCoordinate < 0 ||
                ChunkZCoordinate < 0 ||
                ChunkXCoordinate > ConfigurationManager.Instance.World_Diameter ||
                ChunkYCoordinate > ConfigurationManager.Instance.World_Diameter ||
                ChunkZCoordinate > ConfigurationManager.Instance.World_Diameter 
            )
                throw new IndexOutOfRangeException();

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
                WorldXCoordinate > ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter ||
                WorldYCoordinate > ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter ||
                WorldZCoordinate > ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter
            )
                throw new IndexOutOfRangeException();

            Blocks[WorldXCoordinate, WorldYCoordinate, WorldZCoordinate] = toSet;
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
                LocalXCoordinate > ConfigurationManager.Instance.Chunk_Diameter ||
                LocalYCoordinate > ConfigurationManager.Instance.Chunk_Diameter ||
                LocalZCoordinate > ConfigurationManager.Instance.Chunk_Diameter ||
                ChunkXCoordinate < 0 ||
                ChunkYCoordinate < 0 ||
                ChunkZCoordinate < 0 ||
                ChunkXCoordinate > ConfigurationManager.Instance.World_Diameter ||
                ChunkYCoordinate > ConfigurationManager.Instance.World_Diameter ||
                ChunkZCoordinate > ConfigurationManager.Instance.World_Diameter
            )
                throw new IndexOutOfRangeException();

            Blocks
            [
                ChunkXCoordinate * LocalXCoordinate,
                ChunkYCoordinate * LocalYCoordinate,
                ChunkZCoordinate * LocalZCoordinate
            ] = toSet;
        }

        #endregion

        #region Helpers

        #region Blocks

        private void GenerateData()
        {
            GeneratePreliminaryWorld();
        }

        private void GeneratePreliminaryWorld()
        {

        }

        #endregion

        #region Chunks


        #endregion

        #endregion
    }
}
