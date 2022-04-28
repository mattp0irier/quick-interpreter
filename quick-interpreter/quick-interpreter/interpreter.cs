using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quick_interpreter
{
    public class Interpreter : Statement.Visitor<object?>
    {
        // get an environment, implement vistor statements
        Environment global = new();

        public object? visitGenerateStatement(GenerateStmt stmt)
        {
            // this is where formatting is a big deal
            // make a pretty file out of the test
            Console.WriteLine("FIXME: Implement GenerateStatement");
            return null;
        }

        public object? visitTestStatement(TestStmt stmt)
        {
            global.DefineTest(stmt.name, stmt.banks);
            return null;
        }

        public object? visitBankStatement(BankStmt stmt)
        {
            global.DefineBank(stmt.name, stmt.questions);
            return null;
        }

        public object? visitQuestionStatement(QuestionStmt stmt)
        {
            global.AddToBank(stmt.bank, stmt.questions);
            return null;
        }

        public object? visitPrintStatement(PrintStmt stmt)
        {
            print(stmt.itemToPrint);
            return null;
        }

        public object? visitDeleteStatement(DeleteStmt stmt)
        {
            global.DeleteQuestion(stmt.bank, stmt.index);
            return null;
        }

        public object? visitSetStatement(SetStmt stmt)
        {
            global.updateAnswer(stmt.bank, stmt.index, stmt.answer);
            return null;
        }

        public object? Execute(Statement stmt)
        {
            return stmt.accept(this);
        }

        public void interpret(List<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                Execute(statement);
            }
        }
        public void print(Token name)
        {
            List<Question>? questions = global.GetTest(name);
            if (questions == null)
            {
                questions = global.GetBank(name);
                if (questions == null)
                {
                    Console.WriteLine("No test or bank with name " + name.lexeme + " exists.");
                    return;
                }
                else
                {
                    printBank(name, questions);
                }
            }
            else
            {
                printTest(name, questions);
            }
        }

        public void printBank(Token name, List<Question> questions)
        {
            // FIXME: Bad Formatting
            Console.WriteLine("Question Bank: " + name.lexeme);
            for (int i = 0; i < questions.Count; i++)
            {
                Console.WriteLine("Question " + (i+1).ToString() + ": " + questions[i].problem.lexeme);
                switch (questions[i].type.type)
                {
                    case TokenType.MC:
                        char option = 'A';
                        for (int j = 0; j < questions[i].options.Count; j++)
                        {
                            Console.WriteLine(Char.ToString((char)(option + j)) +
                                ": " + questions[i].options[j].lexeme);
                        }
                        Console.WriteLine("Correct Answer: " + Char.ToString((char)(option + int.Parse(questions[i].solution.lexeme) - 1)));
                        break;
                    case TokenType.TF:
                        Console.WriteLine("TF");
                        break;
                    case TokenType.SA:
                        Console.WriteLine("SA");
                        break;
                    case TokenType.FR:
                        Console.WriteLine("FR");
                        break;
                }
            }
        }

        public void printTest(Token name, List<Question> questions)
        {
            Console.WriteLine("printTest not done yet.");
        }
    }
}
