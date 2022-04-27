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

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }

    public class TestStmt : Statement
    {
        // characteristics?

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

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }

    public class DeleteStmt : Statement
    {
        // characteristics?

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }

    public class SetStmt : Statement
    {
        // characteristics?

        public override T accept<T>(Visitor<T> visitor)
        {
            return null;
        }
    }
}
