using System.IO;

namespace UnityExtensions.Test
{
    public class Player : ScriptableComponent, IBinarySavable
    {
        public int level;
        public float hp;

        public static Player instance { get; private set; }


        void Awake()
        {
            instance = this;
        }


        void IBinarySavable.Read(BinaryReader reader)
        {
            level = reader.ReadInt32();
            hp = reader.ReadSingle();
        }


        void IBinarySavable.Write(BinaryWriter writer)
        {
            writer.Write(level);
            writer.Write(hp);
        }


        void IBinarySavable.Reset()
        {
            level = 1;
            hp = 1;
        }
    }
}