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
            
            return null;
        }
    }
}
