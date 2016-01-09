using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUBot
{
    public class Functions
    {
        public static byte NOP() { return 0; } //NAHP
        //public static byte NOT() { return !MemAndReg.GetRegB();}
        public static byte NEG() { return (byte)~MemAndReg.GetRegB(); } //NOT
        public static byte OR() { return (byte)(MemAndReg.GetRegA() | MemAndReg.GetRegB()); } //OR
        public static byte AND() { return (byte)(MemAndReg.GetRegA() & MemAndReg.GetRegB()); } //AND
        public static byte XOR() { return (byte)(MemAndReg.GetRegA() ^ MemAndReg.GetRegB()); } //XOR
        public static byte ADD() { return (byte)(MemAndReg.GetRegA() + MemAndReg.GetRegB()); } //Addition
        public static byte SUB() { return (byte)(MemAndReg.GetRegA() - MemAndReg.GetRegB()); } //subtraction
        public static byte MUL() { return (byte)(MemAndReg.GetRegA() * MemAndReg.GetRegB()); } //multiplication
        public static byte DIV() { return (byte)(MemAndReg.GetRegA() / MemAndReg.GetRegB()); } //division
        public static byte MOD() { return (byte)(MemAndReg.GetRegA() % MemAndReg.GetRegB()); } //modulus
        public static byte EQL() { if (MemAndReg.GetRegA() == MemAndReg.GetRegB()) return 1; else return 0; }
        public static byte LST() { if (MemAndReg.GetRegA() < MemAndReg.GetRegB()) return 1; else return 0; }
        public static byte GRT() { if (MemAndReg.GetRegA() > MemAndReg.GetRegB()) return 1; else return 0; }
        public static byte JMP() { Program.currPntr = MemAndReg.GetRegB();  return 0; }
        public static byte IGT() { if (MemAndReg.GetRegA() > MemAndReg.GetRegB()) Program.currPntr++; return 0; } //increments the inst pntr if greater than
        public static byte ILT() { if (MemAndReg.GetRegA() < MemAndReg.GetRegB()) Program.currPntr++; return 0; } //increments the instr pntr if less than
        public static byte IET() { if (MemAndReg.GetRegA() == MemAndReg.GetRegB()) Program.currPntr++; return 0; } //increments the instr pntr if equal
        public static byte INE() { if (MemAndReg.GetRegA() != MemAndReg.GetRegB()) Program.currPntr++;  return 0; } //incrments the instr pntr if not equal
        public static byte DGT() { if (MemAndReg.GetRegA() > MemAndReg.GetRegB()) Program.currPntr--; return 0; } //decrements the instr pntr if greater than
        public static byte DLT() { if (MemAndReg.GetRegA() < MemAndReg.GetRegB()) Program.currPntr--; return 0; } //decrements the intr pntr if lesser than
        public static byte DET() { if (MemAndReg.GetRegA() == MemAndReg.GetRegB()) Program.currPntr--; return 0; } //decrements the instr pntr if equal to
        public static byte DNE() { if (MemAndReg.GetRegA() != MemAndReg.GetRegB()) Program.currPntr--; return 0; } //decrements the instr pntr if equal to
        public static byte MRF() { MemAndReg.ReBootRAM(); return 0; }
        public static byte CON() { return 0; }
    }
}
