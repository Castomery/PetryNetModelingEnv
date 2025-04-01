using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetryNet.Models
{
    public abstract class NodeModel
    {
        public int Id { get; private set; }

        protected NodeModel(int id)
        {
            Id = id;
        }
    }
}
