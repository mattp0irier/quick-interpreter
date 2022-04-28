using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quick_interpreter
{
    public class environment
    {
        public readonly Dictionary<string, List<Question>> banks = new();
        public readonly Dictionary<string, List<Question>> tests = new();

        public List<Question>? GetBank(Token name)
        {
            if (banks.ContainsKey(name.lexeme))
            {
                return banks[name.lexeme];
            }
            Console.WriteLine("Undefined bank: " + name.lexeme);
            return null;
        }

        public List<Question>? GetTest(Token name)
        {
            if (tests.ContainsKey(name.lexeme))
            {
                return tests[name.lexeme];
            }
            Console.WriteLine("Undefined test: " + name.lexeme);
            return null;
        }

        public void DefineBank(string name, List<Question> questions)
        {
            banks.Add(name, questions);
        }

        public void DefineTest(string name, List<Question> questions)
        {
            tests.Add(name, questions);
        }

        public void DeleteQuestion(Token name, int index)
        {
            List<Question>? questions = GetBank(name);
            if(questions != null && index < questions.Count)
            {
                questions.RemoveAt(index);
            }
            else
            {
                Console.WriteLine("question not found in test bank " + name.lexeme);
            }

        }

        public void updateAnswer(string name, int index, Token answer)
        {
            if (tests.ContainsKey(name))
            {
                if (index >= 0 && index < banks[name].Count)
                {
                    banks[name][index].solution = answer;
                }
                else
                {
                    Console.WriteLine("Test bank " + name + "has no question at index " + index);
                }
            }
            else
            {
                Console.WriteLine("Undefined bank: " + name);
            }
        }
    }
}
