using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Services.LTL
{
    internal class BinaryOperatorNode : AstNode
    {
        public string Operator { get; private set; }
        public AstNode Left { get; private set; }
        public AstNode Right { get; private set; }

        public BinaryOperatorNode(string op, AstNode left, AstNode right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }
        public override bool Evaluate(List<TransitionModel> tokenHistory)
        {
            return Operator switch
            {
                "&" => Left.Evaluate(tokenHistory) && Right.Evaluate(tokenHistory),
                "|" => Left.Evaluate(tokenHistory) || Right.Evaluate(tokenHistory),
                _ => throw new NotSupportedException($"Unsupported operator: {Operator}")
            };
        }
    }
}
