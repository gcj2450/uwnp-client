using Newtonsoft.Json.Linq;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UWNP;
using static UnityEngine.Rendering.DebugUI;

namespace UWNP
{
    // 定义MyClass类
    public class MyClass
    {
        // 定义类的字段
        // 例如：
        // public int Id { get; set; }
        // public string Name { get; set; }
    }

    [ProtoContract]
    public class DynamicMessage
    {
        [ProtoMember(1)]
        public Dictionary<string, Any> Data { get; set; }
    }

    [ProtoContract]
    public class Any
    {
        [ProtoMember(1, DynamicType = true)]
        public object Value { get; set; }
    }

    [ProtoContract]
    public class AnyList
    {
        [ProtoMember(1, DynamicType = true)]
        public List<object> Values { get; set; }
    }

//    protobuf-net库的ProtoMember属性支持多种数据类型。下面是一些常见的数据类型示例：
//基本类型：
//int, uint, long, ulong, float, double, bool, string, byte, sbyte, short, ushort, char.
//枚举类型：
//枚举类型可以直接使用对应的枚举定义。
//自定义类型：
//自定义类和结构体需要使用[ProtoContract] 属性进行标记，并在成员上使用[ProtoMember] 属性指定序列化顺序。
//可重复类型：
//可以使用List<T> 或数组来表示可重复的成员。
//字典类型：
//可以使用Dictionary<TKey, TValue> 来表示键值对的字典。
//其他类型：
//DateTime, TimeSpan, Guid, Uri, byte[], MemoryStream.
//此外，protobuf-net库还支持对许多其他数据类型的扩展。你可以通过自定义类型的序列化和反序列化方法来扩展protobuf-net的功能，或者使用自定义的类型处理器（TypeHandler）。

//注意，在使用自定义类或结构体作为消息类型时，确保为每个成员指定适当的[ProtoMember] 标识符，以确保正确的序列化和反序列化顺序。

    //    // 创建DynamicMessage对象
    //    var dynamicMessage = new DynamicMessage
    //    {
    //        Data = new Dictionary<string, Any>
    //    {
    //        { "key1", new Any { Value = 42 } },
    //        { "key2", new Any { Value = "Hello, world!" } },
    //        { "key3", new Any { Value = new DynamicMessage { Data = new Dictionary<string, Any>() } } }
    //    }
    //    };

    //    // 将DynamicMessage对象序列化为字节流
    //    byte[] serializedData;
    //using (MemoryStream stream = new MemoryStream())
    //{
    //    Serializer.Serialize(stream, dynamicMessage);
    //    serializedData = stream.ToArray();
    //}

    //// 从字节流反序列化为DynamicMessage对象
    //DynamicMessage deserializedMessage;
    //using (MemoryStream stream = new MemoryStream(serializedData))
    //{
    //    deserializedMessage = Serializer.Deserialize<DynamicMessage>(stream);
    //}

    //// 访问反序列化后的对象的属性
    //Dictionary<string, Any> data = deserializedMessage.Data;
    //Any value1 = data["key1"];
    //Any value2 = data["key2"];
    //Any value3 = data["key3"];

    [ProtoContract]
    class Header
    {
        [ProtoMember(1, IsRequired = true)]
        public int cmd { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public int seq { get; set; }
    }

    [ProtoContract]
    class Person
    {
        [ProtoMember(1, IsRequired = true)]
        public Header header { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public long id { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string name { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public int age { get; set; }

        [ProtoMember(5, IsRequired = false)]
        public string email { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public int[] array;
    }

    //列表和数组定义示例
    [ProtoContract]
    public class MyMessage
    {
        [ProtoMember(1)]
        public List<int> IntList { get; set; }

        [ProtoMember(2)]
        public string[] StringArray { get; set; }
    }

    [ProtoContract(SkipConstructor = true)]
    public class ChunkData
    {
        public ChunkData(ChunkData sourceChunk)
        {
            ChunkID = sourceChunk.ChunkID;
            Checksum = sourceChunk.Checksum;
            Offset = sourceChunk.Offset;
            CompressedLength = sourceChunk.CompressedLength;
            UncompressedLength = sourceChunk.UncompressedLength;
        }

        /// <summary>
        /// Gets the SHA-1 hash chunk id.
        /// </summary>
        [ProtoMember(1)]
        public byte[] ChunkID { get; private set; }

        /// <summary>
        /// Gets the expected Adler32 checksum of this chunk.
        /// </summary>
        [ProtoMember(2)]
        public byte[] Checksum { get; private set; }

        /// <summary>
        /// Gets the chunk offset.
        /// </summary>
        [ProtoMember(3)]
        public ulong Offset { get; private set; }

        /// <summary>
        /// Gets the compressed length of this chunk.
        /// </summary>
        [ProtoMember(4)]
        public uint CompressedLength { get; private set; }

        /// <summary>
        /// Gets the decompressed length of this chunk.
        /// </summary>
        [ProtoMember(5)]
        public uint UncompressedLength { get; private set; }
    }


    [ProtoContract]
    public class Package
    {
        [ProtoMember(1)]
        public uint packageType;

        [ProtoMember(2)]
        public string route = null;

        [ProtoMember(3)]
        public uint packID = 0;

        [ProtoMember(4)]
        public byte[] buff = null;

        [ProtoMember(5)]
        public string modelName = null;
    }

    [ProtoContract]
    public class Message<T>
    {
        [ProtoMember(1)]
        public uint err;

        [ProtoMember(2)]
        public string errMsg = default;

        [ProtoMember(3)]
        public T info = default;

        public Message() { }

        public Message(uint err, string errMsg, T info)
        {
            this.err = err;
            this.errMsg = errMsg;
            this.info = info;
        }
    }

    [ProtoContract]
    public class HandShake
    {
        [ProtoMember(1)]
        public string token;
    }

    [ProtoContract]
    public class Heartbeat
    {
        [ProtoMember(1)]
        public uint heartbeat;
    }

    /// <summary>
    /// 聊天信息
    /// </summary>
    [ProtoContract]
    public class ChatMessage
    {
        [ProtoMember(1)]
        public int channel;
        [ProtoMember(2)]
        public int msgType;

        /// <summary>
        /// 发出方
        /// </summary>
        [ProtoMember(3)]
        public long fromUid;
        [ProtoMember(4)]
        public string fromUserName;
        [ProtoMember(5)]
        public string content;

        /// <summary>
        /// 接收方
        /// </summary>
        [ProtoMember(6)]
        public long toUid;
        [ProtoMember(7)]
        public string toUserName;
        [ProtoMember(8)]
        public long toLid;//used for league chat  
        [ProtoMember(9)]
        public long sendTime;

    }

    [ProtoContract]
    public class NetTransform
    {
        [ProtoMember(1)]
        public int id;
        [ProtoMember(2)]
        public float velocity;
        [ProtoMember(3)]
        public float px;
        [ProtoMember(4)]
        public float py;
        [ProtoMember(5)]
        public float pz;

        [ProtoMember(6)]
        public float rx;

        [ProtoMember(7)]
        public float ry;
        [ProtoMember(8)]
        public float rz;
        [ProtoMember(9)]
        public long timeStamp;
    }

}