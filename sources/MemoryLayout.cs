using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRadarBuddy
{
    public class MemoryLayout
    {
        // offsets and signatures based on projects:
        // https://github.com/FFXIVAPP/sharlayan
        // https://github.com/goaaats/Nhaama

        public static MemoryPath memPathActors = new MemoryPath(0x1B29B40);
        public static MemoryPath memPathCamera = new MemoryPath(0x1B28530, 0);
        public static MemoryPath memPathTarget = new MemoryPathSignature("41bc000000e041bd01000000493bc47555488d0d", 144);

        public class ActorConsts
        {
            public const int Size = 200;

            public const int Name = 48;             // string
            public const int ActorId = 116;         // uint32
            public const int NpcId = 128;           // uint32
            public const int Type = 140;            // uint8
            public const int SubType = 141;         // uint8
            public const int PosX = 160;            // float
            public const int PosY = 168;            // float
            public const int PosZ = 164;            // float
            public const int HitBoxRadius = 192;    // float
        }

        public class TargetConsts
        {
            public const int Size = 48;
            public const int Current = 40;          // uint64 (actor ptr)
        }

        public class CameraConsts
        {
            public const int Size = 0x1c0;
            public const int Fov = 0x124;       // float
            public const int Pos = 0x1A0;       // 3x float
            public const int Focus = 0x1B0;     // 3x float
        }

        public enum ActorType : byte
        {
            None = 0,
            Player = 1,
            BattleNpc = 2,
            EventNpc = 3,
            Treasure = 4,
            Aetheryte = 5,
            GatheringPoint = 6,
            EventObj = 7,
            MountType = 8,
            Minion = 9,
            Retainer = 10,
            Area = 11,
            Housing = 12,
            Cutscene = 13,
            CardStand = 14,
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public class ActorData
        {
            public string Name;
            public uint ActorId;
            public uint NpcId;
            public ActorType Type;
            public byte SubType;
            public float PosX;
            public float PosY;
            public float PosZ;
            public float Radius;

            public ActorData(byte[] bytes) { Set(bytes); }

            public void Set(byte[] bytes)
            {
                ActorId = BitConverter.ToUInt32(bytes, ActorConsts.ActorId);
                NpcId = BitConverter.ToUInt32(bytes, ActorConsts.NpcId);
                Type = (ActorType)bytes[ActorConsts.Type];
                SubType = bytes[ActorConsts.SubType];
                PosX = BitConverter.ToSingle(bytes, ActorConsts.PosX);
                PosY = BitConverter.ToSingle(bytes, ActorConsts.PosY);
                PosZ = BitConverter.ToSingle(bytes, ActorConsts.PosZ);
                Radius = BitConverter.ToSingle(bytes, ActorConsts.HitBoxRadius);

                // read string at Actor.Name
                int useSize = Math.Max(255, bytes.Length - ActorConsts.Name);
                byte[] stringBytes = new byte[useSize];
                for (int Idx = 0; Idx < useSize; Idx++)
                {
                    if (bytes[ActorConsts.Name + Idx] == 0)
                    {
                        Array.Resize(ref stringBytes, Idx);
                        break;
                    }

                    stringBytes[Idx] = bytes[ActorConsts.Name + Idx];
                }

                Name = Encoding.UTF8.GetString(stringBytes);
            }
        }

        public class TargetData
        {
            public long CurrentAddr;

            public TargetData(byte[] bytes) { Set(bytes); }

            public void Set(byte[] bytes)
            {
                CurrentAddr = BitConverter.ToInt64(bytes, TargetConsts.Current);
            }
        }

        public class CameraData
        {
            public float Fov;
            public float PosX;
            public float PosY;
            public float PosZ;
            public float FocusX;
            public float FocusY;
            public float FocusZ;

            public CameraData(byte[] bytes) { Set(bytes); }

            public void Set(byte[] bytes)
            {
                Fov = BitConverter.ToSingle(bytes, CameraConsts.Fov);
                PosX = BitConverter.ToSingle(bytes, CameraConsts.Pos);
                PosY = BitConverter.ToSingle(bytes, CameraConsts.Pos + 4);
                PosZ = BitConverter.ToSingle(bytes, CameraConsts.Pos + 8);
                FocusX = BitConverter.ToSingle(bytes, CameraConsts.Focus);
                FocusY = BitConverter.ToSingle(bytes, CameraConsts.Focus + 4);
                FocusZ = BitConverter.ToSingle(bytes, CameraConsts.Focus + 8);
            }
        }
    }
}
