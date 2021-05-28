using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
namespace Archiver_Haffman
{
    class Program
    {
        static void Main(string[] args)
        {

            Dictionary<char, int> Count;
            int a=0;
            while (a != 3)
            {
                Console.WriteLine("1. Заархивировать файл");
                Console.WriteLine("2. Разархивировать архив");
                Console.WriteLine("3. Закрыть консоль");
                a =Convert.ToInt32(Console.ReadLine());
                if (a == 1)
                {
                    Console.WriteLine("Введите путь к файлу, который нужно заархивировать");
                    string input = Console.ReadLine();
                    Console.WriteLine("Введите путь будущего архива");
                    string Archive = Console.ReadLine();
                    string f = File.ReadAllText(input); ;
                    Count = Haffman.Counting(f);
                    List<Tree> TreeList = new List<Tree>();
                    Tree inputTree;
                    foreach (KeyValuePair<char, int> keyValue in Count)
                    {
                        TreeList.Add(new Tree(keyValue.Key, keyValue.Value));

                    }

                    inputTree = Haffman.huffman(TreeList);


                    int len = 0;
                    string lenn;
                    foreach (KeyValuePair<char, int> keyValue in Count)
                    {
                        lenn = inputTree.getCodeForCharacter(keyValue.Key, "");
                        len += lenn.Length * keyValue.Value;
                       // Console.WriteLine(keyValue.Value);
                    }
                    StringBuilder encoded = new StringBuilder();
                    Haffman.SaveToFile(Archive, inputTree, encoded, input, len % 8);
                    Console.WriteLine("Архивация закончена");
                }


                if (a == 2)
                {
                    Console.WriteLine("Введите путь к архиву");
                    string Archive = Console.ReadLine();
                    Console.WriteLine("Введите путь будущего разархивированого файла");
                    string output = Console.ReadLine();
                    String endoyd = "";
                    Tree TreeArchive = new Tree(null, null);
                    TreeArchive = Haffman.LoadFromFile(Archive, TreeArchive, ref endoyd);
                    File.WriteAllText(output, Haffman.huffmanDecode(endoyd.ToString(), TreeArchive));
                    Console.WriteLine("Разархивация закончена");
                }
                Console.Clear();
            }

        }
    }
}
