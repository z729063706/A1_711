using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shardLib
{
    public class RabinHash
    {
        private readonly int q;
        private readonly int d;
        private int h;

        public RabinHash(int q, int d)
        {
            this.q = q;
            this.d = d;
            this.h = 0;
        }

        public void Roll(byte b)
        {
            h = (h * d + b) % q;
        }

        public int Value
        {
            get { return h; }
        }
    }

}
