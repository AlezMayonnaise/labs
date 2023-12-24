using System;
using System.Collections.Generic;

public class RPNCalculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Введите математическое выражение: ");
            string expression = Console.ReadLine();

            try
            {
                double result = RPNCalculator.Calculate(expression);
                Console.WriteLine("Результат: " + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }
    public static double Calculate(string expression)
    {
        // Преобразование строки с математическим выражением в список токенов
        List<string> tokens = Tokenize(expression);

        // Преобразование списка токенов в ОПЗ
        List<string> rpnTokens = ConvertToRPN(tokens);

        // Вычисление результата выражения на основе списка токенов ОПЗ
        double result = EvaluateRPN(rpnTokens);

        return result;
    }

    private static List<string> Tokenize(string expression)
    {
        // Удаление пробелов из выражения
        expression = expression.Replace(" ", "");

        // Преобразование выражения в список токенов
        List<string> tokens = new List<string>();

        string currentToken = "";

        foreach (char c in expression)
        {
            if (char.IsDigit(c) || c == '.')    // Если символ является цифрой или точкой
            {
                currentToken += c;
            }
            else    // Если символ является оператором
            {
                if (!string.IsNullOrEmpty(currentToken))    // Если есть накопленное число, добавить его в список
                {
                    tokens.Add(currentToken);
                    currentToken = "";
                }

                tokens.Add(c.ToString());    // Добавить оператор в список
            }
        }

        if (!string.IsNullOrEmpty(currentToken))    // Если последний токен является числом, добавить его в список
        {
            tokens.Add(currentToken);
        }

        return tokens;
    }

    private static List<string> ConvertToRPN(List<string> tokens)
    {
        List<string> rpnTokens = new List<string>();
        Stack<string> operatorStack = new Stack<string>();

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out double operand))    // Если токен является числом
            {
                rpnTokens.Add(token);
            }
            else if (IsOperator(token))    // Если токен является оператором
            {
                while (operatorStack.Count > 0 && IsOperator(operatorStack.Peek()) && GetOperatorPrecedence(token) <= GetOperatorPrecedence(operatorStack.Peek()))
                {
                    rpnTokens.Add(operatorStack.Pop());
                }

                operatorStack.Push(token);
            }
            else if (token == "(")    // Если токен является открывающей скобкой
            {
                operatorStack.Push(token);
            }
            else if (token == ")")    // Если токен является закрывающей скобкой
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    rpnTokens.Add(operatorStack.Pop());
                }

                if (operatorStack.Count == 0 || operatorStack.Peek() != "(")
                {
                    throw new ArgumentException("Некорректное выражение: незакрытая скобка");
                }

                operatorStack.Pop();
            }
            else
            {
                throw new ArgumentException("Некорректное выражение: неподдерживаемый токен " + token);
            }
        }

        while (operatorStack.Count > 0)
        {
            if (operatorStack.Peek() == "(")
            {
                throw new ArgumentException("Некорректное выражение: незакрытая скобка");
            }

            rpnTokens.Add(operatorStack.Pop());
        }

        return rpnTokens;
    }

    private static bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    private static int GetOperatorPrecedence(string op)
    {
        if (op == "+" || op == "-")
        {
            return 1;
        }
        else if (op == "*" || op == "/")
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    private static double EvaluateRPN(List<string> rpnTokens)
    {
        Stack<double> operandStack = new Stack<double>();

        foreach (string token in rpnTokens)
        {
            if (double.TryParse(token, out double operand))    // Если токен является числом
            {
                operandStack.Push(operand);
            }
            else if (IsOperator(token))    // Если токен является оператором
            {
                if (operandStack.Count < 2)
                {
                    throw new ArgumentException("Некорректное выражение: недостаточно операндов для оператора " + token);
                }

                double operand2 = operandStack.Pop();
                double operand1 = operandStack.Pop();

                double result;

                if (token == "+")
                {
                    result = operand1 + operand2;
                }
                else if (token == "-")
                {
                    result = operand1 - operand2;
                }
                else if (token == "*")
                {
                    result = operand1 * operand2;
                }
                else if (token == "/")
                {
                    if (operand2 == 0)
                    {
                        throw new DivideByZeroException();
                    }

                    result = operand1 / operand2;
                }
                else
                {
                    throw new ArgumentException("Неподдерживаемый оператор " + token);
                }

                operandStack.Push(result);
            }
            else
            {
                throw new ArgumentException("Неподдерживаемый токен " + token);
            }
        }

        if (operandStack.Count != 1)
        {
            throw new ArgumentException("Некорректное выражение: лишние операнды");
        }

        return operandStack.Pop();
    }
}