using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DW.RtfWriter;

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
            generateRTF(stmt.testName.lexeme, global.GetTest(stmt.testName), stmt.quantity);
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
                        Console.WriteLine("A: True\nB: False");
                        string answer = questions[i].solution.lexeme;
                        Console.WriteLine("Correct Answer: " + 
                            ((answer == "t" || answer == "T") ? "A" : "B"));
                        break;
                    case TokenType.SA:
                        Console.WriteLine("\n");
                        break;
                    case TokenType.FR:
                        for (int j=0; j < int.Parse(questions[i].solution.lexeme); j++){
                            Console.WriteLine();
                        }
                        break;
                }
            }
        }

        public void printTest(Token name, List<Question> questions)
        {
            Console.WriteLine("Test: " + name.lexeme);
            for (int i = 0; i < questions.Count; i++)
            {
                Console.WriteLine("Question " + (i + 1).ToString() + ": " + questions[i].problem.lexeme);
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
                        Console.WriteLine("A: True\nB: False");
                        string answer = questions[i].solution.lexeme;
                        Console.WriteLine("Correct Answer: " +
                            ((answer == "t" || answer == "T") ? "A" : "B"));
                        break;
                    case TokenType.SA:
                        Console.WriteLine("\n");
                        break;
                    case TokenType.FR:
                        for (int j = 0; j < int.Parse(questions[i].solution.lexeme); j++)
                        {
                            Console.WriteLine();
                        }
                        break;
                }
            }
        }

        public void generateRTF(string testName, List<Question>? questions, int quantity)
        {
            for(int testNumber = 1; testNumber <= quantity; testNumber++)
            {
                var doc = new RtfDocument(PaperSize.Letter, PaperOrientation.Portrait, Lcid.English);

                // margins
                doc.Margins[Direction.Left] = 50;
                doc.Margins[Direction.Top] = 50;
                doc.Margins[Direction.Bottom] = 50;
                doc.Margins[Direction.Right] = 50;
                RtfParagraph par;
                par = doc.addParagraph();
                par.DefaultCharFormat.Font = doc.createFont("Times New Roman");

                // loop through questions
                if (questions == null) return;
                for (int i = 0; i < questions.Count; i++)
                {
                    par = doc.addParagraph();
                    par.setText((i + 1) + ".\t" + questions[i].problem.lexeme);
                    for (int j = 0; j < questions[i].options.Count; j++)
                    {
                        par = doc.addParagraph();
                        par.LineSpacing = 15;
                        par.setText("\t" + (char)(j + 65) + ".\t" + questions[i].options[j].lexeme); // letter for answer
                    }

                    // space for short answer
                    if (questions[i].type.type == TokenType.SA)
                    {
                        par = doc.addParagraph();
                        par = doc.addParagraph();
                        par = doc.addParagraph();
                    }

                    // lines for free response
                    if (questions[i].type.type == TokenType.FR)
                    {
                        par = doc.addParagraph();
                        for (int k = 0; k < int.Parse(questions[i].solution.lexeme); k++)
                        {
                            par = doc.addParagraph();
                            par.LineSpacing = 25;
                            par.setText("\t___________________________________________________________________________");
                        }
                        par = doc.addParagraph();
                    }

                    par = doc.addParagraph();
                    par.setText("");
                }

                // header
                par = doc.Header.addParagraph();
                par.addControlWord(1, RtfFieldControlWord.FieldType.Page);
                par.Alignment = Align.Right;
                par.DefaultCharFormat.FontSize = 12;

                doc.save(testName + "-" + testNumber + ".rtf");
            }
        }
    }
}
