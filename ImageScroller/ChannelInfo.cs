using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScroller
{
	public class ChannelInfo
	{
		public int ChannelNumber { get; set; }
		public Object ImageViewer { get; set; }
		public String FileBasePath { get; set; }
		public bool isSelected { get; set; }
        public bool cs_isSelected { get; set; }
    }	
}
