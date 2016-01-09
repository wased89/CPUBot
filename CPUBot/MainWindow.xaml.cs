using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DarkIrc;
using System.IO;

namespace CPUBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<String> instructionBuffer = new List<String>(); //the incoming instructions to be executed
        public static IrcConnection ircC;
        public String currUser;
        public String currChannel;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Console.WriteLine("BOOTING MEM");
            MemAndReg.ReBootRAM();
            Console.WriteLine("RAM BOOTED");

            Console.WriteLine("LOADING INSTRUCTION SET");
            Program.opcodes.Add("NOP", Functions.NOP);
            Program.opcodes.Add("NEG", Functions.NEG);
            Program.opcodes.Add("OR", Functions.OR);
            Program.opcodes.Add("AND", Functions.AND);
            Program.opcodes.Add("XOR", Functions.XOR);
            Program.opcodes.Add("ADD", Functions.ADD);
            Program.opcodes.Add("SUB", Functions.SUB);
            Program.opcodes.Add("MUL", Functions.MUL);
            Program.opcodes.Add("DIV", Functions.DIV);
            Program.opcodes.Add("MOD", Functions.MOD);
            Program.opcodes.Add("EQL", Functions.EQL);
            Program.opcodes.Add("LST", Functions.LST);
            Program.opcodes.Add("GRT", Functions.GRT);
            Program.opcodes.Add("IGT", Functions.IGT);
            Program.opcodes.Add("ILT", Functions.ILT);
            Program.opcodes.Add("IET", Functions.IET);
            Program.opcodes.Add("INE", Functions.INE);
            Program.opcodes.Add("DGT", Functions.DGT);
            Program.opcodes.Add("DLT", Functions.DLT);
            Program.opcodes.Add("DET", Functions.DET);
            Program.opcodes.Add("DNE", Functions.DNE);
            Program.opcodes.Add("MRF", Functions.MRF);
            Console.WriteLine("INSTRUCTION SET LOADED");

            Console.WriteLine("Starting IRCBot");
            ircC = new IrcConnection("irc.esper.net", "CPUBot");

            ircC.IrcEvents.DisconnectEvent += () =>
            {
                System.Threading.Thread.Sleep(1000);
                ircC.Connect();
            };
            ircC.IrcEvents.ChannelMessageEvent += (channel, user, message) =>
            {
                Console.WriteLine(channel + " <" + user + "> " + message);
                ProcessMessage(user, channel, message);
            };
            ircC.IrcEvents.PrivateMessageEvent += (user, message) =>
            {
                Console.WriteLine("<" + user + "> " + message);
                ProcessMessage(user, user, message);
            };

            //do bot joins and shit
            ircC.Connect();
            Console.WriteLine("Bot Started");
            System.Threading.Thread.Sleep(10000);
            ircC.IrcIO.JoinChannel("#FoxBotTest");
            ircC.IrcIO.JoinChannel("#bottorture");
            //ircC.IrcIO.JoinChannel("#kspmodders");
            Console.WriteLine(ircC.Connected);
        }
        public static void OUTPUT(String channel, String user, String message)
        {
            ircC.IrcIO.SendMessage(channel, user + ", OUTPUT FOR MEMADDR: " + message + " : " + Convert.ToString(MemAndReg.getValFromMem(Convert.ToUInt16(message, 16)), 2).PadLeft(8, '0'));
        }
        public void ProcessMessage(String user, String channel, String message)
        {
            if(message.StartsWith("OUTPUT "))
            {
                String msg = message.Substring(7);
                ircC.IrcIO.SendMessage(channel, user + ", Output for Memaddr: " + msg + " : " + Convert.ToString(MemAndReg.getValFromMem(Convert.ToUInt16(msg, 16)), 2).PadLeft(8, '0'));
                return;
            }
            if (message.Equals("EXIT") && user.Equals(Program.currUser))
            {
                Program.EXIT();
                ircC.IrcIO.SendMessage(channel, "DONE");
            }
            if(message.StartsWith("ADDINSTR "))
            {
                Program.ADDTESTINSTR(user, message.Substring(9));
                ircC.IrcIO.SendMessage(channel, "DONE");
                return;
            }
            if(message.Equals("DELINSTR"))
            {
                Program.DELTESTINSTR(user);
                ircC.IrcIO.SendMessage(channel, "DONE");
                return;
            }
            if(message.StartsWith("SVPROG "))
            {
                String progname = message.Substring(7);
                Console.WriteLine("Saving program: " + progname);
                Program.SVPROG(user, progname);
                ircC.IrcIO.SendMessage(channel, "DONE");
                return;
            }
            if(message.StartsWith("LDPROG ") && !Program.isRunning)
            {
                String progname = message.Substring(7);
                Console.WriteLine("Loading program: " + progname);
                Program.LDPROG(user, progname);
                ircC.IrcIO.SendMessage(channel, "DONE");
                return;
            }
            if(message.Equals("EMPTYPROG"))
            {
                Program.EMPTYTESTINSTR(user);
                ircC.IrcIO.SendMessage(channel, "DONE");
                return;
            }
            else if(message.StartsWith("LDPROG ") && Program.isRunning)
            {
                ircC.IrcIO.SendMessage(channel, user + ", A PROGRAM IS ALREADY RUNNING!");
                return;
            }
            if(message.Equals("RUN") && !Program.isRunning)
            {
                Console.WriteLine("Running program");
                Program.currUser = user;
                Program.RUNPROG();
                return;
            }
            else if(message.Equals("RUN") && Program.isRunning)
            {
                ircC.IrcIO.SendMessage(channel, user + ", A PROGRAM IS ALREADY RUNNING!");
                return;
            }
            if(message.Equals("TESTRUN"))
            {
                Console.WriteLine("Running test program");
                Program.currUser = user;
                Program.RUNTESTPROG();
                return;
            }
            if (message.Equals("~HELP"))
            {
                ircC.IrcIO.SendMessage(user, "My commands are: NOP, NEG, OR, AND, XOR, ADD, SUB, MUL, DIV, MOD, EQL, LST, GRT, " +
                    "IGT, ILT, IET, INE, DGT, DLT, DET, DNE, MRF, " + 
                    "OUTPUT <0000>. " +
                    "A and B take either 00 or 0000 in hex format. For single input functions, register B is used. " +
                    "Output is always formatted as 0000 in hex format and refers to a memory address. " +
                    "ADDINSTR <string> to add an instruction to your testprogram, DELINSTR to delete last instruction from your testprogram. " +
                    "RUN <progname> obviously runs your program. EMPTYPROG will empty your test program. When writing progs, the first line is ignored. " +
                    "EXIT exits the program.");
                return;
            }
            //do the string processing
            string[] strings = message.Split(' '); //split at teh spaces so we know the params
            if (!Program.opcodes.ContainsKey(strings[0])) { return; } //check if opcode exists

            if (Program.Execute(strings))//execute the opcode, returns false if error
            {
                ircC.IrcIO.SendMessage(channel, "Done");
            }
            else
            {
                ircC.IrcIO.SendMessage(channel, "Error, check inputs");
            }
        }
    }
}
