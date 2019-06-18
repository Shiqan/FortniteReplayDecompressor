namespace FortniteReplayReaderDecompressor.Core
{
    /// <summary>
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Public/Serialization/Archive.h
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/Core/Private/Serialization/Archive.cpp
    /// </summary>
    public abstract class FArchive
    {
        private byte[] Data;
        private uint ArEngineNetVer;
        private uint ArGameNetVer;
        private bool ArIsError;

        public int Position { get; private set; }

        public FArchive(byte[] data)
        {
            this.Data = data;
        }


        /// <summary>
        /// Sets the archive engine network version.
        /// </summary>
        /// <param name="InEngineNetVer"></param>
        public virtual void SetEngineNetVer(uint InEngineNetVer)
        {
        }

        /// <summary>
        /// Sets the archive game network version.
        /// </summary>
        /// <param name="InGameNetVer"></param>
        public virtual void SetGameNetVer(uint InGameNetVer)
        {
        }

        public virtual bool AtEnd()
        {
        }

        public abstract void CountBytes(uint InNum, uint InMax);
        public virtual bool GetError() { return ArIsError; }
        public abstract void Seek(int pos);
        public abstract int TotalSize();
        public virtual int Tell() { }

        public virtual void Close() { }

        public void Dispose() { }

        public virtual int PeekChar() { }

        public virtual int Read(byte[] buffer, int index, int count) { }

        public virtual int Read(char[] buffer, int index, int count) { }

        public virtual int Read(Span<byte> buffer) { }

        public virtual int Read(Span<char> buffer) { }

        public virtual bool ReadBoolean() { }

        public virtual byte ReadByte() { }

        public virtual byte[] ReadBytes(int count) { }

        public virtual char ReadChar() { }

        public virtual char[] ReadChars(int count) { }

        public virtual decimal ReadDecimal() { }

        public virtual double ReadDouble() { }

        public virtual short ReadInt16() { }

        public virtual int ReadInt32() { }

        public virtual long ReadInt64() { }

        public virtual sbyte ReadSByte() { }

        public virtual float ReadSingle() { }

        public virtual string ReadString() { }

        public virtual ushort ReadUInt16() { }

        public virtual uint ReadUInt32() { }

        public virtual ulong ReadUInt64() { }

        protected virtual void Dispose(bool disposing) { }

    }
}
