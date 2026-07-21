using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationLayer.Dto
{
    public class DtoOrderListItem
    {
        public string? OrderNumber { get; set; }
        public DtoOrder Order { get; set; } = null!;
        public IEnumerable<DtoOrderItem> OrderItems { get; set; } = Enumerable.Empty<DtoOrderItem>();
    }
}
