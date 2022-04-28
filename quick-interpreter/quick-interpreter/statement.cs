using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quick_interpreter
{
    public abstract class Statement
    {
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

        public abstract T accept<T>(Visitor<T> visitor);
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

    public class GenerateStmt : Statement
    {
        // characteristics?
        public readonly Token testName;
        public readonly int quantity;
        public readonly bool shuffle;

        public GenerateStmt(Token testName, int quantity, bool shuffle)
        {
            this.testName = testName;
            this.quantity = quantity;
            this.shuffle = shuffle;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitGenerateStatement(this);
        }
    }

    public class TestStmt : Statement
    {
        // characteristics?
        public readonly Token name;
        public readonly List<Token> banks;

        public TestStmt(Token name, List<Token> banks)
        {
            this.name = name;
            this.banks = banks;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitTestStatement(this);
        }
    }

    public class BankStmt : Statement
    {
        // characteristics?
        public readonly Token name;
        public readonly List<Question> questions;

        public BankStmt(Token name, List<Question> questions)
        {
            this.name = name;
            this.questions = questions;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitBankStatement(this);
        }
    }

    public class QuestionStmt : Statement
    {
        // characteristics?
        public readonly Token bank;
        public readonly List<Question> questions;

        public QuestionStmt(Token bank, List<Question> questions)
        {
            this.bank = bank;
            this.questions = questions;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitQuestionStatement(this);
        }
    }

    public class PrintStmt : Statement
    {
        // characteristics?
        public readonly Token itemToPrint;

        public PrintStmt(Token itemToPrint)
        {
            this.itemToPrint = itemToPrint;
        }   

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitPrintStatement(this);
        }
    }

    public class DeleteStmt : Statement
    {
        // characteristics?
        public readonly Token bank;
        public readonly int index;

        public DeleteStmt(Token bank, int index)
        {
            this.bank = bank;
            this.index = index;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitDeleteStatement(this);
        }
    }

    public class SetStmt : Statement
    {
        // characteristics?
        public readonly Token bank;
        public readonly int index;
        public readonly Token answer;

        public SetStmt(Token bank, int index, Token answer)
        {
            this.bank = bank;
            this.index = index;
            this.answer = answer;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitSetStatement(this);
        }
    }
}
