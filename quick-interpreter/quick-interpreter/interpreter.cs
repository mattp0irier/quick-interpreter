using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using DW.RtfWriter;

// Interpreter.cs: part of Quick by Matt Poirier and Trevor Russell

namespace quick_interpreter
{
    public class Interpreter : Statement.Visitor<object?>
    {
        // get an environment, implement vistor statements
        Environment global = new();

        // visitGenerateStatement: call generateRTF function to create files
        public object? visitGenerateStatement(GenerateStmt stmt)
        {
            // make a pretty file out of the test
            generateRTF(stmt.testName.lexeme, stmt.title.lexeme, global.GetTest(stmt.testName), stmt.quantity, stmt.shuffle);
            return null;
        }

        // visitTestStatement: call function to define a test in the environment
        public object? visitTestStatement(TestStmt stmt)
        {
            global.DefineTest(stmt.name, stmt.banks);
            return null;
        }

        // visitBankStatement: call function to define a bank in the environment
        public object? visitBankStatement(BankStmt stmt)
        {
            global.DefineBank(stmt.name, stmt.questions);
            return null;
        }

        // visitQuestionStatement: call function to add a question to a bank
        public object? visitQuestionStatement(QuestionStmt stmt)
        {
            global.AddToBank(stmt.bank, stmt.questions);
            return null;
        }

        // visitPrintStatement: call print function
        public object? visitPrintStatement(PrintStmt stmt)
        {
            print(stmt.itemToPrint);
            return null;
        }

        // visitDeleteStatement: call function to delete a question from a bank
        public object? visitDeleteStatement(DeleteStmt stmt)
        {
            global.DeleteQuestion(stmt.bank, stmt.index);
            return null;
        }

        // visitSetStatement: call function to update an answer of a question in a bank
        public object? visitSetStatement(SetStmt stmt)
        {
            global.updateAnswer(stmt.bank, stmt.index, stmt.answer);
            return null;
        }

        // Execute: call accept function of visitor statement
        public object? Execute(Statement stmt)
        {
            return stmt.accept(this);
        }

        // interpret: for each statement, call execute
        public void interpret(List<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                Execute(statement);
            }
        }

        // print: get questions from either test or bank and call corresponding function
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

