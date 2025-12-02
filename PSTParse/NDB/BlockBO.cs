//------------------------------------------------------------------------
// <remarks>
// Forked from PSTParse project available at: <see href="https://github.com/dancash/PST-Parser.git"/>.
// </remarks>
//------------------------------------------------------------------------

namespace PSTParse.NDB
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides business logic operations for handling blocks within a PST file.
    /// </summary>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/a9c1981d-d1ea-457c-b39e-dc7fb0eb95d4"/> for more details on block structures in PST files.
    /// </remarks>
    public static class BlockBO
    {
        #region Methods

        #region Public Methods

        /// <summary>
        /// Retrieves the data associated with a specific node in a PST file.
        /// </summary>
        /// <remarks>
        /// This method retrieves the main data of the specified node and, if the node has sub-nodes, their data as well.
        /// The returned <see cref="NodeDataDTO"/> contains the main node data in the <see cref="NodeDataDTO.NodeData"/> property and the sub-node data in the <see cref="NodeDataDTO.SubNodeData"/> property.
        /// </remarks>
        /// <param name="nid">The unique identifier of the node to retrieve data for.</param>
        /// <param name="pst">The PST file from which the node data will be retrieved. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="NodeDataDTO"/> object containing the main data of the node and, if applicable, the data of its sub-nodes.</returns>
        public static NodeDataDTO GetNodeData(ulong nid, PSTFile pst)
        {
            var nodeBIDs = pst.GetNodeBIDs(nid);
            var mainData = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(nodeBIDs.Item1), pst);
            var subNodeData = new Dictionary<ulong, NodeDataDTO>();
            if (nodeBIDs.Item2 != 0)
            {
                subNodeData = BlockBO.GetSubNodeData(pst.GetBlockBBTEntry(nodeBIDs.Item2), pst);
            }

            return new NodeDataDTO { NodeData = mainData, SubNodeData = subNodeData };
        }

        /// <summary>
        /// Retrieves the data associated with a specific node in a PST file.
        /// </summary>
        /// <remarks>
        /// This method retrieves the main node data using the block ID specified in the <paramref name="entry"/> parameter.
        /// If the sub-node block ID is non-zero, the corresponding sub-node data is also retrieved and included in the result.
        /// </remarks>
        /// <param name="entry">The NBT entry containing the block IDs for the node and sub-node data.</param>
        /// <param name="pst">The PST file from which the data will be retrieved.</param>
        /// <returns>A <see cref="NodeDataDTO"/> object containing the node data and, if applicable, the sub-node data. If the sub-node block ID is not present, the <see cref="NodeDataDTO.SubNodeData"/> property will be <see langword="null"/>.</returns>
        public static NodeDataDTO GetNodeData(NBTENTRY entry, PSTFile pst)
        {
            var mainData = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(entry.BID_Data), pst);
            if (entry.BID_SUB != 0)
            {
                var subnodeData = BlockBO.GetSubNodeData(pst.GetBlockBBTEntry(entry.BID_SUB), pst);
                return new NodeDataDTO { NodeData = mainData, SubNodeData = subnodeData };
            }

            return new NodeDataDTO { NodeData = mainData, SubNodeData = null };
        }

        /// <summary>
        /// Retrieves the data associated with a specific <see cref="SLENTRY"/> in a PST file.
        /// </summary>
        /// <remarks>
        /// This method retrieves the main node data using the block BBT entry associated with the node identifier in <paramref name="entry"/>.
        /// If a sub-node identifier is present, the corresponding sub-node data is also retrieved.
        /// </remarks>
        /// <param name="entry">The <see cref="SLENTRY"/> object containing the identifiers for the node and sub-node data to retrieve.</param>
        /// <param name="pst">The <see cref="PSTFile"/> instance representing the PST file from which the data will be extracted.</param>
        /// <returns>A <see cref="NodeDataDTO"/> object containing the node data and, if available, the sub-node data. If the sub-node identifier in <paramref name="entry"/> is 0, the sub-node data will be <see langword="null"/>.</returns>
        public static NodeDataDTO GetNodeData(SLENTRY entry, PSTFile pst)
        {
            var mainData = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(entry.SubNodeBID), pst);
            if (entry.SubSubNodeBID != 0)
            {
                var subNodeData = BlockBO.GetSubNodeData(pst.GetBlockBBTEntry(entry.SubSubNodeBID), pst);
                return new NodeDataDTO { NodeData = mainData, SubNodeData = subNodeData };
            }

            return new NodeDataDTO { NodeData = mainData, SubNodeData = null };
        }

        /// <summary>
        /// Retrieves the raw data blocks associated with a given BBT (Block B-Tree) entry.
        /// </summary>
        /// <remarks>
        /// This method retrieves the raw bytes associated with the BID (Block ID) of the specified BBT entry.
        /// If the entry is internal, it may involve processing data trees via XBLOCK or XXBLOCK structures.
        /// The method accounts for differences between ANSI and Unicode PST formats, including variations in block trailer sizes.
        /// It also ensures that the block size adheres to the minimum alignment requirements.
        /// The returned data blocks are decrypted using the PST file's encoding scheme.
        /// </remarks>
        /// <param name="entry">The BBT entry for which to retrieve the data blocks.</param>
        /// <param name="pst">The PST file containing the BBT entry.</param>
        /// <returns>A list of <see cref="BlockDataDTO"/> objects representing the data blocks associated with the specified BBT entry. The list may contain one or more data blocks, depending on the type of the entry.</returns>
        /// <exception cref="NotImplementedException">Thrown if the BBT entry type is not recognized or supported.</exception>
        public static List<BlockDataDTO> GetBBTEntryData(IBBTENTRY entry, PSTFile pst)
        {
            var dataSize = entry.BlockByteCount;

            var trailerSize = 16;

            if (pst.Header.IsANSI == true)
            {
                trailerSize = 12;
            }

            var blockSize = entry.BlockByteCount + trailerSize;

            // smallest possible block size is 64 bytes
            if (blockSize % 64 != 0)
            {
                blockSize += 64 - (blockSize % 64);
            }

            List<BlockDataDTO> dataBlocks;

            if (entry.Internal)
            {
                using (var viewer = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, blockSize))
                {
                    var blockBytes = new byte[dataSize];
                    viewer.ReadArray(0, blockBytes, 0, dataSize);

                    var trailerBytes = new byte[trailerSize];
                    viewer.ReadArray(blockSize - trailerSize, trailerBytes, 0, trailerSize);
                    var trailer = new BlockTrailer(trailerBytes, 0);

                    var dataBlockDTO = new BlockDataDTO
                    {
                        Data = blockBytes,
                        PstOffset = entry.BREF.IB,
                        CRCOffset = (uint)((long)entry.BREF.IB + (blockSize - 12)),
                        BBTEntry = entry,
                    };

                    if (pst.Header.IsANSI == true)
                    {
                        // ANSI PSTs have the CRC as the last 4 bytes of the block trailer.
                        dataBlockDTO.CRCOffset = (uint)((long)entry.BREF.IB + (blockSize - 4));
                    }

                    var type = blockBytes[0];
                    var level = blockBytes[1];

                    if (type == 2)
                    {
                        // si or sl entry
                        return new List<BlockDataDTO> { dataBlockDTO };
                    }
                    else if (type == 1)
                    {
                        if (blockBytes[1] == 0x01)
                        {
                            // XBLOCK
                            var xblock = new XBLOCK(dataBlockDTO);
                            return BlockBO.GetXBlockData(xblock, pst);
                        }
                        else
                        {
                            // XXBLOCK
                            var xxblock = new XXBLOCK(dataBlockDTO);
                            return BlockBO.GetXXBlockData(xxblock, pst);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {
                using (var viewer = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, blockSize))
                {
                    var dataBytes = new byte[dataSize];
                    viewer.ReadArray(0, dataBytes, 0, dataSize);

                    var trailerBytes = new byte[trailerSize];
                    viewer.ReadArray(blockSize - trailerSize, trailerBytes, 0, trailerSize);
                    var trailer = new BlockTrailer(trailerBytes, 0);
                    dataBlocks = new List<BlockDataDTO>
                    {
                        new BlockDataDTO
                        {
                            Data = dataBytes,
                            PstOffset = entry.BREF.IB,
                            CRC32 = trailer.CRC,
                            CRCOffset = (uint)(blockSize - 12),
                            BBTEntry = entry,
                        },
                    };
                }
            }

            for (int i = 0; i < dataBlocks.Count; i++)
            {
                var temp = dataBlocks[i].Data;
                DatatEncoder.CryptPermute(temp, temp.Length, false, pst);
            }

            return dataBlocks;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieves sub-node data from the specified BBT entry in the PST file.
        /// </summary>
        /// <remarks>
        /// This method processes the BBT entry to determine whether it contains intermediate or leaf-level data blocks.
        /// If the entry contains intermediate data blocks, the method retrieves data from either SLBlock or SIBlock structures based on the block's characteristics.
        /// If the entry does not contain intermediate data blocks, an exception is thrown.
        /// </remarks>
        /// <param name="entry">The BBT entry from which to retrieve sub-node data. Must not be null.</param>
        /// <param name="pst">The PST file containing the BBT entry. Must not be null.</param>
        /// <returns>A dictionary where the keys are unique identifiers for sub-nodes and the values are <see cref="NodeDataDTO"/> objects representing the data associated with each sub-node.</returns>
        /// <exception cref="Exception">Thrown if the specified BBT entry does not contain intermediate data blocks.</exception>
        private static Dictionary<ulong, NodeDataDTO> GetSubNodeData(IBBTENTRY entry, PSTFile pst)
        {
            var allData = BlockBO.GetBBTEntryData(entry, pst);
            var dataBlock = allData[0];
            if (entry.Internal)
            {
                var type = dataBlock.Data[0];
                var cLevel = dataBlock.Data[1];
                if (cLevel == 0)
                {
                    // SLBlock, no intermediate
                    return BlockBO.GetSLBlockData(new SLBLOCK(dataBlock), pst);
                }
                else
                {
                    // SIBlock
                    return BlockBO.GetSIBlockData(new SIBLOCK(dataBlock), pst);
                }
            }
            else
            {
                throw new Exception("Whoops");
            }
        }

        /// <summary>
        /// Retrieves a dictionary containing node data extracted from the specified SIBlock and its associated SLBlocks.
        /// </summary>
        /// <remarks>
        /// This method processes the entries in the provided SIBlock, resolves the associated SLBlocks, and aggregates their data into a single dictionary.
        /// The returned dictionary combines all node data from the SLBlocks referenced by the SIBlock.
        /// </remarks>
        /// <param name="siBlock">The SIBlock from which to extract node data.</param>
        /// <param name="pst">The PST file used to resolve block data.</param>
        /// <returns>A dictionary where the keys are unique identifiers (ULong) and the values are <see cref="NodeDataDTO"/> objects representing the extracted node data.</returns>
        private static Dictionary<ulong, NodeDataDTO> GetSIBlockData(SIBLOCK siBlock, PSTFile pst)
        {
            var ret = new Dictionary<ulong, NodeDataDTO>();

            foreach (var entry in siBlock.Entries)
            {
                var curSLBlockBBT = pst.GetBlockBBTEntry(entry.SLBlockBID);
                var slBlock = new SLBLOCK(BlockBO.GetBBTEntryData(curSLBlockBBT, pst)[0]);
                var data = BlockBO.GetSLBlockData(slBlock, pst);

                foreach (var item in data)
                {
                    ret.Add(item.Key, item.Value);
                }
            }

            return ret;
        }

        /// <summary>
        /// Retrieves all data associated with the specified SL block and its immediate sub-nodes.
        /// </summary>
        /// <remarks>
        /// This method processes the entries in the provided SL block to extract the main data for each sub-node.
        /// If a sub-node contains additional sub-nodes, their data is also retrieved recursively and stored in the <see cref="NodeDataDTO.SubNodeData"/> property of the corresponding <see cref="NodeDataDTO"/> object.
        /// </remarks>
        /// <param name="slblock">The SL block containing entries that point to sub-nodes.</param>
        /// <param name="pst">The PST file from which block data is retrieved.</param>
        /// <returns>A dictionary where the keys are the unique identifiers (NIDs) of the sub-nodes and the values are <see cref="NodeDataDTO"/> objects containing the data for each sub-node.</returns>
        private static Dictionary<ulong, NodeDataDTO> GetSLBlockData(SLBLOCK slblock, PSTFile pst)
        {
            var ret = new Dictionary<ulong, NodeDataDTO>();
            foreach (var entry in slblock.Entries)
            {
                // this data should represent the main data part of the subnode
                var data = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(entry.SubNodeBID), pst);
                var cur = new NodeDataDTO { NodeData = data };
                ret.Add(entry.SubNodeNID, cur);

                // see if there are sub nodes of this current sub node
                if (entry.SubSubNodeBID != 0)
                {
                    // if there are subnodes, treat them like any other subnode
                    cur.SubNodeData = GetSubNodeData(pst.GetBlockBBTEntry(entry.SubSubNodeBID), pst);
                }
            }

            return ret;
        }

        /// <summary>
        /// Retrieves a list of block data from the specified XBLOCK structure.
        /// </summary>
        /// <param name="xblock">The XBLOCK structure containing the BID entries to process.</param>
        /// <param name="pst">The PST file from which block data will be retrieved.</param>
        /// <returns>A list of <see cref="BlockDataDTO"/> objects representing the data extracted from the BID entries in the specified XBLOCK structure.</returns>
        private static List<BlockDataDTO> GetXBlockData(XBLOCK xblock, PSTFile pst)
        {
            var ret = new List<BlockDataDTO>();
            foreach (var bid in xblock.BIDEntries)
            {
                var bbtEntry = pst.GetBlockBBTEntry(bid);
                ret.AddRange(BlockBO.GetBBTEntryData(bbtEntry, pst));
            }

            return ret;
        }

        /// <summary>
        /// Retrieves a list of block data from the specified XXBLOCK structure.
        /// </summary>
        /// <remarks>
        /// This method processes each block identifier in the provided XXBLOCK structure, retrieves the corresponding block data from the PST file, and aggregates the results into a single list.
        /// </remarks>
        /// <param name="xxBlock">The XXBLOCK structure containing the block identifiers to process.</param>
        /// <param name="pst">The PST file from which block data will be retrieved.</param>
        /// <returns>A list of <see cref="BlockDataDTO"/> objects representing the data extracted from the blocks referenced by the XXBLOCK structure.</returns>
        private static List<BlockDataDTO> GetXXBlockData(XXBLOCK xxBlock, PSTFile pst)
        {
            var ret = new List<BlockDataDTO>();
            foreach (var bid in xxBlock.BIDEntries)
            {
                var bbtEntry = pst.GetBlockBBTEntry(bid);
                var curXBlockData = BlockBO.GetBBTEntryData(bbtEntry, pst);

                foreach (var block in curXBlockData)
                {
                    ret.Add(block);
                }
            }

            return ret;
        }

        #endregion

        #endregion
    }
}
