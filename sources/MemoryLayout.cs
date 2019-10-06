using System;
using System.Numerics;
using System.Text;

namespace FFRadarBuddy
{
    public class MemoryLayout
    {
        // actor and target => based on project
        // https://github.com/FFXIVAPP/sharlayan
        //
        // camera: researched on my own

        public static MemoryPath memPathActors = new MemoryPathSignature("488b420848c1e8033da701000077248bc0488d0d", 0);
        public static MemoryPath memPathTarget = new MemoryPathSignature("41bc000000e041bd01000000493bc47555488d0d", 144);
        public static MemoryPath memPathCamera = new MemoryPathSignature("4c897c242083f8ff75054533ffeb0d48984c8d3d", 0, 0);

        public class ActorConsts
        {
            public const int Size = 200;

            public const int Name = 48;             // string
            public const int ActorIdA = 116;        // uint32
            public const int ActorIdB = 120;        // uint32
            public const int NpcId = 128;           // uint32
            public const int Type = 140;            // uint8
            public const int SubType = 141;         // uint8
            public const int Position = 160;        // 3x float
            public const int HitBoxRadius = 192;    // float
        }

        public class TargetConsts
        {
            public const int Size = 48;
            public const int Current = 40;          // uint64 (actor ptr)
        }

        public class CameraConsts
        {
            public const int Size = 0x1d0;
            public const int Position = 0x1b0;       // 3x float
            public const int Target = 0x1c0;         // 3x float
            public const int Distance = 0x118;      // float
            public const int Fov = 0x124;           // float
        }

        public enum ActorType : byte
        {
            None = 0,
            Player = 1,
            Monster = 2,
            Npc = 3,
            Treasure = 4,
            Aetheryte = 5,
            Gathering = 6,
            Interaction = 7,
            Mount = 8,
            Minion = 9,
            Retainer = 10,
            Housing = 12,
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public class ActorData
        {
            public string Name;
            public uint ActorIdA;
            public uint ActorIdB;
            public uint NpcId;
            public ActorType Type;
            public byte SubType;
            public Vector3 Position = new Vector3();
            public float Radius;
            public long UniqueId;

            public ActorData() { }
            public ActorData(byte[] bytes) { Set(bytes); }

            public void SetIdOnly(byte[] bytes)
            {
                ActorIdA = BitConverter.ToUInt32(bytes, ActorConsts.ActorIdA);
                ActorIdB = BitConverter.ToUInt32(bytes, ActorConsts.ActorIdB);
                NpcId = BitConverter.ToUInt32(bytes, ActorConsts.NpcId);

                UniqueId = (ActorIdB != 0) ? ActorIdB : ActorIdA;
                UniqueId <<= 32;
                UniqueId |= NpcId;
            }

            public void SetDataOnly(byte[] bytes)
            {
                Type = (ActorType)bytes[ActorConsts.Type];
                SubType = bytes[ActorConsts.SubType];
                Position.X = BitConverter.ToSingle(bytes, ActorConsts.Position);
                Position.Y = BitConverter.ToSingle(bytes, ActorConsts.Position + 4);
                Position.Z = BitConverter.ToSingle(bytes, ActorConsts.Position + 8);
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

            public void Set(byte[] bytes)
            {
                SetIdOnly(bytes);
                SetDataOnly(bytes);
            }
        }

        public class TargetData
        {
            public long CurrentAddress;

            public TargetData() { }
            public TargetData(byte[] bytes) { Set(bytes); }

            public void Set(byte[] bytes)
            {
                CurrentAddress = BitConverter.ToInt64(bytes, TargetConsts.Current);
            }
        }

        public class CameraData
        {
            public float Fov;
            public float Distance;
            public Vector3 Position = new Vector3();
            public Vector3 Target = new Vector3();

            public CameraData() { }
            public CameraData(byte[] bytes) { Set(bytes); }

            public void Set(byte[] bytes)
            {
                Fov = BitConverter.ToSingle(bytes, CameraConsts.Fov);
                Distance = BitConverter.ToSingle(bytes, CameraConsts.Distance);
                Position.X = BitConverter.ToSingle(bytes, CameraConsts.Position);
                Position.Y = BitConverter.ToSingle(bytes, CameraConsts.Position + 4);
                Position.Z = BitConverter.ToSingle(bytes, CameraConsts.Position + 8);
                Target.X = BitConverter.ToSingle(bytes, CameraConsts.Target);
                Target.Y = BitConverter.ToSingle(bytes, CameraConsts.Target + 4);
                Target.Z = BitConverter.ToSingle(bytes, CameraConsts.Target + 8);
            }
        }
    }
}
