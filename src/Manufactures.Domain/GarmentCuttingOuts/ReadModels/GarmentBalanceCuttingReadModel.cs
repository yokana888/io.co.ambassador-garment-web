﻿using Infrastructure.Domain.ReadModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Domain.GarmentCuttingOuts.ReadModels
{

	public class GarmentBalanceCuttingReadModel : ReadModelBase
	{
		public GarmentBalanceCuttingReadModel(Guid identity) : base(identity)
		{
		}
		public string RoJob { get; internal set; }
		public string Article { get; internal set; }
		public int UnitId { get; internal set; }
		public string UnitCode { get; internal set; }
		public string UnitName { get; internal set; }
		public string BuyerCode { get; internal set; }
		public double QtyOrder { get; internal set; }
		public string Style { get; internal set; }
		public double Hours { get; internal set; }
		public double Stock { get; internal set; }
		public double CuttingQtyMeter { get; internal set; }
		public double CuttingQtyPcs { get; internal set; }
		public double Fc { get; internal set; }
		public double Expenditure { get; internal set; }
		public double RemainQty { get; internal set; }
		public decimal Price { get; internal set; }
		public decimal Nominal { get; internal set; }
	}
}
