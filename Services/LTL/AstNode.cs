using PetryNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Services.LTL
{
    public abstract class AstNode
    {
        public abstract bool Evaluate(List<TransitionModel> tokenHistory);
    }
}
