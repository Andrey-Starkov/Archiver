using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Archiver_Haffman
{
    public class Haffman
    {

        static Tree Root { get; set; }
        public static Dictionary<char, int> Counting(string f) //Счёт количества каждого символа в файле, который нужно заархивировать
        {
            Dictionary<char, int> Count = new Dictionary<char, int> { [' '] = 0 };
            for (int i = 0; i < f.Length; i++)
            {
                char c = f[i];
                if (Count.ContainsKey(c) == false)
                {
                    Count.Add(c, 1);
                }
                else
                {
                    Count[c] = Count[c] + 1;
                }
            }
            return Count;
        }



        public static string WriteTree(Tree temp) //записывает дерево в файл
        {
            if (temp.IsLeaf()) return "1" + temp.symbol;
            return "0" + WriteTree(temp.left) + WriteTree(temp.right);
        }

        public static Tree huffman(List<Tree> Tree) //Формирование дерева
        {
            while (Tree.Count > 1)
            {
                List<Tree> SortTree = Tree.OrderBy(node => node.weight).ToList<Tree>(); //Сортировка по весу
                if (SortTree.Count >= 2)
                {
                    List<Tree> taken = SortTree.Take(2).ToList<Tree>(); //Берём из листа 2 самых маленьких веса

                    Tree parent = new Tree(default, taken[0].weight + taken[1].weight, taken[0], taken[1]); //Создаёт узел с суммой весов этих 2 элементов, у узла символ *

                    Tree.Remove(taken[0]); //Отчищает лист из двух элементов
                    Tree.Remove(taken[1]);
                    Tree.Add(parent); //Добавляет узел в дерево
                }

                Root = Tree.FirstOrDefault(); //Возвращает первый элемент дерева(ну по своеству, можно сказать, что присваивает дерево, другому дереву)
                //Root это возвращаемое дерево, уже отсортированное.
            }
            return Root;
        }


        public static Tree CreateTree(ref BinaryReader f) //Создаёт дерево из файла
        {
            int fchar;
            //char[] s;
            Tree temp = new Tree(default, null, null);
            fchar = f.ReadChar();
            //fchar = s[0];
            if (fchar == '1')
            {
                temp.symbol = Convert.ToChar(f.ReadChar());
               // Console.WriteLine(temp.symbol);
            }
            else
            {
                temp.left = CreateTree(ref f);
                temp.right = CreateTree(ref f);
            }
            return temp;

        }



        public static String huffmanDecode(String encoded, Tree tree) //Декодирование , encoded - строка, которую нужно закодировать
        {
            StringBuilder decode = new StringBuilder();
            Tree node = tree;
            for (int i = 0; i < encoded.Length; i++)
            {
                if (encoded[i] == '1') { node = node.left; } //Если текущий бит 0, то идём налево, иначе направо
                else { node = node.right; }
                if (node.symbol != default) //Если мы дошли до символа, а не до промежуточного узла
                {
                    decode.Append(node.symbol); //Добавление в декодированнную строку
                                                //  if (node.weight == '0') { decode.Append}
                    node = tree; //Возвращаемся к корню дерева
                }
            }
            return decode.ToString(); //Преобразует в строку из StreamBuilder
        }


        public static byte[] SaveToFile(string f, Tree TreeArchive, StringBuilder bits,string input,int leng) //Архивирование в файл
        {
            byte[] bytes = new byte[new FileInfo(input).Length];

            //string TreeString = Haffman.WriteTree(TreeArchive);
            //byte[] TreeBytes = new byte[TreeString.Length];
            //    for (int i = 0; i < TreeString.Length; i++)
            //      TreeBytes[i] = Convert.ToByte(TreeString[i]);

            using (StreamWriter Farch = new StreamWriter(f))
            {
                Farch.Write(Haffman.WriteTree(TreeArchive));
                Farch.Write(Convert.ToByte(leng));
            }

            using (BinaryWriter fArch = new BinaryWriter(File.OpenWrite(f)))
            {
                 fArch.Seek(0, SeekOrigin.End); //Начинает запись с конца файла
                string str = "";
                int index = 0;
                using (StreamReader finput = new StreamReader(input, true))
                {
                    int fchar; //символ из файла
                    while ((fchar = finput.Read()) != -1) //-1 означает конец файла
                    {
                        str += TreeArchive.getCodeForCharacter(Convert.ToChar(fchar), ""); //Строка нулей и единиц
                        int fchar2;
                        while ((str.Length % 8 != 0) && ((fchar2 = finput.Read()) != -1))
                        {
                            str += TreeArchive.getCodeForCharacter(Convert.ToChar(fchar2), "");
                        }
                        string str2 = "";
                        //int[] bytes = new int[8];
                        for (int bitss = 0; bitss < str.Length / 8; bitss++)
                        {
                            for (int j = bitss * 8; j < bitss * 8 + 8; j++)
                            {
                                str2 += str[j];
                            }
                            //int l = Convert.ToInt32(str2, 2); //Перевод из двоичной в десятичную
                            bytes[index] = Convert.ToByte(str2, 2); //Запись в бит

                            //fArch.WriteByte(bytes[index]);
                            fArch.Write(bytes[index]); //Запись байта в файл
                            index++;
                            str2 = "";
                        }
                        if (str.Length % 8 == 0)
                        {
                            str = "";
                        }
                    }

                }
                if (str.Length % 8 == 0) { }
                else
                {
                    int len = str.Length % 8;
                    str = str.Remove(0, str.Length - (str.Length % 8));
                    int sd = Convert.ToInt32(str, 2);
                    bytes[index] = Convert.ToByte(sd);
                   // fArch.WriteByte(bytes[index]);
                    fArch.Write(bytes[index]);
                }
            }





            return bytes;
        }





        public static Tree LoadFromFile(string input, Tree TreeArchive, ref String bits) //Возвращает строку закадированных битов и словарь с колличеством каждого символа
        {

            byte[] bytes = new byte[new FileInfo(input).Length];
            int index = 0;


            BinaryReader finput = new BinaryReader(File.OpenRead(input),Encoding.UTF8);
            int e;
            TreeArchive = CreateTree(ref finput);
            int count2 = finput.ReadChar() - 48;
            while (finput.BaseStream.Position != finput.BaseStream.Length)
            {
                e = finput.ReadByte();
                bytes[index] = Convert.ToByte(e);
                index++;
            }
            finput.Close();

            string str = "";
            StringBuilder str2 = new StringBuilder();
            int count = ((index * 8) - (8 - count2)) / 8;
            for (int i = 0; i < count; i++)
            {
                str += Convert.ToString((int)(bytes[i]), 2);
                for (int c = str.Length; c < 8; c++)
                {
                    str = '0' + str;
                }
                str2.Append(str);
                str = "";
            }
            str += Convert.ToString((int)(bytes[count]), 2);
            for (int c = str.Length; c < 8; c++)
            {
                str = '0' + str;
            }
            str = str.Remove(0,8-count2);
            str2.Append(str);
            bits = Convert.ToString(str2);


            return TreeArchive;
        }

    }
}

