using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver_Haffman
{
    public class Tree

    {
        public Char symbol;
        public int weight { get; set; }
        public Tree left { get; set; }
        public Tree right { get; set; }

        public int priznak;
        public Tree(Char symbol, int weight)
        {
            this.symbol = symbol;
            this.weight = weight;
        }

        public Tree(Char symbol, int weight, Tree left, Tree right)
        {
            this.symbol = symbol;
            this.weight = weight;
            this.left = left;
            this.right = right;
        }

        public Tree(char symbol, Tree left, Tree right)
        {
            this.symbol = symbol;
            this.left = left;
            this.right = right;
        }

        public Tree(Tree left, Tree right)
        {

            this.left = left;
            this.right = right;
        }

        public string getCodeForCharacter(char c, string f)
        {
            if (symbol == c)
            {
                return f;
            }

            else
            {
                if (left != null)
                {
                    string path = left.getCodeForCharacter(c, f + 1);
                    if (path != null) { return path; }
                }
                if (right != null)
                {
                    string path = right.getCodeForCharacter(c, f + 0);
                    if (path != null) { return path; }
                }

                return null;
            }
        }

        public bool IsLeaf()
        {
            if (right != null && left != null) { return false; }
            else { return true; }
        }

    }

}
