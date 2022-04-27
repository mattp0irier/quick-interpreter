using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quick_interpreter
{
    public class Statement
    {
        public interface Visitor<T>
        {
            T visitGenerateStatement(GenerateStmt stmt);
            T visitTestStatement(TestStmt stmt);
            T visitBankStatement(BankStmt stmt);
            T visitQuestionStatement(QuestionStmt stmt);
            T visitPrintStatement(PrintStmt stmt);
            T visitShuffleStatement(ShuffleStmt stmt);
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

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }

    public class TestStmt : Statement
    {
        // characteristics?
        public readonly Token name;
        public readonly List<Question> questions;

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
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
            return null;
        }
    }

    public class QuestionStmt : Statement
    {
        // characteristics?
        public readonly Token bank;
        public readonly Token type;
        public readonly List<string> options;
        public readonly int solution;

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }

    public class PrintStmt : Statement
    {
        // characteristics?
        readonly string msg;

        public PrintStmt(string msg)
        {
            this.msg = msg;
        }   

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }

    public class ShuffleStmt : Statement
    {
        // characteristics?
        // does this need any????

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }

    public class DeleteStmt : Statement
    {
        // characteristics?
        public readonly Token bank;
        public readonly int index;

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }

    public class SetStmt : Statement
    {
        // characteristics?
        public readonly Token bank;
        public readonly int index;
        public readonly Token answer;

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }
}
