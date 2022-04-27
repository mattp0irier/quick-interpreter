using System;
using System.IO;
using System.Collections.Generic;

// Matthew Poirier and Trevor Russell, token.cs

namespace quick_interpreter
{
    // TokenType: enum for token types
    public enum TokenType
    {
        // One-chars
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACKET, RIGHT_BRACKET, LEFT_BRACE, RIGHT_BRACE,
        COMMA, SEMICOLON, T, F,

        // Literals
        IDENTIFIER, STRING, NUMBER,

        // Other Keywords
        PRINT, GENERATE, TEST, BANK, QUESTION, SHUFFLE, DELETE, SET_ANS, MC, TF, MT, SA, FR,

        EOF
    };

    // Token class
    public class Token
    {
        public TokenType type;
        public string lexeme;
        object literal;
        public int line;

        // Token: constructor for tokens
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        // GetType: return TokenType
        public new TokenType GetType()
        {
            return this.type;
        }

        // GetLiteral: return literal
        public new object GetLiteral()
        {
            return literal;
        }

        // toString: return string with type and lexeme and literal
        public string toString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }

}
