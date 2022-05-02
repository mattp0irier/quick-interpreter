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

        // Parser: constructor taking list of tokens
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        // Parse: parses tokens into statements
        public List<Statement> Parse()
        {
            List<Statement> statements = new();
            while (!AtEnd()) statements.Add(Stmt());
            return statements;
        }

        // Stmt: matches function based on keyword and calls appropriate parsing function accordingly
        Statement Stmt()
        {
            if (Match(TokenType.BANK)) return BankStmt();
            if (Match(TokenType.PRINT)) return PrintStmt();
            if (Match(TokenType.GENERATE)) return GenerateStmt();
            if (Match(TokenType.TEST)) return TestStmt();
            if (Match(TokenType.QUESTION)) return QuestionStmt();
            if (Match(TokenType.DELETE)) return DeleteStmt();
            if (Match(TokenType.SET_ANS)) return SetStmt();
            quick.Error(cur, "Please start a valid statement.");
            return null;
        }

        // GenerateStmt: parses generate statement
        Statement GenerateStmt()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect test name."); // consume test name
            Token title = Consume(TokenType.STRING, "Expect test title after test name."); // consume test title

            int quantity = 1;
            if (Check(TokenType.NUMBER)) // check for quantity of tests to generate
            {
                quantity = int.Parse(Advance().lexeme);
            }

            if (Check(TokenType.SHUFFLE)) // check if shuffle keyword used
            {
                Advance();
                Consume(TokenType.SEMICOLON, "Generate statement must end with semicolon."); // consume semicolon
                return new GenerateStmt(name, title, quantity, true);
            }
            else
            {
                Consume(TokenType.SEMICOLON, "Generate statement must end with semicolon."); // consume semicolon
                return new GenerateStmt(name, title, quantity, false);
            }
        }

        // TestStmt: parses test statement
        Statement TestStmt()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect test name."); // consume test name

            List<Token> banks = new();
            if (Match(TokenType.LEFT_BRACE)) // check for brace listing multiple banks
            {
                while (Check(TokenType.IDENTIFIER)) // consume banks while identifier next
                {
                    banks.Add(Advance());
                    if (Match(TokenType.RIGHT_BRACE)) break; // check for match to right brace indicating end
                    Consume(TokenType.COMMA, "Bank names are separated by a comma."); // consume comma separating banks
                }
            }
            else
            {
                // else one bank, so consume it
                banks.Add(Consume(TokenType.IDENTIFIER, "Test must include at least one question bank."));
            }
            Consume(TokenType.SEMICOLON, "Test delcaration must end with semicolon."); // consume semicolon


            return new TestStmt(name, banks);
        }

        // BankStmt: parses bank statement
        Statement BankStmt()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect bank name."); // consume bank name

            List<Question> questions = new();
            //May include questions with the declaration
            if (Match(TokenType.LEFT_BRACE))
            {
                while(!(Peek().GetType() == TokenType.RIGHT_BRACE)) // while questions, parse them
                {
                    questions.Add(ParseQuestion());
                    if (Peek().GetType() == TokenType.RIGHT_BRACE) break; // if right brace, break loop
                    Consume(TokenType.COMMA, "Questions must be separated by a comma"); // otherwise consume comma
                }
                Consume(TokenType.RIGHT_BRACE, "Question block must end with }"); // consume right brace
            }
            Consume(TokenType.SEMICOLON, "Bank Statement must end with ;"); // consume semicolon

            return new BankStmt(name, questions);
        }

        // QuestionStmt: parses question statement
        Statement QuestionStmt()
        {
            Token bankName = Consume(TokenType.IDENTIFIER, "Please include a bank name when adding questions."); // consume bank name

            List<Question> questions = new();
            //May include questions with the declaration
            if (Match(TokenType.LEFT_BRACE))
            {
                while (!(Peek().GetType() == TokenType.RIGHT_BRACE)) // loop while questions to parse
                {
                    questions.Add(ParseQuestion()); // call ParseQuestion
                    if (Check(TokenType.RIGHT_BRACE)) break; // break if right brace
                    Consume(TokenType.COMMA, "Questions must be separated by a comma"); // otherwise consume comma
                }
                Consume(TokenType.RIGHT_BRACE, "Question block must end with }"); // consume right brace
            }
            else
            {
                // Only one question
                questions.Add(ParseQuestion());
            }
            Consume(TokenType.SEMICOLON, "Question Statement must end with ;"); // consume semicolon

            return new QuestionStmt(bankName, questions);
        }

        // PrintStmt: generate print statement
        Statement PrintStmt()
        {
            Token itemToPrint = Consume(TokenType.IDENTIFIER, "Invalid identifier"); // consume token name to print
            Consume(TokenType.SEMICOLON, "Statements must end with a semicolon."); // consume semicolon
            return new PrintStmt(itemToPrint);
        }

        // DeleteStmt: generate delete statement
        Statement DeleteStmt()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect bank name."); // consume bank name
            Consume(TokenType.LEFT_BRACKET, "Expect left bracket."); // consume left bracket

            int index = int.Parse(Consume(TokenType.NUMBER, "Expect index").lexeme); // consume index
            Consume(TokenType.RIGHT_BRACKET, "Expect right bracket."); // consume right bracket

            Consume(TokenType.SEMICOLON, "Statements must end with a semicolon."); // consume semicolon

            return new DeleteStmt(name, index);
        }

        // SetStmt: generate set statement
        Statement SetStmt()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect bank name."); // consume bank name
            Consume(TokenType.LEFT_BRACKET, "Expect left bracket."); // consume left bracket

            int index = int.Parse(Consume(TokenType.NUMBER, "Expect index").lexeme); // consume index
            Consume(TokenType.RIGHT_BRACKET, "Expect right bracket."); // consume right bracket

            Token answer = null;
            if (Match(TokenType.T) || Match(TokenType.F) || Match(TokenType.NUMBER)) // match either true, false, or number
            {
                answer = Previous();
            }
            Consume(TokenType.SEMICOLON, "Statements must end with a semicolon."); // consume semicolon

            return new SetStmt(name, index, answer);
        }

        // ParseQuestion: parse each type of question properly
        Question ParseQuestion()
        {
            if (Check(TokenType.MC)) // if multiple choice
            {
                Token type = Advance(); // consume mc
                Token problem = Consume(TokenType.STRING, "Expect string for question."); // consume question
                List<Token> options = new();
                options.Add(Consume(TokenType.STRING, "Must provide at least one answer.")); // consume first answer
                while (Check(TokenType.STRING)) // while more answers, consume
                {
                    options.Add(Advance());
                }
                Token solution = Consume(TokenType.NUMBER, "Solution must be a number."); // consume index of answer
                if ((int)solution.GetLiteral() < 1 || (int)solution.GetLiteral() > options.Count()) // check number provided is valid
                {
                    Console.WriteLine("Solution must be in the range 1 - n, where n is the number of answers provided.");
                }
                return new Question(type, problem, options, solution);
            }
            else if (Check(TokenType.TF)) // if true/false
            {
                Token type = Advance(); // consume tf
                Token problem = Consume(TokenType.STRING, "Expect string for question."); // consume question
                List<Token> options = new();
                options.Add(new Token(TokenType.T, "True", "True", -1)); // create true option
                options.Add(new Token(TokenType.F, "False", "False", -1)); // create false option
                Token solution = Advance();
                if (solution.type != TokenType.T && solution.type != TokenType.F) // match solution as T or F
                {
                    Console.WriteLine("Answer to T/F question must be T or F."); // else error
                }
                return new Question(type, problem, options, solution);
            }
            else if (Check(TokenType.FR)) // if free response
            {
                Token type = Advance(); // consume fr
                Token problem = Consume(TokenType.STRING, "Expect string for question."); // consume question
                Token solution = Consume(TokenType.NUMBER, "Expect number for size of free response section."); // consume nubmer of lines
                return new Question(type, problem, new List<Token>(), solution);
            }
            else if (Check(TokenType.SA)) // if short answer
            {
                Token type = Advance(); // consume sa
                Token problem = Consume(TokenType.STRING, "Expect string for question."); // consume question
                if (Peek().GetType() == TokenType.STRING || Peek().GetType() == TokenType.NUMBER) // if other tokens, skip them
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
                // else, invalid question type --> error
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

    // Question: class for holding question
    public class Question
    {
        public readonly Token type;
        public readonly Token problem;
        public readonly List<Token> options;
        public Token solution;

        public Question()
        {
            Console.WriteLine("Garbage Question.");
        }

        // Question: constructor taking the type, the problem, a list of options, and the solution
        public Question(Token type, Token problem, List<Token> options, Token solution)
        {
            this.type = type;
            this.problem = problem;
            this.options = options;
            this.solution = solution;
        }
    }
}
