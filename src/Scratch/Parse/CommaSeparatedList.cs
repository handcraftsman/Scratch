using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Scratch.Parse
{
	[TestFixture]
	public class CommaSeparatedList
	{
		// http://stackoverflow.com/questions/14477036/linq-query-selecting-comma-separated-list

		[Test]
		public void Original()
		{
			var allCsvs = new List<Csv>
        {
            new Csv
                {
                    CommaSepList = "1,2,3,4,,5"
                },
            new Csv
                {
                    CommaSepList = "4,5,7,,5,,"
                },
        };

			int[] intArray = allCsvs
		.SelectMany(c => c.CommaSepList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
		.Select(int.Parse)
		.ToArray();
		}	
		
		[Test]
		public void Distinct()
		{
			var allCsvs = new List<Csv>
        {
            new Csv
                {
                    CommaSepList = "1,2,3,4,,5"
                },
            new Csv
                {
                    CommaSepList = "4,5,7,,5,,"
                },
        };

			int[] intArray = allCsvs
		.SelectMany(c => c.CommaSepList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
		.Select(int.Parse)
		.Distinct()
		.ToArray();
		}

		internal class Csv
		{
			public string CommaSepList { get; set; }
		}
	}
}