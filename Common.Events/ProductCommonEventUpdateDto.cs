using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class ProductCommonEventUpdateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int ChangeQuantity { get; set; }
    }
}
