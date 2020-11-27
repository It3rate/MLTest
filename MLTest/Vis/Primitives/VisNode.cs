using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
	public class VisNode : IPrimitive, IJoiner
    {
		public VisElement Reference { get; }
		public float Position { get; }
		public float Offset { get; }

		public Point Anchor { get; }

		public VisNode(VisElement reference, float position, float offset)
		{
			Reference = reference;
			Position = position;
			Offset = offset;
		}

		public float Length { get; }
		public Point GetPointUsing(float position, float offset)
		{
			throw new NotImplementedException();
		}

		public float Similarity(IPrimitive p)
		{
			throw new NotImplementedException();
		}
    }
}
