using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Lesson1
{

    class Token
    {

    }

    class Number : Token
    {
        public double Symbol;

        public Number(double num)
        {
            Symbol = num;
        }
    }

    class Operation : Token
    {
        public char Symbol;
        public int Priorety;
        public Operation(char symbol)
        {
            Symbol = symbol;
            Priorety = GetPriorety(symbol);
        }

        private int GetPriorety(char symbol)
        {
            Dictionary<object, int> prioretyDictionary = new Dictionary<object, int>
            {
                {'+', 1},
                {'-', 1},
                {'*', 2},
                {'/', 2},
                {'(', 0},
                {')', 5},
            };
            return prioretyDictionary[symbol];
        }
    }

    class Parenthesis : Token
    {
        public char Symbol;
        public bool IsClosing;
        public Parenthesis(char symbol)
        {
            if (symbol != '(' && symbol != ')')
                throw new ArgumentException("This is not valid bracket");

            IsClosing = symbol == ')';
            Symbol = symbol;
        }
    }

    class RPN
    {
        static void Main(string[] args)
        {
            Console.Write("Введите выражение: ");
            string str = Console.ReadLine();
            str = str.Replace(" ", string.Empty);
            var tokens = GetToken(str);
            var prn = PRN(tokens);
            Console.Write("Значение: ");
            Console.WriteLine(string.Join(" ", Result(prn)));
        }

        public static List<Token> GetToken(string str)
        {
            List<Token> tokens = new List<Token>();
            string num = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsDigit(str[i]) || str[i] == ',')
                {
                    num += str[i];
                }
                else if (str[i] == '+' || str[i] == '-' || str[i] == '*' || str[i] == '/')
                {
                    if (num != string.Empty)
                    {
                        tokens.Add(new Number(double.Parse(num)));
                        num = string.Empty;
                    }
                    tokens.Add(new Operation(str[i]));
                }
                else if (str[i] == '(' || str[i] == ')')
                {
                    if (num != string.Empty)
                    {
                        tokens.Add(new Number(double.Parse(num)));
                        num = string.Empty;
                    }
                    tokens.Add(new Parenthesis(str[i]));
                }
            }
            if (num != string.Empty)
            {
                tokens.Add(new Number(double.Parse(num)));
            }
            return tokens;
        }



        public static List<Token> PRN(List<Token> tokens)
        {
            List<Token> prn = new List<Token>();
            Stack<Token> stack = new Stack<Token>();
            foreach (Token token in tokens)
            {
                if (stack.Count == 0 && !(token is Number))
                {
                    stack.Push(token);
                    continue;
                }
                if (token is Operation)
                {
                    if (stack.Peek() is Parenthesis)
                    {
                        stack.Push(token);
                        continue;
                    }

                    Operation oper = (Operation)token;
                    Operation oper2 = (Operation)stack.Peek();
                    if (oper.Priorety > oper2.Priorety)
                    {
                        stack.Push(token);
                    }
                    else if (oper.Priorety <= oper2.Priorety)
                    {
                        while (stack.Count > 0 && !(token is Parenthesis))
                        {
                            prn.Add(stack.Pop());
                        }
                        stack.Push(token);
                    }
                }
                else if (token is Parenthesis)
                {
                    if (((Parenthesis)token).IsClosing)
                    {
                        while (!(stack.Peek() is Parenthesis))
                        {
                            prn.Add(stack.Pop());
                        }
                        stack.Pop();
                    }
                    else
                    {
                        stack.Push(token);
                    }
                }
                else if (token is Number)
                {
                    prn.Add(token);
                }
            }
            while (stack.Count > 0)
            {
                prn.Add(stack.Pop());
            }
            return prn;
        }

        public static Stack<double> Result(List<Token> expression)
        {
            Stack<double> stack = new Stack<double>();
            foreach (Token token in expression)
            {
                if (token is Number number)
                {
                    number = (Number)token;
                    stack.Push(number.Symbol);
                }
                else
                {
                    var second = stack.Pop();
                    var first = stack.Pop();
                    stack.Push(Calculate(token, first, second));
                }
            }
            return stack;
        }

        public static double Calculate(Token op, double first, double second)
        {
            var oper = (Operation)op;
            switch (oper.Symbol)
            {
                case '*': return first * second;
                case '/': return first / second;
                case '+': return first + second;
                case '-': return first - second;
                default: return double.NaN;
            }
        }
    }

}