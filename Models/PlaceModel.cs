using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Models
{
    public class PlaceModel : NodeModel
    {
        public string Name { get; set; } = "P";
        public int TokenLimit { get; private set; }
        public List<TokenModel> Tokens { get; set; } = new List<TokenModel>();

        public PlaceModel(int id, string name) : base(id)
        {
            Name = name;
        }

        public void SetTokenLimit(int tokenLimit)
        {
            TokenLimit = tokenLimit;
        }

        public void AddToken(TokenModel token)
        {
            if (Tokens.Count >= TokenLimit) return; 
            if(Tokens.Contains(token)) return;

            Tokens.Add(token);
        }

        public void RemoveToken(TokenModel token)
        {
            if (!Tokens.Contains(token)) return;

            Tokens.Remove(token);
        }
    }
}
