﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using Microsoft.ML.Probabilistic.Distributions;

namespace MLTest.Vis
{

 //   public class VisOval : Circle
 //   {
	//    public override VisElementType ElementType => VisElementType.Oval;

	//    public VisNode PerimeterSide { get; }
	//    public float RadiusSide{ get; }

	//    public VisOval(VisNode center, VisNode perimeterOrigin, VisNode perimeterSide) : base(center, perimeterOrigin)
	//    {
	//	    PerimeterSide = perimeterSide;
	//	    RadiusSide = center.Anchor.DistanceTo(PerimeterSide.Anchor);
 //       }

	//    public override Point GetPoint(float position, float offset)
	//    {
	//	    var rads = Utils.NormToRadians(position);
	//	    return new Point(Anchor.X + (float)Math.Sin(rads) * (Radius + offset), Anchor.Y + (float)Math.Cos(rads) * (Radius + offset));
	//    }
 //       public VisStroke GetElement(VisDirection direction) => null;
 //   }
 //   public class VisSquare : Point
	//{
	//	public override VisElementType ElementType => VisElementType.Square;

	//	public VisStroke Reference { get; }
	//	public float Position => Val0;
	//	public float Radius => Val1;


	//	public override Point Anchor { get; }

	//	public VisSquare(VisStroke reference, float position, float offset) : base(position, offset * reference.Length())
	//	{
	//		Reference = reference;
	//		Anchor = Reference.GetPoint(Position, 0); // start
	//	}

	//	public VisStroke GetElement(VisDirection direction) => null;

	//	public override Point GetPoint(float position, float offset)
	//	{
	//		var rads = Utils.NormToRadians(position);
	//		return new Point(Anchor.X + (float)Math.Sin(rads) * (Radius + offset), Anchor.Y + (float)Math.Cos(rads) * (Radius + offset));
	//	}
 //   }

}