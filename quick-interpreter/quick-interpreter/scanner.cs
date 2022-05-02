using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// scanner.cs: part of Quick by Matt Poirier and Trevor Russell

namespace quick_interpreter
{
    public class Scanner
    {
        String source;
        List<Token> tokens = new List<Token>();
        int start = 0;
        int cur = 0;
        int line = 1;

        // create hash table of keywords
        readonly Hashtable keywords = new Hashtable()
        {
            {"print", TokenType.PRINT},
            {"generate", TokenType.GENERATE},
            {"test", TokenType.TEST},
            {"bank", TokenType.BANK},
            {"question", TokenType.QUESTION},
            {"shuffle", TokenType.SHUFFLE},
            {"delete", TokenType.DELETE},
            {"set_ans", TokenType.SET_ANS},
            {"mc", TokenType.MC},
            {"tf", TokenType.TF},
            {"sa", TokenType.SA},
            {"fr", TokenType.FR},
        };

        // Scanner: create scanner with source string (the code)
        public Scanner(String source)
        {
            this.source = source;
        }

        // scanTokens: while the file is not at end, scan a token
        public List<Token> scanTokens()
        {
            while (!done())
            {
                start = cur;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        // scanToken: advance token, check character type, and match to correct token type
        void scanToken()
        {
            char c = Advance();
            switch (c)
            {
                //SINGLE-CHARACTER OPERATIONS
                case '(':
                    addToken(TokenType.LEFT_PAREN); break;
                case ')':
                    addToken(TokenType.RIGHT_PAREN); break;
                case '{':
                    addToken(TokenType.LEFT_BRACE); break;
                case '}':
                    addToken(TokenType.RIGHT_BRACE); break;
                case '[':
                    addToken(TokenType.LEFT_BRACKET); break;
                case ']':
                    addToken(TokenType.RIGHT_BRACKET); break;
                case ',':
                    addToken(TokenType.COMMA); break;
                case ';':
                    addToken(TokenType.SEMICOLON); break;
                case 't':
                case 'T':
                    if (Char.IsLetterOrDigit(Peek()) || Peek() == '_')
                    {
                        ScanIdentifier();
                    }
                    else
                    {
                        addToken(TokenType.T);
                    }
                    break;
                case 'f':
                case 'F':
                    if (Char.IsLetterOrDigit(Peek()) || Peek() == '_')
                    {
                        ScanIdentifier();
                    }
                    else
                    {
                        addToken(TokenType.F);
                    }
                    break;


                //single slash is a comment
                case '/':
                    // don't read the rest of the line if it's a comment
                    while (Peek() != '\n' && !done()) Advance(); break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '"':
                    ScanString();
                    break;
                default:
                    if (Char.IsDigit(c))
                        ScanNumber();
                    else if (Char.IsLetter(c) || c == '_')
                        ScanIdentifier();
                    else
                        quick.Error(line, "Unexpected Character."); break;

            }
        }

        // ScanIdentifier: match keywords as identifier
        void ScanIdentifier()
        {
            while (Char.IsLetterOrDigit(Peek()) || Peek() == '_') Advance();

            string name = source[start..cur];
            TokenType type;
#pragma warning disable CS8605 // Unboxing a possibly null value.
            if (keywords.ContainsKey(name)) type = (TokenType)keywords[name];
#pragma warning restore CS8605 // Unboxing a possibly null value.
            else type = TokenType.IDENTIFIER;

            addToken(type);
        }

        // ScanNumber: while there are digits, add them to number, then save as number token
        void ScanNumber()
        {
            while (Char.IsDigit(Peek())) Advance();

            addToken(TokenType.NUMBER, Int32.Parse(source[start..cur]));
        }

        // ScanString: while there are not end quotes or new lines, add to part of string
        void ScanString()
        {
            while (Peek() != '"' && !done())
            {
                if (Peek() == '\n') line++;
                Advance();
            }
            if (done())
            {
                quick.Error(line, "Unterminated String");
                return;
            }
            Advance();

            string value = source[(start + 1)..(cur - 1)];
            addToken(TokenType.STRING, value);
        }

        // match: match a character to a provided value
        bool match(char test)
        {
            if (done()) return false;
            if (source[cur] != test) return false;
            cur++;
            return true;
        }

        // Peek: return current character if file not done
        char Peek()
        {
            if (done()) return '\0';
            return source[cur];
        }

        // PeekNext: return next character
        char PeekNext()
        {
            return source[cur + 1];
        }

        // Advance: return next character and advance cur
        char Advance()
        {
            return source[cur++];
        }

        // addToken: add token of certain TokenType
        void addToken(TokenType type)
        {
            addToken(type, null);
        }

        // addToken: add token of certain TokenType with its literal
        void addToken(TokenType type, object literal)
        {
            string text = source[start..cur];
            if (type == TokenType.STRING)
            {
                text = source[(start + 1)..(cur - 1)];
            }
            tokens.Add(new Token(type, text, literal, line));
        }

        // done: return if file is < the source length
        bool done()
        {
            return cur >= source.Length;
        }
    }
}