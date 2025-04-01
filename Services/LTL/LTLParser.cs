using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Services.LTL
{
    internal class LTLParser
    {
        private string _input;
        private int _position;

        public AstNode Parse( string input)
        {
            _input = input;
            _position = 0;
            return ParseExpression();
        }

        private AstNode ParseExpression()
        {
            var left = ParseTerm();
            while (Peek() == '|' || Peek() == '&')
            {
                var op = ReadOperator();
                var right = ParseTerm();
                left = new BinaryOperatorNode(op, left, right);
            }
            return left;
        }

        private AstNode ParseTerm()
        {
            if (Peek() == '!')
            {
                ReadChar(); // Consume '!'
                var operand = ParseTerm();
                return new UnaryOperatorNode("!", operand);
            }
            if (Peek() == 'F' || Peek() == 'G')
            {
                var op = ReadChar().ToString();
                ReadChar(); // Consume '('
                var operand = ParseExpression();
                ReadChar(); // Consume ')'
                return new UnaryOperatorNode(op, operand);
            }
            if (Peek() == '(')
            {
                ReadChar(); // Consume '('
                var expr = ParseExpression();
                ReadChar(); // Consume ')'
                return expr;
            }
            return new AtomicPropositionNode(ReadProposition());
        }
        private char Peek() => _position < _input.Length ? _input[_position] : '\0';

        private char ReadChar() => _input[_position++];

        private string ReadOperator()
        {
            return ReadChar().ToString();
        }

        private string ReadProposition()
        {
            var start = _position;
            while (char.IsLetterOrDigit(Peek()))
                ReadChar();
            return _input.Substring(start, _position - start);
        }
    }
}
