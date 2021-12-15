using ConcernsCaseWork.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Models
{
	[Parallelizable(ParallelScope.All)]
	public class TypeModelTests
	{
		[Test]
		public void WhenTypeModel_IsMapped()
		{
			// arrange
			var typeModel = new TypeModel
			{
				Type = "governance",
				SubType = "governance",
				TypesDictionary = new Dictionary<string, IList<TypeModel.TypeValueModel>>()
			};
			
			// assert
			Assert.That(typeModel, Is.Not.Null);
			Assert.That(typeModel.Type, Is.EqualTo("governance"));
			Assert.That(typeModel.SubType, Is.EqualTo("governance"));
			Assert.That(typeModel.TypeDisplay, Is.EqualTo("governance: governance"));
			Assert.That(typeModel.TypesDictionary, Is.Not.Null);
			Assert.That(typeModel.TypesDictionary.Count, Is.EqualTo(0));
		}
		
		[Test]
		public void WhenTypeValueModel_IsMapped()
		{
			// arrange
			var typeModel = new TypeModel
			{
				Type = "governance",
				SubType = "governance",
				TypesDictionary = new Dictionary<string, IList<TypeModel.TypeValueModel>>
				{
					{
						"typeUrn", 
						new List<TypeModel.TypeValueModel>
						{
							new TypeModel.TypeValueModel{ Urn = 1, SubType = "governance" }	
						}
					}
				}
			};
			
			// assert
			Assert.That(typeModel, Is.Not.Null);
			Assert.That(typeModel.Type, Is.EqualTo("governance"));
			Assert.That(typeModel.SubType, Is.EqualTo("governance"));
			Assert.That(typeModel.TypeDisplay, Is.EqualTo("governance: governance"));
			Assert.That(typeModel.TypesDictionary, Is.Not.Null);
			Assert.That(typeModel.TypesDictionary.Count, Is.EqualTo(1));
			Assert.That(typeModel.TypesDictionary.First().Key, Is.EqualTo("typeUrn"));
			Assert.That(typeModel.TypesDictionary.First().Value, Is.Not.Null);
			Assert.That(typeModel.TypesDictionary.First().Value.First(), Is.Not.Null);
			Assert.That(typeModel.TypesDictionary.First().Value.First().Urn, Is.EqualTo(1));
			Assert.That(typeModel.TypesDictionary.First().Value.First().SubType, Is.EqualTo("governance"));
		}
	}
}