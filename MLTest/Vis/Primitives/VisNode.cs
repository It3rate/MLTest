using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
	public class VisNode : IPrimitive, IJoiner
    {
		public IPath Reference { get; }
		public float Position { get; }
		public float Offset { get; }

		public Point Anchor { get; }

		public VisNode(IPath reference, float position, float offset)
		{
			Reference = reference;
			Position = position;
			Offset = offset;

			Anchor = reference.GetPoint(position, offset);
		}

		public float Length { get; }
		public Point GetPoint(float position, float offset)
		{
			throw new NotImplementedException();
		}

		public float Similarity(IPrimitive p)
		{
			throw new NotImplementedException();
		}
    }
}
