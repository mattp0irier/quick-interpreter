using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quick_interpreter
{
    // Statement: abstract class for each type of statement in Quick
    public abstract class Statement
    {
        // visitor interface
        public interface Visitor<T>
        {
            T visitGenerateStatement(GenerateStmt stmt);
            T visitTestStatement(TestStmt stmt);
            T visitBankStatement(BankStmt stmt);
            T visitQuestionStatement(QuestionStmt stmt);
            T visitPrintStatement(PrintStmt stmt);
            T visitDeleteStatement(DeleteStmt stmt);
            T visitSetStatement(SetStmt stmt);
        }

        // accept<T>: to be written in subclasses
        public abstract T accept<T>(Visitor<T> visitor);

        // ToString: override tostring to return type of statement
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

    // GenerateStmt: class for generate statement
    public class GenerateStmt : Statement
    {
        // characteristics
        public readonly Token testName; // hold test name (token)
        public readonly Token title; // hold test title to be displayed on output
        public readonly int quantity; // quantity of tests to generate
        public readonly bool shuffle; // whether or not to randomize

        // constructor
        public GenerateStmt(Token testName, Token title, int quantity, bool shuffle)
        {
            this.testName = testName;
            this.title = title;
            this.quantity = quantity;
            this.shuffle = shuffle;
        }

        // accept
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitGenerateStatement(this);
        }
    }

    // TestStmt: class for test statement
    public class TestStmt : Statement
    {
        // characteristics
        public readonly Token name; // holds test name
        public readonly List<Token> banks; // holds banks to be included in test

        // constructor
        public TestStmt(Token name, List<Token> banks)
        {
            this.name = name;
            this.banks = banks;
        }

        // accept
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitTestStatement(this);
        }
    }

    // BankStmt: class for bank statement
    public class BankStmt : Statement
    {
        // characteristics
        public readonly Token name; // holds bank name
        public readonly List<Question> questions; // holds list of questions to be added to bank

        // constructor
        public BankStmt(Token name, List<Question> questions)
        {
            this.name = name;
            this.questions = questions;
        }

        // accept
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitBankStatement(this);
        }
    }

    // QuestionStmt: class for question statement
    public class QuestionStmt : Statement
    {
        // characteristics
        public readonly Token bank; // holds bank name to add question(s) to
        public readonly List<Question> questions; // holds questions to be added

        // constructor
        public QuestionStmt(Token bank, List<Question> questions)
        {
            this.bank = bank;
            this.questions = questions;
        }

        // accept
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitQuestionStatement(this);
        }
    }

    // PrintStmt: class for print statement
    public class PrintStmt : Statement
    {
        // characteristics
        public readonly Token itemToPrint; // token of item to print

        // constructor
        public PrintStmt(Token itemToPrint)
        {
            this.itemToPrint = itemToPrint;
        }   

        // accept
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitPrintStatement(this);
        }
    }

    // DeleteStmt: class for delete statment
    public class DeleteStmt : Statement
    {
        // characteristics
        public readonly Token bank; // bank to delete question from
        public readonly int index; // index of question to delete

        // constructor
        public DeleteStmt(Token bank, int index)
        {
            this.bank = bank;
            this.index = index;
        }

        // accept
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitDeleteStatement(this);
        }
    }

    // SetStmt: class for set statement
    public class SetStmt : Statement
    {
        // characteristics
        public readonly Token bank; // bank with question to update
        public readonly int index; // index of question to update
        public readonly Token answer; // new answer

        // constructor
        public SetStmt(Token bank, int index, Token answer)
        {
            this.bank = bank;
            this.index = index;
            this.answer = answer;
        }

        // accept
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitSetStatement(this);
        }
    }
}
