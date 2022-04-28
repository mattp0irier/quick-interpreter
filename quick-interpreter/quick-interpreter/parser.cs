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
            return null;
        }

        Statement GenerateStmt()
        {
            return new GenerateStmt();
        }

        Statement TestStmt()
        {
            return new TestStmt();
        }

        Statement BankStmt()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect bank name.");

            List<Question> questions = new();
            //May include questions with the declaration
            if (Match(TokenType.LEFT_BRACE))
            {
                while(!(Peek().GetType() == TokenType.RIGHT_BRACE))
                {
                    questions.Add(ParseQuestion());
                    if (Peek().GetType() == TokenType.RIGHT_BRACE) break;
                    Consume(TokenType.COMMA, "Questions must be separated by a comma");
                }
                Consume(TokenType.RIGHT_BRACE, "Question block must end with }");
            }
            Consume(TokenType.SEMICOLON, "Bank Statement must end with ;");

            return new BankStmt(name, questions);
        }

        Statement QuestionStmt()
        {
            Token bankName = Consume(TokenType.IDENTIFIER, "Please include a bank name when adding questions.");

            List<Question> questions = new();
            //May include questions with the declaration
            if (Match(TokenType.LEFT_BRACE))
            {
                while (!(Peek().GetType() == TokenType.RIGHT_BRACE))
                {
                    questions.Add(ParseQuestion());
                    if (Peek().GetType() == TokenType.RIGHT_BRACE) break;
                    Consume(TokenType.COMMA, "Questions must be separated by a comma");
                }
                Consume(TokenType.RIGHT_BRACE, "Question block must end with }");
            }
            else
            {
                // Only one question
                questions.Add(ParseQuestion());
            }
            Consume(TokenType.SEMICOLON, "Question Statement must end with ;");

            return new QuestionStmt(bankName, questions);
        }

        Statement PrintStmt()
        {
            Token itemToPrint = Consume(TokenType.IDENTIFIER, "Invalid identifier");
            return new PrintStmt(itemToPrint);
        }

        Statement DeleteStmt()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect bank name.");
            Consume(TokenType.LEFT_BRACKET, "Expect left bracket.");

            int index = int.Parse(Consume(TokenType.NUMBER, "Expect index").lexeme);
            Consume(TokenType.RIGHT_BRACKET, "Expect right bracket.");

            return new DeleteStmt(name, index);
        }

        Statement SetStmt()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect bank name.");
            Consume(TokenType.LEFT_BRACKET, "Expect left bracket.");

            int index = int.Parse(Consume(TokenType.NUMBER, "Expect index").lexeme);
            Consume(TokenType.RIGHT_BRACKET, "Expect right bracket.");

            Token answer = null;
            if (Match(TokenType.T) || Match(TokenType.F) || Match(TokenType.NUMBER))
            {
                answer = Previous();
            }

            return new SetStmt(name, index, answer);
        }

        Question ParseQuestion()
        {
            if (Check(TokenType.MC))
            {
                Token type = Advance();
                Token problem = Consume(TokenType.STRING, "Expect string for question.");
                List<Token> options = new();
                options.Add(Consume(TokenType.STRING, "Must provide at least one answer."));
                while (Check(TokenType.STRING))
                {
                    options.Add(Advance());
                }
                Token solution = Consume(TokenType.NUMBER, "Solution must be a number.");
                if ((int)solution.GetLiteral() < 1 || (int)solution.GetLiteral() > options.Count())
                {
                    Console.WriteLine("Solution must be in the range 1 - n, where n is the number of answers provided.");
                }
                return new Question(type, problem, options, solution);
            }
            else if (Check(TokenType.TF))
            {
                Token type = Advance();
                Token problem = Consume(TokenType.STRING, "Expect string for question.");
                List<Token> options = new();
                options.Add(new Token(TokenType.T, "True", "True", -1));
                options.Add(new Token(TokenType.F, "False", "False", -1));
                Token solution = Advance();
                if (solution.type != TokenType.T && solution.type != TokenType.F)
                {
                    Console.WriteLine("Answer to T/F question must be T or F.");
                }
                return new Question(type, problem, options, solution);
            }
            else if (Check(TokenType.FR))
            {
                Token type = Advance();
                Token problem = Consume(TokenType.STRING, "Expect string for question.");
                Token solution = Consume(TokenType.NUMBER, "Expect number for size of free response section.");
                return new Question(type, problem, new List<Token>(), solution);
            }
            else if (Check(TokenType.SA))
            {
                Token type = Advance();
                Token problem = Consume(TokenType.STRING, "Expect string for question.");
                if (Peek().GetType() == TokenType.STRING || Peek().GetType() == TokenType.NUMBER)
                {
                    Console.WriteLine("Improper number of arguments to Short Answer question, skipping extras");
                    while (Peek().GetType() == TokenType.STRING || Peek().GetType() == TokenType.NUMBER)
                    {
                        Advance();
                    }
                }
                return new Question(type, problem, new List<Token>(), new Token(TokenType.EOF, "", "", -1));
            }
            else
            {
                Console.WriteLine("Invalid question type");
                return new Question();
            }
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
        public readonly Token type;
        public readonly Token problem;
        public readonly List<Token> options;
        public readonly Token solution;

        public Question()
        {
            Console.WriteLine("Garbage Question.");
        }
        public Question(Token type, Token problem, List<Token> options, Token solution)
        {
            this.type = type;
            this.problem = problem;
            this.options = options;
            this.solution = solution;
        }
    }
}
