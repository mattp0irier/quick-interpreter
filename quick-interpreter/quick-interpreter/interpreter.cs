using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
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
            generateRTF(stmt.testName.lexeme, stmt.title.lexeme, global.GetTest(stmt.testName), stmt.quantity, stmt.shuffle);
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

        public List<Question>? shuffle(List<Question>? questions)
        {
            if (questions == null) return null;
            List<Question> newQuestions = new();
            List<int> newOrder = randomList(questions.Count);

            foreach (int num in newOrder) {
                newQuestions.Add(questions[num]);
            }

            return newQuestions;
        }

        public List<int> randomList(int upper)
        {
            List<int> nums = new();
            nums.Add(RandomNumberGenerator.GetInt32(0, upper));
            while (nums.Count != upper)
            {
                int randomNumber = RandomNumberGenerator.GetInt32(0, upper);
                if (!(nums.Contains(randomNumber)))
                {
                    nums.Add(randomNumber);
                }
                else
                {
                    int i = 1;
                    while (nums.Contains((randomNumber+i) % upper))
                    {
                        i++;
                    }
                    nums.Add((randomNumber + i) % upper);
                }
            }
            return nums;
        }

        public void generateRTF(string fileName, string testName, List<Question>? questions, int quantity, bool isShuffle)
        {
            if (questions == null)
            {
                Console.WriteLine("Test contains no questions. No file will be generated.");
                return;
            }

            int isKey = 0;
            for (int testNumber = 1; testNumber <= quantity; testNumber++)
            { 
                bool done = false;
                while (!done)
                {
                    var doc = new RtfDocument(PaperSize.Letter, PaperOrientation.Portrait, Lcid.English);
                    RtfParagraph par;
                    RtfCharFormat fmt;
                    int lineCounter = 0;

                    // margins
                    doc.Margins[Direction.Left] = 50;
                    doc.Margins[Direction.Top] = 50;
                    doc.Margins[Direction.Bottom] = 50;
                    doc.Margins[Direction.Right] = 50;

                    // header
                    par = doc.addParagraph();
                    par.Alignment = Align.FullyJustify;
                    par.setText("Name: ___________________________________\t\t\tDate: ________________");
                    par = doc.addParagraph();
                    par = doc.addParagraph();
                    if (quantity != 1)
                    {
                        if (isKey == 1) par.setText(testName + " Form " + Char.ToString((char)('A' + testNumber - 1)) + " KEY");
                        else par.setText(testName + " Form " + Char.ToString((char)('A' + testNumber - 1)));
                    }
                    else
                    {
                        if (isKey == 1) par.setText(testName + " KEY");
                        else par.setText(testName);
                    }
                    par.Alignment = Align.Center;
                    par.DefaultCharFormat.FontSize = 12;
                    fmt = par.addCharFormat();
                    fmt.FontStyle.addStyle(FontStyleFlag.Bold);
                    lineCounter += 2;

                    if ((isKey == 0) && isShuffle) questions = shuffle(questions);

                    // loop through questions
                    for (int i = 0; i < questions.Count; i++)
                    {
                        if (pageOverflow(questions[i], lineCounter + 2) == 0)
                        {
                            par = doc.addParagraph();
                            par = doc.addParagraph();
                            lineCounter = (lineCounter + 2) % 48;
                        }
                        else
                        {
                            while (lineCounter != 0)
                            {
                                par = doc.addParagraph();
                                lineCounter = (lineCounter + 1) % 48;
                            }
                        }
                        Console.WriteLine("Question " + i + " starts on line " + lineCounter);
                        par.Alignment = Align.FullyJustify;
                        par.setText((i + 1) + ".\t" + questions[i].problem.lexeme);
                        for (int j = 0; j < questions[i].options.Count; j++)
                        { 
                            par = doc.addParagraph();
                            par.LineSpacing = 15;
                            par.Alignment = Align.FullyJustify;
                            par.setText("\t" + (char)(j + 65) + ".\t" + questions[i].options[j].lexeme); // letter for answer
                            lineCounter = (lineCounter + 1) % 48;

                            // bold answer
                            if(isKey == 1 && (questions[i].type.type == TokenType.MC && ((j + 1 == int.Parse(questions[i].solution.lexeme))) || questions[i].type.type == TokenType.TF && questions[i].options[j].type == questions[i].solution.type))
                            {
                                fmt = par.addCharFormat();
                                fmt.FontStyle.addStyle(FontStyleFlag.Bold);
                            }
                        }

                        // space for short answer
                        if (questions[i].type.type == TokenType.SA)
                        {
                            par = doc.addParagraph();
                            par = doc.addParagraph();
                            par = doc.addParagraph();
                            lineCounter = (lineCounter + 3) % 48;
                        }

                        // lines for free response
                        if (questions[i].type.type == TokenType.FR)
                        {
                            par = doc.addParagraph();
                            lineCounter = (lineCounter + 1) % 48;
                            for (int k = 0; k < int.Parse(questions[i].solution.lexeme); k++)
                            {
                                par = doc.addParagraph();
                                //par.LineSpacing = 12;
                                par.setText("\t____________________________________________________________________________");
                                lineCounter = (lineCounter + 1) % 48;
                            }
                        }
                    }

                    // header
                    par = doc.Header.addParagraph();
                    par.addControlWord(1, RtfFieldControlWord.FieldType.Page);
                    par.Alignment = Align.Right;
                    par.DefaultCharFormat.FontSize = 12;

                    if(isKey == 1)
                    {
                        doc.save(fileName + "-" + Char.ToString((char)('A' + testNumber - 1)) + "-key.rtf");
                    }
                    else
                    {
                        doc.save(fileName + "-" + Char.ToString((char)('A' + testNumber - 1)) + ".rtf");
                    }
                    if (isKey == 1)
                    {
                        done = true;
                    }
                    isKey = (isKey + 1) % 2;
                }
            }
        }

        public int pageOverflow (Question question, int lineCount)
        {
           if (question.type.type == TokenType.FR)
            {
                if (lineCount + int.Parse(question.solution.lexeme)  < 48) return 0;
                else return 48 - lineCount;
            }
           else if (question.type.type == TokenType.SA)
            {
                if (lineCount + 3 < 48) return 0;
                else return 48 - lineCount;
            }
            else
            {
                if (lineCount + question.options.Count < 48) return 0;
                else return 48 - lineCount;
            }
        }
    }
}
