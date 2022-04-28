using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quick_interpreter
{
    public class Environment
    {
        public readonly Dictionary<string, List<Question>> banks = new();
        public readonly Dictionary<string, List<Question>> tests = new();

        public List<Question>? GetBank(Token name)
        {
            if (banks.ContainsKey(name.lexeme))
            {
                return banks[name.lexeme];
            }
           //Console.WriteLine("Undefined bank: " + name.lexeme);
            return null;
        }

        public List<Question>? GetTest(Token name)
        {
            if (tests.ContainsKey(name.lexeme))
            {
                return tests[name.lexeme];
            }
            //Console.WriteLine("Undefined test: " + name.lexeme);
            return null;
        }

        public void DefineBank(Token name, List<Question> questions)
        {
            banks.Add(name.lexeme, questions);

            /*debugging: output contents of question bank
            Console.WriteLine("Bank " + name.lexeme + ":");
            foreach (Question question in banks[name.lexeme])
            {
                Console.WriteLine(question.problem.lexeme);
            }
            */
        }

        public void AddToBank(Token name, List<Question> questions)
        {
            if (!(banks.ContainsKey(name.lexeme)))
            {
                Console.WriteLine("Bank " + name.lexeme + " does not exist.");
                return;
            }
            banks[name.lexeme].AddRange(questions);

            /* debugging: output updated bank
            Console.WriteLine("Bank " + name.lexeme + ":");
            foreach (Question question in banks[name.lexeme])
            {
                Console.WriteLine(question.problem.lexeme);
            }
            */
        }

        public void DefineTest(Token name, List<Token> bankList)
        {
            List<Question> questions = new();
            foreach (Token bank in bankList)
            {
                if (banks.ContainsKey(bank.lexeme)){
                    questions.AddRange(banks[bank.lexeme]);
                }
                else
                {
                    Console.WriteLine("Could not add bank " + bank.lexeme + "to this test.");
                }
            }
            tests.Add(name.lexeme, questions);
        }

        public void DeleteQuestion(Token name, int index)
        {
            List<Question>? questions = GetBank(name);
            if (questions == null)
            {
                Console.WriteLine("Bank " + name.lexeme + " does not exist.");
                return;
            }
            if(index <= questions.Count)
            {
                banks[name.lexeme].RemoveAt(index-1);
            }
            else
            {
                Console.WriteLine("question not found in bank " + name.lexeme);
            }

        }

        public void updateAnswer(Token name, int index, Token answer)
        {
            index = index - 1; // start numbering at 1
            List<Question>? questions = GetBank(name);
            if (questions == null)
            {
                Console.WriteLine("Bank " + name.lexeme + " does not exist.");
                return;
            }
            else
            {
                if (index >= 0 && index < banks[name.lexeme].Count)
                {
                    banks[name.lexeme][index].solution = answer;
                }
                else
                {
                    Console.WriteLine("Test bank " + name + "has no question at index " + index);
                }
            }
        }
    }
}
