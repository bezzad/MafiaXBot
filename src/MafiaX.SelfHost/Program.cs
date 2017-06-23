using System;
using MafiaX.BotEngine;

namespace MafiaX.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(@" 
   _______  _______  _______ _________ _______               ______   _______ _________
  (       )(  ___  )(  ____ \\__   __/(  ___  )  |\     /|  (  ___ \ (  ___  )\__   __/
  | () () || (   ) || (    \/   ) (   | (   ) |  ( \   / )  | (   ) )| (   ) |   ) (   
  | || || || (___) || (__       | |   | (___) |   \ (_) /   | (__/ / | |   | |   | |   
  | |(_)| ||  ___  ||  __)      | |   |  ___  |    ) _ (    |  __ (  | |   | |   | |   
  | |   | || (   ) || (         | |   | (   ) |   / ( ) \   | (  \ \ | |   | |   | |   
  | )   ( || )   ( || )      ___) (___| )   ( |  ( /   \ )  | )___) )| (___) |   | |   
  |/     \||/     \||/       \_______/|/     \|  |/     \|  |/ \___/ (_______)   )_(   
                                                                                       
                                                                                              
");
            var bot = new BotManager("379713905:AAEWC1UdFdz8tmmrVXCCgdpZvNQE3PIJqcM");
            bot.StartListening();

            Console.Write(new string('\n', 10));
            do
            {
                ClearCurrentConsoleLine();
                Console.Write("\tPlease enter 'Q' to exit: ");
            } while (Console.ReadLine() != "Q");
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop - 1;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}