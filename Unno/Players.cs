using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unno
{
    class Players
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public List<Cards> PlayersDeck { get; set; }

        public bool hasShuffled { get; set; }
        public int PointsScored { get; set; }
    }
}
