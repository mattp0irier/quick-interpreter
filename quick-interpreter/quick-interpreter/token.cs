using System;
using System.IO;
using System.Collections.Generic;

namespace trmpLox
{
    // TokenType: enum for token types
    public enum TokenType
    {
        // One-chars
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

        // Comparitors and Equal
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Literals
        IDENTIFIER, STRING, NUMBER,

        // Other Keywords
        AND, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
        PRINT, RETURN, THIS, TRUE, VAR, WHILE,

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
