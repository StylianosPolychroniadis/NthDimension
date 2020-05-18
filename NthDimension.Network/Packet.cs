/* LICENSE
 * Copyright (C) 2017 - 2018 Rafa Software, Hellas - All Rights Reserved
 * Written by Stylianos N. Polychroniadis (info@rafasoftware.com) http://www.rafasoftware.com
 * 
 * This file is part of MySoci.Net Social Network
 * 
 * WARNING! Commercial Software, All Use Must Be Licensed
 * This software is protected by Hellenic Copyright Law and International Treaties. 
 * Unauthorized use, duplication, reverse engineering, any form of redistribution, or 
 * use in part or in whole other than by prior, express, printed and signed license 
 * for use is subject to civil and criminal prosecution. 
*/

using System;
using System.Collections.Generic;
using System.IO;
using Lidgren.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace NthDimension.Network
{
    public class Packet
    {
        #region Singleton 

        public static Packet Instance
        {
            get { return _instance ?? (_instance = new Packet()); }
        }
        private static Packet _instance;
        private readonly Dictionary<ushort, Type> _packetTypes;

        private Packet()
        {
            _packetTypes = new Dictionary<ushort, Type>();
        }

        #endregion

        public void Register(ushort packetCode, Type packetType)
        {
            _packetTypes.Add(packetCode, packetType);
        }

        public NetOutgoingMessage Serialize(ushort packetCode, object data, NetOutgoingMessage basePacket)
        {
            byte[] buffer;

            using (var ms = new MemoryStream())
            using (var writer = new BsonWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.Objects;
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;
                serializer.Serialize(writer, data);

                buffer = ms.ToArray();
            }

            basePacket.Write(packetCode);
            basePacket.WritePadBits();
            basePacket.Write(buffer.Length);
            basePacket.Write(buffer);

            return basePacket;
        }

        public ushort Deserialize(NetIncomingMessage basePacket, out object data)
        {
            var packetCode = basePacket.ReadUInt16();

            basePacket.SkipPadBits();

            var bufferLength = basePacket.ReadInt32();

            var buffer = basePacket.ReadBytes(bufferLength);

            Type packetType;
            if (!_packetTypes.TryGetValue(packetCode, out packetType))
            {
                throw new Exception("Invalid packet code");
            }


            using (var ms = new MemoryStream(buffer))
            using (var reader = new BsonReader(ms))
            {
                var serializer = new JsonSerializer();
                data = serializer.Deserialize(reader, packetType);
            }

            return packetCode;
        }
    }
}
