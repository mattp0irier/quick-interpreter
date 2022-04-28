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
            return null;
        }

        public object? visitTestStatement(TestStmt stmt)
        {
            Console.WriteLine("not yet implemented.");
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
            Console.WriteLine("not yet implemented.");
            return null;
        }

        public object? visitDeleteStatement(DeleteStmt stmt)
        {
            Console.WriteLine("not yet implemented.");
            return null;
        }

        public object? visitSetStatement(SetStmt stmt)
        {
            Console.WriteLine("not yet implemented.");
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
    }
}
