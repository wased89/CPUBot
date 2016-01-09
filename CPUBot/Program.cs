using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CPUBot
{
    public class Program
    {
        public static Dictionary<String, Func<byte>> opcodes = new Dictionary<String, Func<byte>>();
        public static String[] currProg;
        public static Dictionary<String, List<String>> testProg = new Dictionary<string, List<string>>();
        public static byte currPntr = 0;
        public static bool isRunning = false;
        public static String currUser;
        public static String currChannel;
        
        public static void EMPTYTESTINSTR(String user)
        {
            Console.WriteLine("Emptying " + user + "'s test instructions");
            testProg[user].Clear();
        }
        public static void ADDTESTINSTR(String user, String instr)
        {
            if(!testProg.ContainsKey(user))
            {
                testProg.Add(user, new List<string>());
            }
            testProg[user].Add(instr);
        }
        public static void DELTESTINSTR(String user)
        {
            if(!testProg.ContainsKey(user))
            {
                MainWindow.ircC.IrcIO.SendMessage(currChannel, user + ", NO TEST PROGRAM LOADED.");
            }
            testProg[user].RemoveAt(testProg.Count-1);
        }
        public static void LDPROG(String user, String progname)
        {
            try
            {
                currProg = File.ReadAllLines("/Programs/" + user + "/" + progname + ".txt");
                MainWindow.ircC.IrcIO.SendMessage(currChannel, user + ", PROGRAM: " + progname + " LOADED.");
            }
            catch(Exception e)
            {
                MainWindow.ircC.IrcIO.SendMessage(currChannel, user + ", ERROR LOADING PROGRAM: " + progname);
            }
            
        }
        public static void SVPROG(String user, String progname)
        {
            Console.WriteLine("Saving program: " + progname);
            if(!Directory.Exists("/Programs/" + user))
            {
                Console.WriteLine("User not found, making new directory");
                Directory.CreateDirectory("/Programs/" + user + "/");
            }
            if(File.Exists("/Programs/" + user + "/" + progname + ".txt"))
            {
                File.Delete("/Programs/" + user + "/" + progname + ".txt");
            }
            try
            {
                File.WriteAllLines("/Programs/" + user + "/" + progname + ".txt", testProg[user]);
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception writing file!: " + e);
                MainWindow.ircC.IrcIO.SendMessage(currChannel, user + ", ERROR SAVING PROGRAM: " + progname);
                return;
            }
            MainWindow.ircC.IrcIO.SendMessage(currChannel, user + ", SUCCESS: " + progname);
        }
        public static void RUNPROG()
        {
            isRunning = true;
            MainWindow.ircC.IrcIO.SendMessage(currChannel, currUser + "PROGRAM HAS STARTED");
            while (isRunning)
            {
                if(!EXENEXT(currProg[currPntr]))
                {
                    MainWindow.ircC.IrcIO.SendMessage(currChannel, currUser + "PROGRAM HAS TERMINATED WITH AN ERROR, LINE: " + currPntr--);
                }
            }
            MainWindow.ircC.IrcIO.SendMessage(currChannel, currUser + "PROGRAM HAS TERMINATED SUCCESSFULLY");
        }
        public static void RUNTESTPROG()
        {
            isRunning = true;
            MainWindow.ircC.IrcIO.SendMessage(currChannel, currUser + "TEST PROGRAM HAS STARTED");
            while (isRunning)
            {
                currPntr++;
                if (!EXENEXT(testProg[currUser][currPntr]))
                {
                    MainWindow.ircC.IrcIO.SendMessage(currChannel, currUser + "PROGRAM HAS TERMINATED WITH AN ERROR, LINE: " + currPntr--);
                }
            }
            MainWindow.ircC.IrcIO.SendMessage(currChannel, currUser + "PROGRAM HAS TERMINATED SUCCESSFULLY");
        }
        public static bool EXENEXT(String line)
        {
            //do the string processing
            string[] strings = line.Split(' '); //split at teh spaces so we know the params
            if (!Program.opcodes.ContainsKey(strings[0])) { return false; } //check if opcode exists

            if (Program.Execute(strings))//execute the opcode, returns false if error
            {
                return true;
            }
            else
            {
                MainWindow.ircC.IrcIO.SendMessage(currChannel, "Error, check inputs");
                return false;
            }
        }
        public static bool Execute(string[] strings)
        {
            String opcode = strings[0];
            
            if(opcode == "EXIT") //programs are terminated with EXIT
            {
                EXIT();
                return true;
            }
            if(opcode == "OUTPUT")
            {
                MainWindow.OUTPUT(currChannel, currUser, strings[1]); //outputs a memory address
            }
            if (opcode != "NOP")//NOP is a 0 param instruction, so dont do this shit if it's a NOP
            {
                //we check to see if the inputs are literals or memory addresses
                if (strings[1].Length == 2) //literal
                {
                    MemAndReg.SetRegA(Convert.ToByte(strings[1], 16));
                }
                else if (strings[1].Length == 4) //memory address
                {
                    MemAndReg.SetRegA(Convert.ToUInt16(strings[1], 16));
                }
                else
                {
                    //error, invalid length
                    return false;
                }

                if (strings[2].Length == 2) //literal
                {
                    MemAndReg.SetRegB(Convert.ToByte(strings[2], 16));
                }
                else if (strings[2].Length == 4) //memory address
                {
                    MemAndReg.SetRegB(Convert.ToUInt16(strings[2], 16));
                }
                else
                {
                    //error, invalid length
                    return false;
                }
                //execute the opcode and then return the result to the output register
                MemAndReg.SetOutReg(opcodes[opcode]());
                try
                {
                    MemAndReg.saveValToMem(Convert.ToUInt16(strings[3], 16));
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                opcodes[opcode](); //execute the nop
            }
            return true;
        }
        public static void EXIT()
        {
            isRunning = false;
            currPntr = 0;
            currProg = new String[0];
        }
    }
}
