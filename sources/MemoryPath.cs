using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRadarBuddy
{
    public class MemoryPath
    {
        protected long[] PointerPath;
        protected long ResolvedAddress;

        public MemoryPath(params long[] pointerPath)
        {
            PointerPath = pointerPath;
            ResolvedAddress = 0;
        }

        public long GetResolvedAddress()
        {
            return ResolvedAddress;
        }

        public virtual void Invalidate()
        {
            PointerPath = null;
            ResolvedAddress = 0;
        }

        public virtual bool IsValid()
        {
            return (PointerPath != null);
        }

        public bool IsResolved()
        {
            return ResolvedAddress != 0;
        }

        public override string ToString()
        {
            string Desc = "(base)";
            for (int Idx = 0; Idx < PointerPath.Length; Idx++)
            {
                Desc += " +0x" + PointerPath[Idx].ToString("x");
                if (Idx < (PointerPath.Length - 1)) { Desc += "]"; }
            }

            return Desc;
        }

        public virtual bool Resolve(MemoryScanner scanner)
        {
            ResolvedAddress = 0;
            long Address = scanner.GetBaseAddress();
            try
            {
                Address += PointerPath[0];
                for (int Idx = 1; Idx < PointerPath.Length; Idx++)
                {
                    Address = scanner.ReadPointer(Address) + PointerPath[Idx];
                }

                ResolvedAddress = Address;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to resolve pointer path! Exception:" + ex);
                ResolvedAddress = 0;
            }

            return ResolvedAddress != 0;
        }
    }

    public class MemoryPathSignature : MemoryPath
    {
        private string PatternDesc;
        private byte[] PatternBytes;
        private byte[] PatternMask;
        private long PatternJumpAddress;

        public MemoryPathSignature(string signature, params long[] pointerPath) : base(pointerPath)
        {
            PatternDesc = signature;
            PatternBytes = null;
            PatternMask = null;
            PatternJumpAddress = 0;

            InitializePattern();
        }

        private void InitializePattern()
        {
            int patternSize = PatternDesc.Length / 2;
            PatternBytes = new byte[patternSize];
            PatternMask = new byte[patternSize];
            bool needsMask = false;

            int charIdx = 0;
            for (int Idx = 0; Idx < patternSize; Idx++)
            {
                char hexC0 = PatternDesc[charIdx]; charIdx++;
                char hexC1 = PatternDesc[charIdx]; charIdx++;

                if (hexC0 == '*' || hexC0 == '?')
                {
                    PatternBytes[Idx] = 0;
                    PatternMask[Idx] = 0;
                    needsMask = true;
                }
                else
                {
                    PatternBytes[Idx] = (byte)((GetNumberFromHexChar(hexC0) << 4) + GetNumberFromHexChar(hexC1));
                    PatternMask[Idx] = 1;
                }
            }

            if (!needsMask)
            {
                PatternMask = null;
            }
        }

        private int GetNumberFromHexChar(char hexChar)
        {
            return (hexChar >= '0' && hexChar <= '9') ? (hexChar - '0') :
                (hexChar >= 'A' && hexChar <= 'F') ? (hexChar - 'A' + 10) :
                (hexChar >= 'a' && hexChar <= 'f') ? (hexChar - 'a' + 10) :
                0;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            PatternBytes = null;
            PatternJumpAddress = 0;
        }

        public override bool IsValid()
        {
            return (PatternBytes != null) && base.IsValid();
        }

        public override string ToString()
        {
            string Desc = PatternDesc + " => " + (PatternJumpAddress != 0 ? ("0x" + PatternJumpAddress.ToString("x")) : "??") + "]";
            for (int Idx = 0; Idx < PointerPath.Length; Idx++)
            {
                Desc += " +0x" + PointerPath[Idx].ToString("x");
                if (Idx < (PointerPath.Length - 1)) { Desc += "]"; }
            }

            return Desc;
        }

        public override bool Resolve(MemoryScanner scanner)
        {
            ResolvedAddress = 0;
            try
            {
                // sig match: start of opcode sequence
                // move to end and do short jump
                PatternJumpAddress = scanner.FindPatternMatchFull(PatternBytes, PatternMask);
                if (PatternJumpAddress != 0)
                {
                    long NextAddress = PatternJumpAddress + PatternBytes.Length;
                    int ShortJumpOffset = scanner.ReadInt(NextAddress);
                    NextAddress += ShortJumpOffset + 4;

                    for (int Idx = 0; Idx < PointerPath.Length - 1; Idx++)
                    {
                        NextAddress = scanner.ReadPointer(NextAddress + PointerPath[Idx]);
                    }

                    if (PointerPath.Length > 0)
                    {
                        NextAddress += PointerPath.Last();
                    }

                    ResolvedAddress = NextAddress;
                }
                else
                {
                    Logger.WriteLine("Failed to resolve pointer path! No signature match");
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to resolve pointer path! Exception:" + ex);
                ResolvedAddress = 0;
            }

            return ResolvedAddress != 0;
        }
    }
}
