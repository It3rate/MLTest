﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
	public class VisNode : IPrimitive, IJoiner
    {
	    //public VisJointType JointType { get; }

        public IPath Reference { get; } // if reference is a stroke, this must be a joint
		public float Position { get; }

		public VisNode PreviousNode { get; private set; }
		public VisNode NextNode { get; private set; }

        // calculated
        public Point Anchor { get; }
		public float Length { get; }
		public virtual Point Start => Anchor;
        public Point End { get; }

        public VisNode(IPath reference, float position)
		{
			Reference = reference;
			Position = position;

			Anchor = reference.GetPoint(position);
		}


        public Point GetPoint(float position, float offset=0)
		{
			throw new NotImplementedException();
		}

		public float Similarity(IPrimitive p)
		{
			throw new NotImplementedException();
		}
    }

	public class VisTipNode : VisNode
	{
		// Offset can't be zero in a middle node, as it causes overlap on lines that are tangent to each other. 
		// The corner of a P is part of the shape with potential overlap on the serif.
		// Maybe X could be a V with overlap.H would be a half U with 0.5 overlap. Maybe this is too obfuscated. Yes it is. Might work for serifs though.
        public float Offset { get; }

        public VisTipNode(IPath reference, float position, float offset) : base(reference, position)
		{
			Offset = offset;
        }
		//public VisTipNode(IPath reference, float position, float offset, float length) : base(reference, position)
		//{
		//	Offset = offset;
		//	Length = length;
  //      }
    }

}
