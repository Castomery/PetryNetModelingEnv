using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Models
{
    public class TokenModel
    {
        //public int Id { get; private set; }
        //public PlaceModel parentPlace;
        public List<TransitionModel> History { get; private set; }

        public TokenModel()
        {
            //Id = id;
            //History = new List<TransitionModel>();
        }

        //public void AddTransitionToHistory(TransitionModel transition, List<TransitionModel> prevhistory)
        //{
        //    if (prevhistory.Count != 0)
        //    {
        //        History.AddRange(prevhistory);
        //    }
        //    History.Add(transition);
        //}
    }
}
