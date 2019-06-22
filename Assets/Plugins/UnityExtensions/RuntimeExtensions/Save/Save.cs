using System;
using System.IO;

namespace UnityExtensions
{
    /// <summary>
    /// 存档基类。不必直接继承此类，继承 BinarySave 和 TextSave 以简化编程。
    /// </summary>
    public abstract class Save
    {
        internal Exception ToBytes(out byte[] data)
        {
            try
            {
                using (var stream = new MemoryStream(256))
                {
                    Write(stream);
                    data = stream.ToArray();
                    return null;
                }
            }
            catch (Exception e)
            {
                data = null;
                return e;
            }
        }


        internal Exception FromBytes(ref byte[] data)
        {
            try
            {
                using (var stream = new MemoryStream(data))
                {
                    Read(stream);
                    return null;
                }
            }
            catch (Exception e)
            {
                Reset();
                return e;
            }
        }


        /// <summary>
        /// 重置数据. 当读取失败时会自动执行
        /// </summary>
        public abstract void Reset();


        /// <summary>
        /// 从 Stream 中读取数据 (不必在内部捕获异常，如果抛出异常，则自动执行 Reset)
        /// </summary>
        protected abstract void Read(Stream stream);


        /// <summary>
        /// 写入数据到 Stream (不必在内部捕获异常)
        /// </summary>
        protected abstract void Write(Stream stream);

    } // class Save


    /// <summary>
    /// 二进制存档
    /// 继承此类并实现 Reset, Read 和 Write
    /// </summary>
    public abstract class BinarySave : Save
    {
        protected sealed override void Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                Read(reader);
            }
        }


        protected sealed override void Write(Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                Write(writer);
            }
        }


        /// <summary>
        /// 从 Stream 中读取数据 (不必在内部捕获异常，如果抛出异常，则自动执行 Reset)
        /// </summary>
        protected abstract void Read(BinaryReader reader);


        /// <summary>
        /// 写入数据到 Stream (不必在内部捕获异常)
        /// </summary>
        protected abstract void Write(BinaryWriter writer);

    } // class BinarySave


    /// <summary>
    /// 文本存档
    /// 继承此类并实现 Reset, Read 和 Write
    /// </summary>
    public abstract class TextSave : Save
    {
        protected sealed override void Read(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                Read(reader);
            }
        }


        protected sealed override void Write(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                Write(writer);
            }
        }


        /// <summary>
        /// 从 Stream 中读取数据 (不必在内部捕获异常，如果抛出异常，则自动执行 Reset)
        /// </summary>
        protected abstract void Read(StreamReader reader);


        /// <summary>
        /// 写入数据到 Stream (不必在内部捕获异常)
        /// </summary>
        protected abstract void Write(StreamWriter writer);

    } // class TextSave

} // namespace UnityExtensions