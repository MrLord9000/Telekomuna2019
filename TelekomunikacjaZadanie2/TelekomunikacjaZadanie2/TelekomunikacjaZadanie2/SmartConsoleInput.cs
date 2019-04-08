using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelekomunikacjaZadanie2
{
    static class SmartConsoleInput
    {
        public static T ListSelect<T>(string _header, T[] list, string _footer)
        {
            int currentSelection = 0;
            ConsoleKeyInfo key;
            while (true)
            {
                Console.WriteLine(_header);
                for (int i = 0; i < list.Length; i++)
                {
                    if (currentSelection == i)
                        Console.Write(" <" + i + ".> ");
                    else
                        Console.Write("  " + i + ".  ");

                    Console.WriteLine(list[i]);
                }
                Console.WriteLine(_footer);
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (currentSelection == 0)
                        currentSelection = list.Length - 1;
                    else
                        currentSelection--;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (currentSelection == list.Length - 1)
                        currentSelection = 0;
                    else
                        currentSelection++;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    return list[currentSelection];
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return default(T);
                }
                Console.SetCursorPosition(0, Console.CursorTop - (list.Length + 2));
            }
            throw new IndexOutOfRangeException();
        }

        public static int ListSelectIndex(string _header, string[] list, string _footer)
        {
            int currentSelection = 0;
            ConsoleKeyInfo key;
            while (true)
            {
                Console.WriteLine(_header);
                for (int i = 0; i < list.Length; i++)
                {
                    if (currentSelection == i)
                        Console.Write(" <" + i + ".> ");
                    else
                        Console.Write("  " + i + ".  ");

                    Console.WriteLine(list[i]);
                }
                Console.WriteLine(_footer);
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (currentSelection == 0)
                        currentSelection = list.Length - 1;
                    else
                        currentSelection--;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (currentSelection == list.Length - 1)
                        currentSelection = 0;
                    else
                        currentSelection++;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    return currentSelection;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    return -1;
                }
                Console.SetCursorPosition(0, Console.CursorTop - (list.Length + 2));
            }
            throw new IndexOutOfRangeException();
        }

        public static bool TrueFalse(string _header, string _footer)
        {
            bool currentSelection = false;
            ConsoleKeyInfo key;
            while (true)
            {
                Console.WriteLine(_header);
                if (currentSelection)
                    Console.WriteLine(" <true>\t false ");
                else
                    Console.WriteLine("  true \t<false>");
                Console.WriteLine(_footer);

                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow)
                {
                    currentSelection = !currentSelection;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    return currentSelection;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    throw new IndexOutOfRangeException();
                }
                Console.SetCursorPosition(0, Console.CursorTop - 3);
            }
        }
    }
}
