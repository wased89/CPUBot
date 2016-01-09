using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUBot
{
    public class MemAndReg
    {
        private static byte[] memory = new byte[65536];

        private static byte registerA;
        private static byte registerB;
        private static byte outputReg;

        public static void SetRegA(byte a)
        {
            //Console.WriteLine("MAN SET REG A");
            registerA = a;
        }
        public static void SetRegA(UInt16 a)
        {
            registerA = memory[a]; //load value from memory and put it in registerA
        }
        public static byte GetRegA()
        {
            return registerA;
        }
        public static void SetRegB(byte b)
        {
            //Console.WriteLine("MAN SET REG B");
            registerB = b;
        }
        public static void SetRegB(UInt16 b)
        {
            registerB = memory[b]; //load value from memory and put it in registerB
        }
        public static byte GetRegB()
        {
            return registerB;
        }
        public static void SetOutReg(byte b)
        {
            outputReg = b;
        }
        public static byte GetOutReg()
        {
            return outputReg;
        }
        public static byte getValFromMem(UInt16 addr)
        {
            return memory[addr];
        }
        public static void saveValToMem(UInt16 addr)
        {
            memory[addr] = outputReg;
        }
        public static void ReBootRAM()
        {
            memory = new byte[65536];
        }
    }
}