        // printBank: output each question in a bank and its correct answer to the console
        public void printBank(Token name, List<Question> questions)
        {
            Console.WriteLine("Question Bank: " + name.lexeme); // output bank name
            for (int i = 0; i < questions.Count; i++) // loop through each question
            {
                Console.WriteLine("Question " + (i+1).ToString() + ": " + questions[i].problem.lexeme);
                switch (questions[i].type.type) // output each type of question properly
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

        // printTest: output each question in a test and its correct answer to the console
        public void printTest(Token name, List<Question> questions)
        {
            Console.WriteLine("Test: " + name.lexeme); // output test name
            for (int i = 0; i < questions.Count; i++) // loop through questions
            {
                Console.WriteLine("Question " + (i + 1).ToString() + ": " + questions[i].problem.lexeme);
                switch (questions[i].type.type) // output each type of question properly
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

        // shuffle: return new question list using randomList in random order
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

        // randomList: return a list of numbers from 0 to numQuestions-1 in a random order
        public List<int> randomList(int upper)
        {
            List<int> nums = new();
            nums.Add(RandomNumberGenerator.GetInt32(0, upper)); // call random number generator
            while (nums.Count != upper)
            {
                int randomNumber = RandomNumberGenerator.GetInt32(0, upper);
                if (!(nums.Contains(randomNumber))) // if it's not already in the list, add to list
                {
                    nums.Add(randomNumber);
                }
                else
                {
                    // otherwise, increment until a blank spot is found, then add that number there (similar to hash)
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

        // generateRTF: create RTF documents (both test and key) for each version of the test
        public void generateRTF(string fileName, string testName, List<Question>? questions, int quantity, bool isShuffle)
        {
            // check null questions
            if (questions == null)
            {
                Console.WriteLine("Test contains no questions. No file will be generated.");
                return;
            }

            int isKey = 0; // used to keep track of generating a key

            // if generating more than 1 test, put in subfolder
            string subfolder = "";
            if(quantity > 1)
            {
                subfolder = testName + "/";
                Directory.CreateDirectory("output/" + subfolder);
            }

            // loop through number of versions of test to generate
            for (int testNumber = 1; testNumber <= quantity; testNumber++)
            { 
                bool done = false;
                while (!done)
                {
                    // create a new RTF doc and variables
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

                    // test title
                    if (quantity != 1) // if more than one version of test, add Form [letter] to the title
                    {
                        if (isKey == 1) par.setText(testName + " Form " + Char.ToString((char)('A' + testNumber - 1)) + " KEY"); // if key, add KEY to the title
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
                    lineCounter = (lineCounter + 2) % 50; // update line counter

                    if ((isKey == 0) && isShuffle) questions = shuffle(questions); // shuffle questions

                    // loop through questions
                    for (int i = 0; i < questions.Count; i++)
                    {
                        if (pageOverflow(questions[i], lineCounter + 2) == 0) // check if page break needed
                        {
                            par = doc.addParagraph();
                            par = doc.addParagraph();
                            lineCounter = (lineCounter + 2) % 50;
                        }
                        else
                        {
                            while (lineCounter != 0) // add new lines until needed to perform page break
                            {
                                par = doc.addParagraph();
                                lineCounter = (lineCounter + 1) % 50;
                            }
                        }
                        //Console.WriteLine("Question " + i + " starts on line " + lineCounter);
                        par.Alignment = Align.FullyJustify; // fixes weird text issues

                        par.setText((i + 1) + ".\t" + questions[i].problem.lexeme); // write out problem

                        // loop through options
                        for (int j = 0; j < questions[i].options.Count; j++)
                        { 
                            par = doc.addParagraph();
                            par.Alignment = Align.FullyJustify;
                            par.setText("\t" + (char)(j + 65) + ".\t" + questions[i].options[j].lexeme); // letter for answer
                            lineCounter = (lineCounter + 1) % 50;

                            // bold answer on key
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
                            lineCounter = (lineCounter + 3) % 50; // update line counter
                        }

                        // lines for free response
                        if (questions[i].type.type == TokenType.FR)
                        {
                            par = doc.addParagraph();
                            lineCounter = (lineCounter + 1) % 50;

                            // add number of lines needed and update line counter
                            for (int k = 0; k < int.Parse(questions[i].solution.lexeme); k++)
                            {
                                par = doc.addParagraph();
                                par.setText("\t____________________________________________________________________________");
                                lineCounter = (lineCounter + 1) % 50;
                            }
                        }
                    }

                    // header
                    par = doc.Header.addParagraph();
                    par.addControlWord(1, RtfFieldControlWord.FieldType.Page);
                    par.Alignment = Align.Right;
                    par.DefaultCharFormat.FontSize = 12;

                    // save with correct title in subfolder
                    if (quantity != 1)
                    {
                        if (isKey == 1)
                        {
                            doc.save("output/" + subfolder + fileName + "-" + Char.ToString((char)('A' + testNumber - 1)) + "-key.rtf");
                        }
                        else
                        {
                            doc.save("output/" + subfolder + fileName + "-" + Char.ToString((char)('A' + testNumber - 1)) + ".rtf");
                        }
                    }
                    else
                    {
                        if (isKey == 1)
                        {
                            doc.save("output/" + subfolder + fileName + "-key.rtf");
                        }
                        else
                        {
                            doc.save("output/" + subfolder + fileName + ".rtf");
                        }
                    }

                    if (isKey == 1)
                    {
                        done = true;
                    }
                    isKey = (isKey + 1) % 2;
                }
            }
        }

        // returns amount of lines needed to create page break
        public int pageOverflow (Question question, int lineCount)
        {
           if (question.type.type == TokenType.FR)
            {
                if (lineCount + int.Parse(question.solution.lexeme)  < 50) return 0; // no page break needed
                else return 50 - lineCount; // return number of lines needed to get to new page
            }
           else if (question.type.type == TokenType.SA)
            {
                if (lineCount + 3 < 50) return 0; // no page break needed
                else return 50 - lineCount;
            }
            else
            {
                if (lineCount + question.options.Count < 50) return 0; // no page break needed
                else return 50 - lineCount;
            }
        }
    }
}
