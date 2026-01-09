using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLibrary.Models
{
	public class Order
	{
		public int Id { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public DateTime OrderDate { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public decimal TotalAmount { get; set; }
	}
}
