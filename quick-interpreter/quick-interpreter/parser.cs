using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quick_interpreter
{
    public class Parser
    {
        readonly List<Token> tokens;
        int cur = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public List<Statement> Parse()
        {
            List<Statement> statements = new();
            while (!AtEnd()) statements.Add(Stmt());
            return statements;
        }

        Statement Stmt()
        {
            if (Match(TokenType.BANK)) return BankStmt();
            if (Match(TokenType.PRINT)) return PrintStmt();
            if (Match(TokenType.GENERATE)) return GenerateStmt();
            if (Match(TokenType.TEST)) return TestStmt();
            if (Match(TokenType.QUESTION)) return QuestionStmt();
            if (Match(TokenType.SHUFFLE)) return ShuffleStmt();
            if (Match(TokenType.DELETE)) return DeleteStmt();
            if (Match(TokenType.SET_ANS)) return SetStmt();
            quick.Error(cur, "Please start a valid statement.");
            return new PrintStmt("");
        }

        Statement GenerateStmt()
        {

        }

        Statement TestStmt()
        {

        }

        Statement BankStmt()
        {

        }

        Statement QuestionStmt()
        {

        }

        Statement PrintStmt()
        {

        }

        Statement ShuffleStmt()
        {

        }

        Statement DeleteStmt()
        {

        }

        Statement SetStmt()
        {

        }


        // Match: match token to type
        bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types) // loop through token types sought to be matched
            {
                if (Check(type)) // check for match
                {
                    Advance(); // advance index
                    return true;
                }
            }
            return false; // return false if no match
        }

        // Consume: match token else throw error
        Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance(); // if match token, advance

            Console.WriteLine(message);
            return Peek();
        }

        // Check: check eof, else return type of token
        bool Check(TokenType type)
        {
            if (AtEnd()) return false;
            return Peek().GetType() == type; // return if token matches type
        }

        // Advance: if not EOF, advance index and return previous token
        Token Advance()
        {
            if (!AtEnd()) cur++;
            return Previous();
        }

        // AtEnd: check for EOF token
        bool AtEnd()
        {
            return Peek().GetType() == TokenType.EOF;
        }

        // Peek: return token at current position
        Token Peek()
        {
            return tokens[cur];
        }

        // Previous: return token at previous position
        Token Previous()
        {
            return tokens[cur - 1];
        }
    }

    public class Question
    {
        public readonly string type;
        public readonly string problem;
        public readonly List<string> options;
        public readonly int solution;

        public Question(string type, string problem, List<string> options, int solution)
        {
            this.type = type;
            this.problem = problem;
            this.options = options;
            this.solution = solution;
        }
    }
}
